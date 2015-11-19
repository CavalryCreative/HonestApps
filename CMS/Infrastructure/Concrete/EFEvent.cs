using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFEvent : IEvent
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Event> Get(Guid? matchId)
        {
            IList<Event> events = new List<Event>();

            if (matchId.HasValue)
            {
                events = context.Events.Where(x => (x.MatchId == matchId)).ToList();
            }
            else
            {
                if (matchId != System.Guid.Empty)
                {
                    events = context.Events.ToList();
                }
            }

            return events;
        }

        public string Save(Event updatedRecord)
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

                    context.Events.Add(updatedRecord);
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

        public string Delete(Event updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}