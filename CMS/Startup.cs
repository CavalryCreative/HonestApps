using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System.Collections.Generic;
using CMS.Controllers;

[assembly: OwinStartupAttribute(typeof(CMS.Startup))]
namespace CMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application using OWIN startup, visit http://go.microsoft.com/fwlink/?LinkID=316888

            ConfigureSignalR(app);
            ConfigureAuth(app);          

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });

            HomeController home = new HomeController();
            //RecurringJob.AddOrUpdate(() => home.GetFixtures(), Cron.Daily(9));
            //GetCommentaries
            RecurringJob.AddOrUpdate(() => home.GetCommentaries(), Cron.Daily(9));
        }

        public static void ConfigureSignalR(IAppBuilder app)
        {          
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr" path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }

    public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return context.Authentication.User.Identity.IsAuthenticated;
        }
    }
}
