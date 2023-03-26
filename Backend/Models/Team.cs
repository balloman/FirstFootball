using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstFootball.Backend.Models;

public class Team
{
    [Key]
    public required string Name { get; set; }
    [InverseProperty("HomeTeam")]
    public required List<Fixture> HomeFixtures { get; set; }
    [InverseProperty("AwayTeam")]
    public required List<Fixture> AwayFixtures { get; set; }

    private sealed class NameEqualityComparer : IEqualityComparer<Team>
    {
        public bool Equals(Team? x, Team? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(Team obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public static IEqualityComparer<Team> NameComparer { get; } = new NameEqualityComparer();
}
