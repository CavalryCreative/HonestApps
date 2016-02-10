using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Text { get; set; }
        public byte MatchRating { get; set; }
        public byte PlayerRating { get; set; }
        public byte CommentType { get; set; }
        public byte EventType { get; set; }
        public byte Perspective { get; set; }
        public byte Position { get; set; }
    }

    public enum Perspective
    {
        Positive = 1,
        Neutral = 2,
        Negative = 3
    }

    public enum CommentType
    {
        Team = 1,
        Player = 2,
        Game = 3,
        Rivals = 4
    }

    public enum EventType
    {
        AttemptBlocked = 1,
        AttemptMissedTooHigh = 2,
        AttemptMissedHighAndWide = 3,
        AttemptMissesToRightOrLeft = 4,
        AttemptMissesJustABitTooHigh = 5,
        AttemptSaved = 6,
        FreeKick = 7,     
        YellowCard = 8,
        RedCard = 9,
        Corner = 10,
        Delay = 11,
        DelayEnds = 12,
        FirstHalfBegins = 13,
        FirstHalfEnds = 14,
        Foul = 15,
        Goal = 16,
        LineupsAnnounced = 17,
        MatchEnds = 18,
        Offside = 19,
        HitsTheBar = 20,
        HitsThePost = 21,
        SecondHalfBegins = 22,
        Substitution = 23,
        Handball = 24
    }

    public enum Position
    {
        All = 0,
        Goalkeeper = 1,
        Defender = 2,
        Midfielder = 3,
        Forward = 4
    }
}
