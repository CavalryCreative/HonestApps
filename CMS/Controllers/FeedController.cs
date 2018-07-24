using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CMS.Infrastructure.Entities;
using Newtonsoft.Json;
using CMS.Infrastructure.Abstract;

namespace CMS.Controllers
{
    public class FeedController : ApiController
    {
        
        // GET: api/Feed/5
        public IHttpActionResult Get(int id)
        {
            FeedUpdate update = FeedUpdate.Instance;

            return Json(update.GetMatchFeed(id), DefaultJsonSettings);
        }

        private JsonSerializerSettings DefaultJsonSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
            }
        }
    }
}
