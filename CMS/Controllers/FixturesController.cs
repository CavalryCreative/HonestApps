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
    public class FixturesController : ApiController
    {
        // GET api/<controller>
        public string Get()
        {
            Feed feed = new Feed();
            IList<Match> fixtures = new List<Match>();

            using (EFDbContext context = new EFDbContext())
            {
                foreach (var match in context.Matches)
                {
                    fixtures.Add(match);
                }
            }

            IList<FeedFixture> feedFixtures = new List<FeedFixture>();

            foreach (var fixture in fixtures)
            {
                FeedFixture feedFixture = new FeedFixture();

                var homeTeam = GetTeamByAPIId(fixture.HomeTeamAPIId);
                var awayTeam = GetTeamByAPIId(fixture.AwayTeamAPIId);

                feedFixture.APIId = fixture.APIId;
                feedFixture.HomeTeamAPIId = homeTeam.APIId;
                feedFixture.HomeTeam = homeTeam.Name;
                feedFixture.AwayTeamAPIId = awayTeam.APIId;
                feedFixture.AwayTeam = awayTeam.Name;
                feedFixture.MatchDate = fixture.Date.Value.ToString("D");

                feedFixtures.Add(feedFixture);
            }

            feed.Fixtures = feedFixtures;

            return JsonConvert.SerializeObject(feed);
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            Feed feed = new Feed();
            IList<Match> fixtures = new List<Match>();

            using (EFDbContext context = new EFDbContext())
            {
                foreach (var match in context.Matches.Where(x => x.AwayTeamAPIId == id || x.HomeTeamAPIId == id).OrderBy(x => x.Date))
                {
                    fixtures.Add(match);
                }
            }

            IList<FeedFixture> feedFixtures = new List<FeedFixture>();

            foreach (var fixture in fixtures)
            {
                FeedFixture feedFixture = new FeedFixture();

                var homeTeam = GetTeamByAPIId(fixture.HomeTeamAPIId);
                var awayTeam = GetTeamByAPIId(fixture.AwayTeamAPIId);

                feedFixture.APIId = fixture.APIId;
                feedFixture.HomeTeamAPIId = homeTeam.APIId;
                feedFixture.HomeTeam = homeTeam.Name;
                feedFixture.AwayTeamAPIId = awayTeam.APIId;
                feedFixture.AwayTeam = awayTeam.Name;
                feedFixture.MatchDate = fixture.Date.Value.ToString("D");
                feedFixture.FullTimeScore = fixture.FullTimeScore;

                feedFixtures.Add(feedFixture);
            }

            feed.Fixtures = feedFixtures;

            return JsonConvert.SerializeObject(feed);
        }

        private static Team GetTeamByAPIId(int id)
        {
            Team team = new Team();

            using (EFDbContext context = new EFDbContext())
            {
                team = context.Teams.Where(x => (x.APIId == id)).FirstOrDefault();
            }

            return team;
        }
    }
}