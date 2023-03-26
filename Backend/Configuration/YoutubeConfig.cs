namespace FirstFootball.Backend.Configuration;

public class YoutubeConfig
{
    public const string ROOT = "Youtube";
    
    public required string ApiKey { get; set; }
    public required string PlaylistId { get; set; }
    public required string LiveScoreApiKey { get; set; }
}
