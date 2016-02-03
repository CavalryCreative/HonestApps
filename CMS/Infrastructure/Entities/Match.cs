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

        //public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Stat> Stats { get; set; }
        public virtual ICollection<Lineup> Lineups { get; set; }
        public virtual ICollection<Summary> Summaries { get; set; }
        public virtual ICollection<Substitution> Substitutions { get; set; }
        public virtual ICollection<PlayerStat> PlayerStats { get; set; }

        public int APIId { get; set; }
        public string Stadium { get; set; }
        public string Attendance { get; set; }
        public string Time { get; set; }
        public string Referee { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }
        public int HomeTeamAPIId { get; set; }
        public int AwayTeamAPIId { get; set; }
        public bool IsToday { get; set; }
        public bool IsLive { get; set; }
       

        public Match()
        {
            //Teams = new HashSet<Team>();
            Events = new HashSet<Event>();
            Stats = new HashSet<Stat>();
            Lineups = new HashSet<Lineup>();
            Summaries = new HashSet<Summary>();
            Substitutions = new HashSet<Substitution>();
            PlayerStats = new HashSet<PlayerStat>();
        }
    }
}