using NodaTime;

namespace FirstFootball.Backend.Models;

[Index(nameof(DatePosted))]
public class Fixture
{
    public required string Id { get; set; }
    public required Team HomeTeam { get; set; }
    public required Team AwayTeam { get; set; }
    public required int HomeScore { get; set; }
    public required int AwayScore { get; set; }
    public required int MatchWeek { get; set; }
    public required Instant DatePosted { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Id)}: {Id}, {nameof(HomeTeam)}: {HomeTeam}, {nameof(AwayTeam)}: {AwayTeam}, " +
        $"{nameof(HomeScore)}: {HomeScore}, {nameof(AwayScore)}: {AwayScore}, {nameof(MatchWeek)}: {MatchWeek}, {nameof(DatePosted)}: " +
        $"{DatePosted}";

    private sealed class IdEqualityComparer : IEqualityComparer<Fixture>
    {
        public bool Equals(Fixture x, Fixture y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(Fixture obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public static IEqualityComparer<Fixture> IdComparer { get; } = new IdEqualityComparer();
}
