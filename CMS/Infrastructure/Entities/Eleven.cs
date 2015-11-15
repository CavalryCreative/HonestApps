using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Eleven : Lineup
    {
        public virtual ICollection<Player> HomeTeam { get; set; }
        public virtual ICollection<Player> AwayTeam { get; set; }

        public Eleven()
        {
            HomeTeam = new HashSet<Player>();
            AwayTeam = new HashSet<Player>();
        }
    }
}