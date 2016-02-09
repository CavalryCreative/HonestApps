using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class PlayerStat : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid MatchId { get; set; }
        public byte PositionX { get; set; }
        public byte PositionY { get; set; }
        public byte? TotalShots { get; set; }
        public byte? ShotsOnGoal { get; set; }
        public byte? Goals { get; set; }
        public byte? Assists { get; set; }
        public byte? Offsides { get; set; }
        public byte? FoulsDrawn { get; set; }
        public byte? FoulsCommitted { get; set; }
        public byte? Saves { get; set; }
        public byte? YellowCards { get; set; }
        public byte? RedCards { get; set; }
        public byte? PenaltiesScored { get; set; }
        public byte? PenaltiesMissed { get; set; }
        public int APIId { get; set; }
        public byte Rating { get; set; }
    }
}