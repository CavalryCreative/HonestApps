using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Substitution : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public string PlayerOff { get; set; }
        public Guid PlayerOffId { get; set; }
        public int APIPlayerOffId { get; set; }
        public string PlayerOn { get; set; }
        public Guid PlayerOnId { get; set; }
        public int APIPlayerOnId { get; set; }
        public byte Minute { get; set; }
        public bool IsHomeTeam { get; set; }
    }
}