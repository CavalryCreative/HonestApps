using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Player : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<PlayerStat> PlayerStats { get; set; }

        public Player()
        {
            Teams = new HashSet<Team>();
            PlayerStats = new HashSet<PlayerStat>();
        }
    }
}