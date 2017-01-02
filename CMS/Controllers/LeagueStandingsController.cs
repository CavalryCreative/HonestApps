using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Concrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMS.Controllers
{
    public class LeagueStandingsController : ApiController
    {
        // GET api/<controller>
        public Feed Get()
        {
            Feed feed = new Feed();
            IList<LeagueStanding> leagueStandings = new List<LeagueStanding>();

            using (EFDbContext context = new EFDbContext())
            {
                leagueStandings = context.LeagueStanding.OrderBy(x => x.Position).ToList();
            }

            IList<FeedStanding> feedStandings = new List<FeedStanding>();

            foreach (var team in leagueStandings)
            {
                FeedStanding leagueStanding = new FeedStanding();

                leagueStanding.Name = team.Name;
                leagueStanding.GamesPlayed = team.GamesPlayed;
                leagueStanding.GoalDifference = team.GoalDifference;
                leagueStanding.Points = team.Points;
                leagueStanding.Description = team.Description;
                leagueStanding.TeamAPI = team.APIId;

                feedStandings.Add(leagueStanding);
            }

            feed.Standings = feedStandings;

            return feed;

           // return JsonConvert.SerializeObject(feed);
        }
    }
}