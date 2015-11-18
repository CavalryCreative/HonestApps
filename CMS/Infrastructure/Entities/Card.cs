using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public abstract class Card : CMSBase
    {
        public Guid Id { get; set; }
        public Guid SummaryId { get; set; }
        public bool IsHomeTeam { get; set; }
        public string PlayerName { get; set; }
        public int APIPlayerId { get; set; }
        public byte Minute { get; set; }
    }
}