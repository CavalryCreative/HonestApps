﻿using System;
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
                firstEleven = context.Lineups.Include("HomeTeam").Include("AwayTeam").OfType<Eleven>().Where(x => (x.MatchId == matchId)).ToList();
            }
            else
            {
                if (matchId != System.Guid.Empty)
                {
                    firstEleven = context.Lineups.Include("HomeTeam").Include("AwayTeam").OfType<Eleven>().ToList();
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

                    recordToUpdate.DateUpdated = DateTime.Now;
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