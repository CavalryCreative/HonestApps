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
    
    public partial class Event
    {
        public System.Guid Id { get; set; }
        public System.Guid MatchId { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.Guid> UpdatedByUserId { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public bool Important { get; set; }
        public bool Goal { get; set; }
        public byte Minute { get; set; }
        public string Comment { get; set; }
        public int APIId { get; set; }
        public string Score { get; set; }
        public byte HomeTeamMatchRating { get; set; }
        public byte AwayTeamMatchRating { get; set; }
    
        public virtual Match Match { get; set; }
    }
}
