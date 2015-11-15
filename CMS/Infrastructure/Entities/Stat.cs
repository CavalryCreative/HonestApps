using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Stat : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public byte? HomeTeamTotalShots { get; set; }
        public byte? HomeTeamOnGoalShots { get; set; }
        public byte? HomeTeamFouls { get; set; }
        public byte? HomeTeamCorners { get; set; }
        public byte? HomeTeamOffsides { get; set; }
        public byte? HomeTeamPossessionTime { get; set; }
        public byte? HomeTeamYellowCards { get; set; }
        public byte? HomeTeamRedCards { get; set; }
        public byte? HomeTeamSaves { get; set; }
        public byte? AwayTeamTotalShots { get; set; }
        public byte? AwayTeamOnGoalShots { get; set; }
        public byte? AwayTeamFouls { get; set; }
        public byte? AwayTeamCorners { get; set; }
        public byte? AwayTeamOffsides { get; set; }
        public byte? AwayTeamPossessionTime { get; set; }
        public byte? AwayTeamYellowCards { get; set; }
        public byte? AwayTeamRedCards { get; set; }
        public byte? AwayTeamSaves { get; set; }
    }
}