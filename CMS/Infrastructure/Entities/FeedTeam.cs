using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedTeam
    {
        public int APIId { get; set; }
        public string Name { get; set; }
        public string Stadium { get; set; }
        public string PrimaryColour { get; set; }
        public string SecondaryColour { get; set; }
    }
}