using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFSummary : ISummary
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Summary> Get(Guid? matchId)
        {
            IList<Summary> summary = new List<Summary>();

            if (matchId.HasValue)
            {
                summary = context.Summaries.Where(x => (x.MatchId == matchId)).ToList();
            }
            else
            {
                if (matchId != System.Guid.Empty)
                {
                    summary = context.Summaries.ToList();
                }
            }

            return summary;
        }

        public string Save(Summary updatedRecord)
        {
            Guid Id = Guid.Empty;

            if (updatedRecord != null)
            {
                if (updatedRecord.Id == System.Guid.Empty)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    context.Summaries.Add(updatedRecord);
                    Id = updatedRecord.Id;
                }
            }

            try
            {
                context.SaveChanges();

                return string.Format("{0}:{1}", Res.Resources.RecordAdded, Id.ToString());
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        public string Delete(Summary updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}