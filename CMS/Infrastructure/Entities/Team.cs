using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Entities
{
    public class Team : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int APIId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Match> HomeMatches { get; set; }
        public virtual ICollection<Match> AwayMatches { get; set; }
        public virtual ICollection<Rival> Rivals { get; set; }
        public virtual ICollection<Lineup> Lineups { get; set; }
        public virtual ICollection<Player> Players { get; set; }

        public Team()
        {
            HomeMatches = new HashSet<Match>();
            AwayMatches = new HashSet<Match>();
            Rivals = new HashSet<Rival>();
            Lineups = new HashSet<Lineup>();
            Players = new HashSet<Player>();
        }
    }
}