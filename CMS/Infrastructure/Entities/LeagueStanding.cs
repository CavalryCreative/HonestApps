using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class LeagueStanding : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int APIId { get; set; }
        public string Name { get; set; }
        public byte Position { get; set; }
        public byte GamesPlayed { get; set; }
        public byte GamesWon { get; set; }
        public byte GamesDrawn { get; set; }
        public byte GamesLost { get; set; }
        public byte GoalsScored { get; set; }
        public byte GoalsConceded { get; set; }
        public int GoalDifference { get; set; }
        public byte Points { get; set; }
        public string Description { get; set; }
    }
}