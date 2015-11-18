using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFYellowCard : IYellowCard
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<YellowCard> Get(Guid? summaryId, bool IsHomeTeam)
        {
            IList<YellowCard> yellowCard = new List<YellowCard>();

            if (summaryId.HasValue)
            {
                yellowCard = context.Cards.OfType<YellowCard>().Where(x => (x.SummaryId == summaryId)).ToList();
            }
            else
            {
                if (summaryId != System.Guid.Empty)
                {
                    yellowCard = context.Cards.OfType<YellowCard>().ToList();
                }
            }

            return yellowCard;
        }

        public string Save(YellowCard updatedRecord)
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

                    context.Cards.Add(updatedRecord);
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

        public string Delete(YellowCard updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}