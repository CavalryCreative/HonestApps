using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
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
            // For more information on how to configure your application using OWIN startup, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.MapSignalR();
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
