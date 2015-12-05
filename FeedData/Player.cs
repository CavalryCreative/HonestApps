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
    
    public partial class Player
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Player()
        {
            this.PlayerStats = new HashSet<PlayerStat>();
            this.Teams = new HashSet<Team>();
            this.Lineups = new HashSet<Lineup>();
        }
    
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.Guid> UpdatedByUserId { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public byte SquadNumber { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int APIPlayerId { get; set; }
        public Nullable<System.Guid> Eleven_Id { get; set; }
        public Nullable<System.Guid> Eleven_Id1 { get; set; }
        public Nullable<System.Guid> Sub_Id { get; set; }
        public Nullable<System.Guid> Sub_Id1 { get; set; }
    
        public virtual Lineup Lineup { get; set; }
        public virtual Lineup Lineup1 { get; set; }
        public virtual Lineup Lineup2 { get; set; }
        public virtual Lineup Lineup3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerStat> PlayerStats { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Teams { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lineup> Lineups { get; set; }
    }
}