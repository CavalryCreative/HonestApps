using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedEvent
    {
        public string EventComment { get; set; }
        public string HomeComment { get; set; }
        public int HomeTeamAPIId { get; set; }
        public string AwayComment { get; set; }
        public int AwayTeamAPIId { get; set; }
        public string Score { get; set; }
        public byte Minute { get; set; }
        public int MatchAPIId { get; set; }
        public int EventAPIId { get; set; }
    }
}