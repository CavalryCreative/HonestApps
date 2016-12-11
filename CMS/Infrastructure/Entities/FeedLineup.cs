using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedLineup
    {
        public Guid PlayerId { get; set; }
        public string PlayerSurname { get; set; }
        public bool IsHomePlayer { get; set; }
        public bool IsSub { get; set; }
        public bool Substituted { get; set; }
        public string SubTime { get; set; }
        public string Position { get; set; }
        public string Number { get; set; }
    }
}