using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Infrastructure.Concrete
{
        public class EFDbContext : DbContext
        {
            public EFDbContext() : base("DefaultConnection") { }          
           
            public DbSet<Event> Events { get; set; }
            public DbSet<Lineup> Lineups { get; set; }
            public DbSet<Match> Matches { get; set; }
            public DbSet<Player> Players { get; set; }
            public DbSet<PlayerStat> PlayerStats { get; set; }
            public DbSet<Rival> Rivals { get; set; }
            public DbSet<Stat> Stats { get; set; }
            public DbSet<Substitution> Substitutions { get; set; }
            public DbSet<Summary> Summaries { get; set; }
            public DbSet<Team> Teams { get; set; }
            public DbSet<Goal> Goals { get; set; }
            public DbSet<Card> Cards { get; set; }
            public DbSet<UpdateHistory> UpdateHistory { get; set; }
            public DbSet<MatchesToday> MatchesToday { get; set; }
            public DbSet<SiteException> SiteException { get; set; }
            public DbSet<BroadcastFeed> BroadcastFeed { get; set; }
            public DbSet<Comment> Comment { get; set; }
            public DbSet<LeagueStanding> LeagueStanding { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Configurations.Add(new TeamMap());
                modelBuilder.Configurations.Add(new MatchMap());
                modelBuilder.Configurations.Add(new EventMap());
                modelBuilder.Configurations.Add(new GoalMap());
                modelBuilder.Configurations.Add(new CardMap());
                modelBuilder.Configurations.Add(new PlayerMap());
                modelBuilder.Configurations.Add(new SubstitutionMap());
                modelBuilder.Configurations.Add(new MatchesTodayMap());
                modelBuilder.Configurations.Add(new SiteExceptionMap());
                modelBuilder.Configurations.Add(new BroadcastFeedMap());
                modelBuilder.Configurations.Add(new CommentMap());
                modelBuilder.Configurations.Add(new LineupMap());
                modelBuilder.Configurations.Add(new LeagueStandingMap());
            }
        }

        public class TeamMap : EntityTypeConfiguration<Team>
        {
            public TeamMap()
            {
                Property(p => p.Name).HasMaxLength(500).IsUnicode(false);
                Property(p => p.Stadium).HasMaxLength(500).IsUnicode(false);
                Property(p => p.PrimaryColour).HasMaxLength(50).IsUnicode(false);
                Property(p => p.SecondaryColour).HasMaxLength(50).IsUnicode(false);
            }
        }

        public class MatchMap : EntityTypeConfiguration<Match>
        {
            public MatchMap()
            {
                HasMany(p => p.Events).WithRequired().HasForeignKey(ph => ph.MatchId);

                Property(p => p.Stadium).HasMaxLength(100).IsUnicode(false);
                Property(p => p.Attendance).HasMaxLength(10).IsUnicode(false);
                Property(p => p.Time).HasMaxLength(20).IsUnicode(false);
                Property(p => p.Referee).HasMaxLength(100).IsUnicode(false);
                Property(p => p.HalfTimeScore).HasMaxLength(10).IsUnicode(false);
                Property(p => p.FullTimeScore).HasMaxLength(10).IsUnicode(false);
            }
        }

        public class EventMap : EntityTypeConfiguration<Event>
        {
            public EventMap()
            {
                Property(p => p.Comment).HasMaxLength(1000).IsUnicode(false);
                Property(p => p.Score).HasMaxLength(250).IsUnicode(false);
            }
        }

        public class GoalMap : EntityTypeConfiguration<Goal>
        {
            public GoalMap()
            {
                Property(p => p.PlayerName).HasMaxLength(250).IsUnicode(false);
            }
        }

        public class CardMap : EntityTypeConfiguration<Card>
        {
            public CardMap()
            {
                Property(p => p.PlayerName).HasMaxLength(250).IsUnicode(false);
            }
        }

        public class PlayerMap : EntityTypeConfiguration<Player>
        {
            public PlayerMap()
            {
                Property(p => p.Name).HasMaxLength(250).IsUnicode(false);
                Property(p => p.Position).HasMaxLength(100).IsUnicode(false);
            }
        }

    public class LineupMap : EntityTypeConfiguration<Lineup>
    {
        public LineupMap()
        {
            Property(p => p.Position).HasMaxLength(100).IsUnicode(false);
        }
    }

    public class SubstitutionMap : EntityTypeConfiguration<Substitution>
        {
            public SubstitutionMap()
            {
                Property(p => p.PlayerOn).HasMaxLength(250).IsUnicode(false);
                Property(p => p.PlayerOff).HasMaxLength(250).IsUnicode(false);
            }
        }

        public class MatchesTodayMap : EntityTypeConfiguration<MatchesToday>
        {
            public MatchesTodayMap()
            {
                Property(p => p.KickOffTime).HasMaxLength(10).IsUnicode(false);
            }
        }

        public class SiteExceptionMap : EntityTypeConfiguration<SiteException>
        {
            public SiteExceptionMap()
            {
                Property(p => p.HResult).HasMaxLength(50).IsUnicode(false);
                Property(p => p.InnerException).HasMaxLength(500).IsUnicode(false);
                Property(p => p.Message).HasMaxLength(500).IsUnicode(false);
                Property(p => p.Source).HasMaxLength(250).IsUnicode(false);
                Property(p => p.StackTrace).IsUnicode(false);
                Property(p => p.TargetSite).HasMaxLength(250).IsUnicode(false);
            }
        }

        public class BroadcastFeedMap : EntityTypeConfiguration<BroadcastFeed>
        {
            public BroadcastFeedMap()
            {
                Property(p => p.Message).HasMaxLength(500).IsUnicode(false);
                Property(p => p.IPAddress).HasMaxLength(100).IsUnicode(false);
            }
        }

        public class CommentMap : EntityTypeConfiguration<Comment>
        {
            public CommentMap()
            {
                Property(p => p.Text).HasMaxLength(500).IsUnicode(false);
            }
        }

    public class LeagueStandingMap : EntityTypeConfiguration<LeagueStanding>
    {
        public LeagueStandingMap()
        {
            Property(p => p.Name).HasMaxLength(500).IsUnicode(false);
            Property(p => p.Description).HasMaxLength(500).IsUnicode(false);
        }
    }
}