using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;

namespace CMS.Infrastructure.Entities
{
    public class FeedUpdate
    {
        // Singleton instance
        private readonly static Lazy<FeedUpdate> _instance = new Lazy<FeedUpdate>(() => new FeedUpdate(GlobalHost.ConnectionManager.GetHubContext<FeedHub>().Clients));

        public static FeedUpdate Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}