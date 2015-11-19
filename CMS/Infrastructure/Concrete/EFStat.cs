using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFStat : IStat 
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Stat> Get(Guid? matchId)
        {
            IList<Stat> stats = new List<Stat>();

            if (matchId.HasValue)
            {
                stats = context.Stats.Where(x => (x.MatchId == matchId)).ToList();
            }
            else
            {
                if (matchId != System.Guid.Empty)
                {
                    stats = context.Stats.ToList();
                }
            }

            return stats;
        }

        public string Save(Stat updatedRecord)
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

                    context.Stats.Add(updatedRecord);
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

        public string Delete(Stat updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}