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

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Configurations.Add(new TeamMap());
                modelBuilder.Configurations.Add(new MatchMap());
                modelBuilder.Configurations.Add(new EventMap());
                modelBuilder.Configurations.Add(new GoalMap());
                modelBuilder.Configurations.Add(new CardMap());
                modelBuilder.Configurations.Add(new PlayerMap());
                modelBuilder.Configurations.Add(new SubstitutionMap());
            }
        }

        public class TeamMap : EntityTypeConfiguration<Team>
        {
            public TeamMap()
            {
                //HasMany(p => p.Matches).WithMany(c => c.Teams).Map(m =>
                //{
                //    m.MapLeftKey("MatchId");
                //    m.MapRightKey("TeamId");
                //});

                //Property(p => p.URL).HasMaxLength(500).IsUnicode(false);
                //Property(p => p.JobRef).HasMaxLength(50).IsUnicode(false);
                //Property(p => p.Title).HasMaxLength(100).IsUnicode(false);
                //Property(p => p.Description).IsUnicode(false);
                //Property(p => p.Summary).HasMaxLength(5000).IsUnicode(false);
                //Property(p => p.Keywords).HasMaxLength(5000).IsUnicode(false);
                //Property(p => p.MetaDescription).HasMaxLength(5000).IsUnicode(false);
                //Property(p => p.Location).HasMaxLength(1000).IsUnicode(false);
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
            }
        }

        public class EventMap : EntityTypeConfiguration<Event>
        {
            public EventMap()
            {
                Property(p => p.Comment).HasMaxLength(1000).IsUnicode(false);
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

        public class SubstitutionMap : EntityTypeConfiguration<Substitution>
        {
            public SubstitutionMap()
            {
                Property(p => p.PlayerOn).HasMaxLength(250).IsUnicode(false);
                Property(p => p.PlayerOff).HasMaxLength(250).IsUnicode(false);
            }
        }
}