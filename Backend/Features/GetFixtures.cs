using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace FirstFootball.Backend.Features;

public class GetFixtures
{
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Fixture>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> HandleAsync(int? limit, long? beforeMs, IDbContextFactory<BackendContext> dbContextFactory,
                                                  IClock clock)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var instant = beforeMs is null ? clock.GetCurrentInstant() : Instant.FromUnixTimeMilliseconds(beforeMs.Value);
        var fixtures = await context.Fixtures
            .Where(fixture => fixture.DatePosted < instant)
            .OrderByDescending(fixture => fixture.DatePosted)
            .Take(limit ?? 50)
            .ToListAsync();
        return Results.Ok(fixtures);
    }
}
