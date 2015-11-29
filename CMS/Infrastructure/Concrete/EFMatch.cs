using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Res = Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFMatch : IMatch
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Match> Get(Guid? matchId)
        {
            IList<Match> match = new List<Match>();

            if (matchId.HasValue)
            {
                match = context.Matches.Where(x => (x.Id == matchId)).ToList();
            }
            else
            {
                if (matchId != System.Guid.Empty)
                {
                    match = context.Matches.ToList();
                }
            }

            return match;
        }

        public IEnumerable<Match> GetByAPIId(int id)
        {
            IList<Match> match = new List<Match>();
         
            match = context.Matches.Where(x => (x.APIId == id)).ToList();
          
            return match;
        }

        public string Save(Match updatedRecord)
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

                    context.Matches.Add(updatedRecord);
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

        public string SaveTeam(Match updatedRecord, int teamAPIId)
        {
            throw new NotImplementedException();

            //Guid Id = Guid.Empty;

            //Team teamToAdd = new Team();

            //if (updatedRecord != null)
            //{
            //    var team = context.Matches.Where(x => x.APIId == teamAPIId).FirstOrDefault();

            //    if (team == null)
            //    {
            //        return Res.Resources.NotFound;
            //    }

            //    if (updatedRecord.Id == System.Guid.Empty)
            //    {
            //        //Create record
            //        updatedRecord.Id = Guid.NewGuid();
            //        updatedRecord.Active = true;
            //        updatedRecord.Deleted = false;
            //        updatedRecord.DateAdded = DateTime.Now;
            //        updatedRecord.DateUpdated = DateTime.Now;

            //        updatedRecord.Teams.Add(teamToAdd);
            //        Id = updatedRecord.Id;
            //    }
            //}

            //try
            //{
            //    context.SaveChanges();

            //    return string.Format("{0}:{1}", Res.Resources.RecordAdded, Id.ToString());
            //}
            //catch (Exception e)
            //{
            //    return string.Format("Error: {0}", e.InnerException.ToString());
            //}
        }
    }
}