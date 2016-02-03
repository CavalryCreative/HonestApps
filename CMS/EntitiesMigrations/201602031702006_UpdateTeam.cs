namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Cards",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false),
            //            SummaryId = c.Guid(nullable: false),
            //            IsHomeTeam = c.Boolean(nullable: false),
            //            PlayerName = c.String(maxLength: 250, unicode: false),
            //            APIPlayerId = c.Int(nullable: false),
            //            Minute = c.Byte(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //            Discriminator = c.String(nullable: false, maxLength: 128),
            //            Summary_Id = c.Guid(),
            //            Summary_Id1 = c.Guid(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Summaries", t => t.Summary_Id)
            //    .ForeignKey("dbo.Summaries", t => t.Summary_Id1)
            //    .Index(t => t.Summary_Id)
            //    .Index(t => t.Summary_Id1);
            
            //CreateTable(
            //    "dbo.Events",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchId = c.Guid(nullable: false),
            //            Important = c.Boolean(nullable: false),
            //            Goal = c.Boolean(nullable: false),
            //            Minute = c.Byte(nullable: false),
            //            Score = c.String(maxLength: 250, unicode: false),
            //            Comment = c.String(maxLength: 1000, unicode: false),
            //            APIId = c.Int(nullable: false),
            //            HomeTeamMatchRating = c.Byte(nullable: false),
            //            AwayTeamMatchRating = c.Byte(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
            //    .Index(t => t.MatchId);
            
            //CreateTable(
            //    "dbo.Goals",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            SummaryId = c.Guid(nullable: false),
            //            IsHomeTeam = c.Boolean(nullable: false),
            //            PlayerName = c.String(maxLength: 250, unicode: false),
            //            APIPlayerId = c.Int(nullable: false),
            //            Minute = c.Byte(nullable: false),
            //            OwnGoal = c.Boolean(nullable: false),
            //            Penalty = c.Boolean(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Summaries", t => t.SummaryId, cascadeDelete: true)
            //    .Index(t => t.SummaryId);
            
            //CreateTable(
            //    "dbo.Lineups",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchAPIId = c.Int(nullable: false),
            //            IsHomePlayer = c.Boolean(nullable: false),
            //            IsSub = c.Boolean(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //            Match_Id = c.Guid(),
            //            Team_Id = c.Guid(),
            //            Player_Id = c.Guid(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Matches", t => t.Match_Id)
            //    .ForeignKey("dbo.Teams", t => t.Team_Id)
            //    .ForeignKey("dbo.Players", t => t.Player_Id)
            //    .Index(t => t.Match_Id)
            //    .Index(t => t.Team_Id)
            //    .Index(t => t.Player_Id);
            
            //CreateTable(
            //    "dbo.Players",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            SquadNumber = c.Byte(nullable: false),
            //            Name = c.String(maxLength: 250, unicode: false),
            //            Position = c.String(maxLength: 100, unicode: false),
            //            APIPlayerId = c.Int(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.PlayerStats",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            PlayerId = c.Guid(nullable: false),
            //            MatchId = c.Guid(nullable: false),
            //            PositionX = c.Byte(nullable: false),
            //            PositionY = c.Byte(nullable: false),
            //            TotalShots = c.Byte(),
            //            ShotsOnGoal = c.Byte(),
            //            Goals = c.Byte(),
            //            Assists = c.Byte(),
            //            Offsides = c.Byte(),
            //            FoulsDrawn = c.Byte(),
            //            FoulsCommitted = c.Byte(),
            //            Saves = c.Byte(),
            //            YellowCards = c.Byte(),
            //            RedCards = c.Byte(),
            //            PenaltiesScored = c.Byte(),
            //            PenaltiesMissed = c.Byte(),
            //            APIId = c.Int(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
            //    .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
            //    .Index(t => t.PlayerId)
            //    .Index(t => t.MatchId);
            
            //CreateTable(
            //    "dbo.Teams",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            APIId = c.Int(nullable: false),
            //            Name = c.String(maxLength: 500, unicode: false),
            //            Stadium = c.String(maxLength: 500, unicode: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Matches",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            APIId = c.Int(nullable: false),
            //            Stadium = c.String(maxLength: 100, unicode: false),
            //            Attendance = c.String(maxLength: 10, unicode: false),
            //            Time = c.String(maxLength: 20, unicode: false),
            //            Referee = c.String(maxLength: 100, unicode: false),
            //            Date = c.DateTime(),
            //            EndDate = c.DateTime(),
            //            HomeTeamId = c.Guid(nullable: false),
            //            AwayTeamId = c.Guid(nullable: false),
            //            HomeTeamAPIId = c.Int(nullable: false),
            //            AwayTeamAPIId = c.Int(nullable: false),
            //            IsToday = c.Boolean(nullable: false),
            //            IsLive = c.Boolean(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //            Team_Id = c.Guid(),
            //            Team_Id1 = c.Guid(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Teams", t => t.Team_Id)
            //    .ForeignKey("dbo.Teams", t => t.Team_Id1)
            //    .Index(t => t.Team_Id)
            //    .Index(t => t.Team_Id1);
            
            //CreateTable(
            //    "dbo.Stats",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchId = c.Guid(nullable: false),
            //            HomeTeamTotalShots = c.Byte(),
            //            HomeTeamOnGoalShots = c.Byte(),
            //            HomeTeamFouls = c.Byte(),
            //            HomeTeamCorners = c.Byte(),
            //            HomeTeamOffsides = c.Byte(),
            //            HomeTeamPossessionTime = c.String(),
            //            HomeTeamYellowCards = c.Byte(),
            //            HomeTeamRedCards = c.Byte(),
            //            HomeTeamSaves = c.Byte(),
            //            AwayTeamTotalShots = c.Byte(),
            //            AwayTeamOnGoalShots = c.Byte(),
            //            AwayTeamFouls = c.Byte(),
            //            AwayTeamCorners = c.Byte(),
            //            AwayTeamOffsides = c.Byte(),
            //            AwayTeamPossessionTime = c.String(),
            //            AwayTeamYellowCards = c.Byte(),
            //            AwayTeamRedCards = c.Byte(),
            //            AwayTeamSaves = c.Byte(),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
            //    .Index(t => t.MatchId);
            
            //CreateTable(
            //    "dbo.Substitutions",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchId = c.Guid(nullable: false),
            //            PlayerOff = c.String(maxLength: 250, unicode: false),
            //            PlayerOffId = c.Guid(nullable: false),
            //            APIPlayerOffId = c.Int(nullable: false),
            //            PlayerOn = c.String(maxLength: 250, unicode: false),
            //            PlayerOnId = c.Guid(nullable: false),
            //            APIPlayerOnId = c.Int(nullable: false),
            //            Minute = c.Byte(nullable: false),
            //            IsHomeTeam = c.Boolean(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
            //    .Index(t => t.MatchId);
            
            //CreateTable(
            //    "dbo.Summaries",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchId = c.Guid(nullable: false),
            //            HomeTeam = c.Guid(nullable: false),
            //            AwayTeam = c.Guid(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
            //    .Index(t => t.MatchId);
            
            //CreateTable(
            //    "dbo.Rivals",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            TeamId = c.Guid(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
            //    .Index(t => t.TeamId);
            
            //CreateTable(
            //    "dbo.MatchesTodays",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            APIId = c.Int(nullable: false),
            //            KickOffTime = c.String(maxLength: 10, unicode: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.UpdateHistories",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchAPIId = c.Int(nullable: false),
            //            MatchDetails = c.Boolean(nullable: false),
            //            Lineups = c.Boolean(nullable: false),
            //            MatchStats = c.Boolean(nullable: false),
            //            CreatedByUserId = c.Guid(),
            //            DateAdded = c.DateTime(),
            //            Active = c.Boolean(),
            //            Deleted = c.Boolean(),
            //            UpdatedByUserId = c.Guid(),
            //            DateUpdated = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.TeamPlayers",
            //    c => new
            //        {
            //            Team_Id = c.Guid(nullable: false),
            //            Player_Id = c.Guid(nullable: false),
            //        })
            //    .PrimaryKey(t => new { t.Team_Id, t.Player_Id })
            //    .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
            //    .ForeignKey("dbo.Players", t => t.Player_Id, cascadeDelete: true)
            //    .Index(t => t.Team_Id)
            //    .Index(t => t.Player_Id);

            DropTable("dbo.Matches");
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Lineups", "Player_Id", "dbo.Players");
            //DropForeignKey("dbo.Rivals", "TeamId", "dbo.Teams");
            //DropForeignKey("dbo.TeamPlayers", "Player_Id", "dbo.Players");
            //DropForeignKey("dbo.TeamPlayers", "Team_Id", "dbo.Teams");
            //DropForeignKey("dbo.Lineups", "Team_Id", "dbo.Teams");
            //DropForeignKey("dbo.Matches", "Team_Id1", "dbo.Teams");
            //DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            //DropForeignKey("dbo.Summaries", "MatchId", "dbo.Matches");
            //DropForeignKey("dbo.Cards", "Summary_Id1", "dbo.Summaries");
            //DropForeignKey("dbo.Cards", "Summary_Id", "dbo.Summaries");
            //DropForeignKey("dbo.Goals", "SummaryId", "dbo.Summaries");
            //DropForeignKey("dbo.Substitutions", "MatchId", "dbo.Matches");
            //DropForeignKey("dbo.Stats", "MatchId", "dbo.Matches");
            //DropForeignKey("dbo.PlayerStats", "MatchId", "dbo.Matches");
            //DropForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches");
            //DropForeignKey("dbo.Events", "MatchId", "dbo.Matches");
            //DropForeignKey("dbo.PlayerStats", "PlayerId", "dbo.Players");
            //DropIndex("dbo.Lineups", new[] { "Player_Id" });
            //DropIndex("dbo.Rivals", new[] { "TeamId" });
            //DropIndex("dbo.TeamPlayers", new[] { "Player_Id" });
            //DropIndex("dbo.TeamPlayers", new[] { "Team_Id" });
            //DropIndex("dbo.Lineups", new[] { "Team_Id" });
            //DropIndex("dbo.Matches", new[] { "Team_Id1" });
            //DropIndex("dbo.Matches", new[] { "Team_Id" });
            //DropIndex("dbo.Summaries", new[] { "MatchId" });
            //DropIndex("dbo.Cards", new[] { "Summary_Id1" });
            //DropIndex("dbo.Cards", new[] { "Summary_Id" });
            //DropIndex("dbo.Goals", new[] { "SummaryId" });
            //DropIndex("dbo.Substitutions", new[] { "MatchId" });
            //DropIndex("dbo.Stats", new[] { "MatchId" });
            //DropIndex("dbo.PlayerStats", new[] { "MatchId" });
            //DropIndex("dbo.Lineups", new[] { "Match_Id" });
            //DropIndex("dbo.Events", new[] { "MatchId" });
            //DropIndex("dbo.PlayerStats", new[] { "PlayerId" });
            //DropTable("dbo.TeamPlayers");
            //DropTable("dbo.UpdateHistories");
            //DropTable("dbo.MatchesTodays");
            //DropTable("dbo.Rivals");
            //DropTable("dbo.Summaries");
            //DropTable("dbo.Substitutions");
            //DropTable("dbo.Stats");
            //DropTable("dbo.Matches");
            //DropTable("dbo.Teams");
            //DropTable("dbo.PlayerStats");
            //DropTable("dbo.Players");
            //DropTable("dbo.Lineups");
            //DropTable("dbo.Goals");
            //DropTable("dbo.Events");
            //DropTable("dbo.Cards");
        }
    }
}
