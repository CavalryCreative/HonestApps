using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFLineup : ILineup
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Lineup> Get(int matchId, bool isHomePlayer, bool isSub)
        {
            return context.Lineups.Where(x => (x.MatchAPIId == matchId) && (x.IsHomePlayer == isHomePlayer) && (x.IsSub == isSub)).ToList();
        }

        public string Save(Lineup updatedRecord)
        {
            bool isNewRecord = false;
            Guid Id = System.Guid.Empty;

            if (updatedRecord != null)
            {
                if (updatedRecord.Id == System.Guid.Empty)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    context.Lineups.Add(updatedRecord);
                    Id = updatedRecord.Id;

                    isNewRecord = true;
                }
            }
            else
            {
                return Res.Resources.NotFound;
            }

            try
            {
                context.SaveChanges();

                return isNewRecord ? string.Format("{0}:{1}", Res.Resources.RecordAdded, Id.ToString())
                    : string.Format("{0}:{1}", Res.Resources.RecordUpdated, Id.ToString());
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        public string Delete(Lineup updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}