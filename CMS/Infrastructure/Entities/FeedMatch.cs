using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedMatch
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public Event LatestEvent { get; set; }
        public IList<FeedLineup> HomeLineUp { get; set; }
        public IList<FeedLineup> AwayLineUp { get; set; }
    }
}