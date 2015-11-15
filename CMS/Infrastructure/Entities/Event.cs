using System;
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
        public string Comment { get; set; }
        public int APIId { get; set; }
    }
}