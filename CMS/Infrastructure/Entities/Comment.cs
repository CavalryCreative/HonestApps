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
        public byte TeamRating { get; set; }
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
        Match = 3,
        Rivals = 4
    }

    public enum EventType
    {
        AttemptBlocked = 1,
        AttemptMissedTooHigh = 2,
        AttemptMissedHighAndWide = 3,
        AttemptMissesToRightOrLeft = 4,
        AttemptMissesJustABitTooHigh = 5,
        AttemptMissed = 6,
        AttemptSaved = 7,
        FreeKick = 8,     
        YellowCard = 9,
        RedCard = 10,
        Corner = 11,
        Delay = 12,
        DelayEnds = 13,
        FirstHalfBegins = 14,
        FirstHalfEnds = 15,
        Foul = 16,
        Goal = 17,
        LineupsAnnounced = 18,
        MatchEnds = 19,
        Offside = 20,
        HitsTheBar = 21,
        HitsThePost = 22,
        SecondHalfBegins = 23,
        Substitution = 24,
        Handball = 25,
        General = 26,
        SecondHalfEnds = 27,
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
