using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<ChatSession> ChatSessions { get; set; }

    public DbSet<Agent> Agents { get; set; }

    public DbSet<ChatTeam> chatTeams { get; set; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
    ) :
        base(options)
    {
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder
    )
    {
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AgentSession");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ChatTeam>()
            .HasData(new List<Agent>()
            {
                new Agent()
                {
                    Name = "John",
                    ConcurrentChats = 10,
                    Seniority = AgentSeniority.Junior
                },
                new Agent()
                {
                    Name = "Sarah",
                    ConcurrentChats = 10,
                    Seniority = AgentSeniority.MidLevel
                },
                new Agent()
                {
                    Name = "Alex",
                    ConcurrentChats = 10,
                    Seniority = AgentSeniority.Senior
                },
                new Agent()
                {
                    Name = "David",
                    ConcurrentChats = 10,
                    Seniority = AgentSeniority.TeamLead
                }
            });

        modelBuilder
            .Entity<ChatTeam>()
            .HasData(new List<ChatTeam>()
            {
                new ChatTeam()
                {
                    TeamName = "Team A",
                    Agents =
                        new List<UserRole> {
                            UserRole.TeamLead,
                            UserRole.MidLevel,
                            UserRole.MidLevel,
                            UserRole.Junior
                        }
                },
                new ChatTeam()
                {
                    TeamName = "Team B",
                    Agents =
                        new List<UserRole> {
                            UserRole.Senior,
                            UserRole.MidLevel,
                            UserRole.Junior,
                            UserRole.Junior
                        }
                },
                new ChatTeam()
                {
                    TeamName = "Team C",
                    Agents =
                        new List<UserRole> {
                            UserRole.MidLevel,
                            UserRole.MidLevel
                        }
                }
            });
    }
}
