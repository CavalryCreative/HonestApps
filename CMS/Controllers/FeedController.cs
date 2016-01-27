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
        // GET: api/Feed
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Feed/5
        public string Get(int id)
        {
            //user teamid
            return "value";
        }

        // POST: api/Feed
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Feed/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Feed/5
        public void Delete(int id)
        {
        }

        private string GetComment(int teamId, int matchId)
        {
            return "";
        }
    }
}
