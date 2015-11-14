using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Summary : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid HomeTeam { get; set; }
        public Guid AwayTeam { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<YellowCard> YellowCards { get; set; }
        public virtual ICollection<RedCard> RedCards { get; set; }

        public Summary()
        {
            Goals = new HashSet<Goal>();
            YellowCards = new HashSet<YellowCard>();
            RedCards = new HashSet<RedCard>();
        }
    }
}