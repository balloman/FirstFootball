using System.Text.Json.Serialization;
using FirstFootball.Backend.Configuration;
using FirstFootball.Backend.Features;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MicroElements.Swashbuckle.NodaTime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FirstFootball.Backend", Version = "v1"
    });
    c.ConfigureForNodaTimeWithSystemTextJson();
});
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddCors(options => options.AddDefaultPolicy(b =>
{
    b.AllowAnyOrigin();
    b.AllowAnyHeader();
    b.AllowAnyMethod();
}));
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
        options.Audience = "backend";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/fixtures", GetFixtures.HandleAsync).WithName("GetFixtures").WithOpenApi();
app.MapPost("/fixtures", UpdateFixtures.HandleAsync).RequireAuthorization().ExcludeFromDescription();
app.Run();


namespace FirstFootball.Backend
{
    public static class Constants
    {
        public const string CLIENT_NAME = "client";
    }
}

