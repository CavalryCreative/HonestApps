using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Abstract;

namespace CMS.Controllers
{
    public class FeedController : ApiController
    {
        
        // GET: api/Feed/5
        public string Get(int id)
        {
            FeedUpdate update = FeedUpdate.Instance;

            return update.GetMatchFeed(id);
        }
    }
}
