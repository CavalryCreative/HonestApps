//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FeedData
{
    using System;
    using System.Collections.Generic;
    
    public partial class Match
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Match()
        {
            this.Events = new HashSet<Event>();
            this.Lineups = new HashSet<Lineup>();
            this.PlayerStats = new HashSet<PlayerStat>();
            this.Stats = new HashSet<Stat>();
            this.Substitutions = new HashSet<Substitution>();
            this.Summaries = new HashSet<Summary>();
        }
    
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.Guid> UpdatedByUserId { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public string Stadium { get; set; }
        public string Attendance { get; set; }
        public string Time { get; set; }
        public string Referee { get; set; }
        public int APIId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.Guid> HomeTeamId { get; set; }
        public Nullable<System.Guid> AwayTeamId { get; set; }
        public int HomeTeamAPIId { get; set; }
        public int AwayTeamAPIId { get; set; }
        public bool IsToday { get; set; }
        public bool IsLive { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> Events { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lineup> Lineups { get; set; }
        public virtual Team Team { get; set; }
        public virtual Team Team1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerStat> PlayerStats { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stat> Stats { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Substitution> Substitutions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Summary> Summaries { get; set; }
    }
}
