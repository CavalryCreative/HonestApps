﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFRedCard : IRedCard
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<RedCard> Get(Guid? summaryId, bool IsHomeTeam)
        {
            IList<RedCard> redCard = new List<RedCard>();

            if (summaryId.HasValue)
            {
                redCard = context.Cards.OfType<RedCard>().Where(x => (x.SummaryId == summaryId)).ToList();
            }
            else
            {
                if (summaryId != System.Guid.Empty)
                {
                    redCard = context.Cards.OfType<RedCard>().ToList();
                }
            }

            return redCard;
        }

        public string Save(RedCard updatedRecord)
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

        public string Delete(RedCard updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}