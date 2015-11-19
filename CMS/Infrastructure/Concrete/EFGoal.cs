using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFGoal : IGoal
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Goal> Get(Guid? summaryId, bool IsHomeTeam)
        {
            IList<Goal> goal = new List<Goal>();

            if (summaryId.HasValue)
            {
                goal = context.Goals.Where(x => (x.SummaryId == summaryId)).ToList();
            }
            else
            {
                if (summaryId != System.Guid.Empty)
                {
                    goal = context.Goals.ToList();
                }
            }

            return goal;
        }

        public string Save(Goal updatedRecord)
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

                    context.Goals.Add(updatedRecord);
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

        public string Delete(Goal updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}