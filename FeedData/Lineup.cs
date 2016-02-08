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
    
    public partial class Lineup
    {
        public System.Guid Id { get; set; }
        public int MatchAPIId { get; set; }
        public bool IsHomePlayer { get; set; }
        public bool IsSub { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.Guid> UpdatedByUserId { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public Nullable<System.Guid> Match_Id { get; set; }
        public Nullable<System.Guid> Team_Id { get; set; }
        public System.Guid PlayerId { get; set; }
    
        public virtual Match Match { get; set; }
        public virtual Team Team { get; set; }
    }
}
