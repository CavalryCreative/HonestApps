using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Concrete;

namespace CMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private FeedUpdate feedUpdate;

        protected void Application_Start()
        {
            feedUpdate = FeedUpdate.Instance;

            //RouteTable.Routes.MapHubs(new HubConfiguration { EnableCrossDomain = true });

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //HangfireBootstrapper.Instance.Start();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            EFSiteException efSiteException = new EFSiteException();
            SiteException siteException = new SiteException();

            siteException.HResult = exception.HResult.ToString();

            if (exception.InnerException != null)
            {
                siteException.InnerException = exception.InnerException.ToString();
            }

            siteException.Message = exception.Message;
            siteException.Source = exception.Source;
            siteException.TargetSite = exception.TargetSite.ToString();

            efSiteException.Save(siteException);

            HttpException httpException = exception as HttpException;

            Server.ClearError();

            //if (httpException != null)
            //{
            //    string action;

            //    switch (httpException.GetHttpCode())
            //    {
            //        case 404:
            //            // page not found
            //            action = "HttpError404";
            //            break;
            //        case 500:
            //            // server error
            //            action = "HttpError500";
            //            break;
            //        default:
            //            action = "General";
            //            break;
            //    }

            //    // clear error on server
            //    Server.ClearError();

            //    //Response.Redirect(String.Format("~/Error/{0}", action));
            //}
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //HangfireBootstrapper.Instance.Stop();
        }
    }
}
