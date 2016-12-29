using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedFixture
    {
        public int APIId { get; set; }
        public int HomeTeamAPIId { get; set; }
        public int AwayTeamAPIId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string MatchDate { get; set; }
        public string KickOff { get; set; }
        public string FullTimeScore { get; set; }
    }
}