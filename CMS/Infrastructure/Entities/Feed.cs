using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
   [NotMapped]
    public class Feed
    {
        public string Updated { get; set; }
        public IList<FeedMatch> Matches { get; set; }
        public IList<FeedStanding> Standings { get; set; }
        public IList<FeedFixture> Fixtures { get; set; }
    }
}