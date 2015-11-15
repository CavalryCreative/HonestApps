using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace CMS.Infrastructure.Entities
{
    public class Sub : Lineup
    {
        public virtual ICollection<Player> HomeTeam { get; set; }
        public virtual ICollection<Player> AwayTeam { get; set; }

        public Sub()
        {
            HomeTeam = new HashSet<Player>();
            AwayTeam = new HashSet<Player>();
        }
    }
}