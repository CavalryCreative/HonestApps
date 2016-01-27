using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Infrastructure.Entities
{
   [NotMapped]
    public class Feed
    {
        public string EventComment { get; set; }
        public string Comment { get; set; }
        public string Score { get; set; }
        public byte Minute { get; set; }
        public int TeamAPIId { get; set; }
    }
}