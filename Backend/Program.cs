using FirstFootball.Backend.Configuration;
using FirstFootball.Backend.Features;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<BackendContext>(options =>
{
    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("Default");
    var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
    {
        Password = configuration["DB_PASS"]
    };
    options.UseNpgsql(connectionStringBuilder.ConnectionString,
            b => b.UseNodaTime()
                .EnableRetryOnFailure())
        .UseSnakeCaseNamingConvention();
});
builder.Services.Configure<YoutubeConfig>(builder.Configuration.GetRequiredSection(YoutubeConfig.ROOT));
builder.Services.AddSingleton<YouTubeService>(services => new YouTubeService(new BaseClientService.Initializer
{
    ApiKey = services.GetRequiredService<IOptions<YoutubeConfig>>().Value.ApiKey
}));
builder.Services
    .AddHttpClient(Constants.CLIENT_NAME)
    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3)));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/fixtures", GetFixtures.HandleAsync);

if (app.Environment.IsDevelopment()) app.Run();
var url = $"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}";
app.Run(url);

namespace FirstFootball.Backend
{
    public static class Constants
    {
        public const string CLIENT_NAME = "client";
    }
}

