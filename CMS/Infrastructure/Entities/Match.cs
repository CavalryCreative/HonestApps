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
    public class Match : CMSBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Stat> Stats { get; set; }

        public Match()
        {
            Teams = new HashSet<Team>();
            Events = new HashSet<Event>();
            Stats = new HashSet<Stat>();
        }
    }
}