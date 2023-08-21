using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using FirstFootball.Backend.Configuration;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using JorgeSerrano.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NodaTime.Text;

namespace FirstFootball.Backend.Features;

public class UpdateFixtures
{
    public static async Task<IResult> HandleAsync(IDbContextFactory<BackendContext> dbContextFactory, YouTubeService service, 
                                                  IOptions<YoutubeConfig> config, IHttpClientFactory clientFactory,
                                                  ILogger<UpdateFixtures> logger, ClaimsPrincipal user, 
                                                  IWebHostEnvironment env)
    {
        if (env.IsProduction())
        {
            if (!user.HasClaim("azp", config.Value.ServiceAccountUid)) return Results.Forbid();
        }
        var client = clientFactory.CreateClient(Constants.CLIENT_NAME);
        var liveScoreResponse = await GetPremierLeagueFixtures(client, config.Value.LiveScoreApiKey);
        var parts = new[] { "snippet", "contentDetails" };
        var playlistItemsListRequest = service.PlaylistItems.List(parts);
        playlistItemsListRequest.PlaylistId = config.Value.PlaylistId;
        playlistItemsListRequest.MaxResults = 50;
        var nextPageToken = ""; // Continue to loop until there are no more videos in the playlist
        var fixtureBuilder = ImmutableArray.CreateBuilder<Fixture>();
        while (true)
        {
            playlistItemsListRequest.PageToken = nextPageToken;
            var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();
            nextPageToken = playlistItemsListResponse.NextPageToken;
            fixtureBuilder.AddRange(playlistItemsListResponse.Items
                .Select(item => PlaylistItemToFixture(item, liveScoreResponse.Data, logger))
                .Where(fixture => fixture is not null)
                .Select(fixture => fixture!));
            if (string.IsNullOrWhiteSpace(playlistItemsListResponse.NextPageToken)) break;
        }
        logger.LogInformation("Found {Count} fixtures", fixtureBuilder.Count);
        var fixtures = fixtureBuilder.ToImmutable();
        // Find the exising teams
        await using var teamsContext = await dbContextFactory.CreateDbContextAsync();
        var teams = fixtures.SelectMany(fixture => new[] { fixture.HomeTeam, fixture.AwayTeam })
            .DistinctBy(team => team.Name)
            .ToList();
        var correctTeams = await GetExistingTeams(teamsContext, teams);
        // Update the teams
        UpdateTeamsWithFixtures(correctTeams, fixtures);
        await teamsContext.SaveChangesAsync();
        await using var finalContext = await dbContextFactory.CreateDbContextAsync();
        finalContext.AttachRange(correctTeams);
        var existingFixtures = await finalContext.Fixtures
            .Where(fixture => fixtures.Contains(fixture))
            .ToListAsync();
        var fixturesToAdd = fixtures.Except(existingFixtures, Fixture.IdComparer).ToList();
        foreach (var fixture in fixturesToAdd)
        {
            finalContext.Entry(fixture).State = EntityState.Added;
        }
        logger.LogInformation("Added {Count} fixtures", fixturesToAdd.Count);
        await finalContext.SaveChangesAsync();
        return Results.Ok();
    }

    private static async Task<LiveScoreResponse> GetPremierLeagueFixtures(HttpClient client, string apiKey)
    {
        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "livescore-football.p.rapidapi.com");
        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
        const string baseUrl = "https://livescore-football.p.rapidapi.com/soccer/matches-by-league";
        var queryParams = new Dictionary<string, string?>
        {
            { "country_code", "england" },
            { "league_code", "premier-league" },
        };
        var newUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);
        var response = await client.GetAsync(newUrl);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<LiveScoreResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }) ?? throw new Exception("Could not deserialize response");
    }

    private static Fixture? PlaylistItemToFixture(PlaylistItem item, IEnumerable<LiveScoreFixture> liveScoreFixtures,
                                                  ILogger<UpdateFixtures> logger)
    {
        var title = item.Snippet.Title;
        if (!title.Contains("| PREMIER LEAGUE HIGHLIGHTS |")) return null;
        var team1 = title.Split("v.")[0].Trim();
        var team2 = title.Split("v.")[1].Split("|")[0].Trim();
        if (team1.Contains("Wolves")) team1 = team1.Replace("Wolves", "Wolverhampton Wanderers");
        if (team2.Contains("Wolves")) team2 = team2.Replace("Wolves", "Wolverhampton Wanderers");
        var liveScoreFixture = liveScoreFixtures.FirstOrDefault(f => 
            f.Team_1.Name.Contains(team1, StringComparison.InvariantCultureIgnoreCase) && 
            f.Team_2.Name.Contains(team2, StringComparison.InvariantCultureIgnoreCase));
        if (liveScoreFixture is null)
        {
            logger.LogWarning("Could not find fixture for {Title}", title);
            return null;
        }
        var homeTeam = new Team
        {
            Name = liveScoreFixture.Team_1.Name,
            AwayFixtures = new List<Fixture>(),
            HomeFixtures = new List<Fixture>(),
        };
        var awayTeam = new Team
        {
            Name = liveScoreFixture.Team_2.Name,
            AwayFixtures = new List<Fixture>(),
            HomeFixtures = new List<Fixture>(),
        };
        var instant = InstantPattern.General.Parse(item.ContentDetails.VideoPublishedAtRaw);
        if (!instant.Success)
        {
            logger.LogWarning("Could not parse date for {Title}", title);
            return null;
        }
        var fixture = new Fixture
        {
            Id = item.ContentDetails.VideoId,
            AwayScore = liveScoreFixture.Score.FullTime.Team_2,
            HomeScore = liveScoreFixture.Score.FullTime.Team_1,
            AwayTeam = awayTeam,
            HomeTeam = homeTeam,
            MatchWeek = int.Parse(liveScoreFixture.RoundInfo),
            DatePosted = instant.Value
        };
        logger.LogInformation("Created fixture for {Title}: {Fixture}", title, fixture);
        return fixture;
    }

    private static async Task<List<Team>> GetExistingTeams(BackendContext dbContext, ICollection<Team> teams)
    {
        var existingTeams = await dbContext.Teams
            .Where(team => teams.Contains(team))
            .ToListAsync();
        var newTeams = teams.Except(existingTeams, Team.NameComparer).ToList();
        dbContext.AddRange(newTeams);
        return existingTeams.Concat(newTeams).ToList();
    }

    private static void UpdateTeamsWithFixtures(
        ICollection<Team> teams, IEnumerable<Fixture> fixtures)
    {
        foreach (var fixture in fixtures)
        {
            var homeTeam = teams.First(team => team.Name == fixture.HomeTeam.Name);
            var awayTeam = teams.First(team => team.Name == fixture.AwayTeam.Name);
            fixture.HomeTeam = homeTeam;
            fixture.AwayTeam = awayTeam;
        }
    }

    private record LiveScoreResponse(int Status, LiveScoreFixture[] Data);

    // ReSharper disable NotAccessedPositionalProperty.Local
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private record LiveScoreFixture(string Round, string MatchId, string Status, LiveScoreTeam Team_2, LiveScoreTeam Team_1,
                                    LiveScoreScoreContainer Score, string RoundInfo);

    private record LiveScoreTeam(string Id, string Country, string Name, string Logo);

    private record LiveScoreScoreContainer(LiveScoreScore FullTime, LiveScoreScore HalfTime);
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private record LiveScoreScore(int Team_1, int Team_2);
}
