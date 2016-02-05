using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Lineup : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int MatchAPIId { get; set; }
        public Guid PlayerId { get; set; }
        public bool IsHomePlayer { get; set; }
        public bool IsSub { get; set; }
    }
}