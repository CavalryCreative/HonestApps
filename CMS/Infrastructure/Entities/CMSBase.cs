using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Infrastructure.Entities
{
    public abstract class CMSBase
    {
        public Guid? CreatedByUserId { get; set; }
        public DateTime? DateAdded { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}