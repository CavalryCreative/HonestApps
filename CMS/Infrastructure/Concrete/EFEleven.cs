using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFEleven : IEleven
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Eleven> Get(Guid? matchId, bool IsHomeTeam)
        {
            IList<Eleven> firstEleven = new List<Eleven>();

            if (matchId.HasValue)
            {
                firstEleven = context.Lineups.OfType<Eleven>().Where(x => (x.MatchId == matchId)).ToList();
            }
            else
            {
                if (matchId != System.Guid.Empty)
                {
                    firstEleven = context.Lineups.OfType<Eleven>().ToList();
                }
            }

            return firstEleven;
        }

        public string Save(Eleven updatedRecord)
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

                    context.Lineups.Add(updatedRecord);
                    Id = updatedRecord.Id;

                    isNewRecord = true;
                }
                else
                {
                    //Update record
                    var recordToUpdate = context.Lineups.Find(updatedRecord.Id);

                    if (recordToUpdate == null)
                    {
                        return Res.Resources.NotFound;
                    }

                    recordToUpdate.JobRef = updatedRecord.JobRef;
                    recordToUpdate.Title = updatedRecord.Title;
                    recordToUpdate.URL = updatedRecord.URL;
                    recordToUpdate.Summary = updatedRecord.Summary;
                    recordToUpdate.Description = updatedRecord.Description;
                    recordToUpdate.Keywords = updatedRecord.Keywords;
                    recordToUpdate.MetaDescription = updatedRecord.MetaDescription;
                    recordToUpdate.MainContact = updatedRecord.MainContact;
                    recordToUpdate.Discipline = updatedRecord.Discipline;
                    recordToUpdate.JobTitle = updatedRecord.JobTitle;
                    recordToUpdate.Region = updatedRecord.Region;
                    recordToUpdate.Country = updatedRecord.Country;
                    recordToUpdate.SubRegion = updatedRecord.SubRegion;
                    recordToUpdate.Location = updatedRecord.Location;
                    recordToUpdate.RemoteWorking = updatedRecord.RemoteWorking;
                    recordToUpdate.HourRateFrom = updatedRecord.HourRateFrom;
                    recordToUpdate.HourRateTo = updatedRecord.HourRateTo;
                    recordToUpdate.SalaryFrom = updatedRecord.SalaryFrom;
                    recordToUpdate.SalaryTo = updatedRecord.SalaryTo;
                    recordToUpdate.SalaryCurrency = updatedRecord.SalaryCurrency;
                    recordToUpdate.Negotiable = updatedRecord.Negotiable;
                    recordToUpdate.Permanent = updatedRecord.Permanent;
                    recordToUpdate.Contract = updatedRecord.Contract;
                    recordToUpdate.ActiveDate = updatedRecord.ActiveDate;
                    recordToUpdate.ClosingDate = updatedRecord.ClosingDate;
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

        public string Delete(Eleven updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}