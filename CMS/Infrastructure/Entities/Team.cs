﻿using System;
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

        public virtual ICollection<Match> Matches { get; set; }
        public virtual ICollection<Rival> Rivals { get; set; }
        public virtual ICollection<Lineup> Lineups { get; set; }
        public virtual ICollection<Player> Players { get; set; }

        public Team()
        {
            Matches = new HashSet<Match>();
            Rivals = new HashSet<Rival>();
            Lineups = new HashSet<Lineup>();
            Players = new HashSet<Player>();
        }
    }
}