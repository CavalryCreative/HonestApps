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

        protected void Application_End(object sender, EventArgs e)
        {
            //HangfireBootstrapper.Instance.Stop();
        }
    }
}
