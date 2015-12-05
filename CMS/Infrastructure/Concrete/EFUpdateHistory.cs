using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFUpdateHistory : IUpdateHistory
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<UpdateHistory> Get(int matchId)
        {
            return context.UpdateHistory.Where(x => x.MatchAPIId == matchId).ToList();
        }

        public string Save(UpdateHistory updatedRecord)
        {
            bool isNewRecord = false;
            Guid Id;

            if (updatedRecord != null)
            {
                if (updatedRecord.Id == System.Guid.Empty)
                {
                    //Create record  
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    context.UpdateHistory.Add(updatedRecord);
                    Id = updatedRecord.Id;

                    isNewRecord = true;
                }
                else
                {
                    //Update record
                    var recordToUpdate = context.UpdateHistory.Find(updatedRecord.Id);

                    if (recordToUpdate == null)
                    {
                        return Res.Resources.NotFound;
                    }

                    recordToUpdate.Deleted = updatedRecord.Deleted;
                    recordToUpdate.DateUpdated = DateTime.Now;
                    recordToUpdate.UpdatedByUserId = updatedRecord.UpdatedByUserId;
                    recordToUpdate.Active = updatedRecord.Active;

                    context.Entry(recordToUpdate).State = System.Data.Entity.EntityState.Modified;
                    Id = updatedRecord.Id;
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
    }
}