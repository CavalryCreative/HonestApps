using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;
using Resources;

namespace CMS.Infrastructure.Concrete
{
    public class EFSiteException
    {
        private EFDbContext context = new EFDbContext();

        public string Save(SiteException exception)
        {
            bool isNewRecord = false;

            SiteException record = new SiteException();

            if (exception != null)
            {
                //Create record
                record.HResult = exception.HResult;
                record.InnerException = exception.InnerException;
                record.Message = exception.Message;
                record.Source = exception.Source;
                record.StackTrace = exception.StackTrace;
                record.TargetSite = exception.TargetSite;
                record.DateAdded = DateTime.Now;

                context.SiteException.Add(record);

                isNewRecord = true;
            }
            else
            {
                return Resources.Resources.NotFound;
            }

            try
            {
                context.SaveChanges();

                return isNewRecord ? string.Format("{0}:{1}", Resources.Resources.RecordAdded, record.Id.ToString())
                    : string.Format("{0}", Resources.Resources.RecordUpdated);
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
    }
}
}