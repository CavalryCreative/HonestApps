﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
    public class Event : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public bool Important { get; set; }
        public bool Goal { get; set; }
        public byte Minute { get; set; }
        public string Score { get; set; }
        public string Comment { get; set; }
        public int APIId { get; set; }
        public byte HomeTeamMatchRating { get; set; }
        public byte AwayTeamMatchRating { get; set; }

        //Properties used in feed
        [NotMapped]
        public string EventComment { get; set; }
        [NotMapped]
        public string HomeComment { get; set; }
        [NotMapped]
        public int HomeTeamAPIId { get; set; }
        [NotMapped]
        public string AwayComment { get; set; }
        [NotMapped]
        public int AwayTeamAPIId { get; set; }
        [NotMapped]
        public int MatchAPIId { get; set; }
        [NotMapped]
        public int EventAPIId { get; set; }
    }
}