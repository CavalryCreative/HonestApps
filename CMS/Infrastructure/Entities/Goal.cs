using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Goal : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int APIId { get; set; }
        public Guid SummaryId { get; set; }
        public bool IsHomeTeam { get; set; }
        public string PlayerName { get; set; }
        public int APIPlayerId { get; set; }
        public byte Minute { get; set; }
        public bool OwnGoal { get; set; }
        public bool Penalty { get; set; }
    }
}