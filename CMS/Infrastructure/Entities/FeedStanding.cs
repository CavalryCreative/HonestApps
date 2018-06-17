﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedStanding
    {
        public string Name { get; set; }
        public int TeamAPI { get; set; }
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