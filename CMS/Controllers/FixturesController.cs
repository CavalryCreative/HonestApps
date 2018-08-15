using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Concrete;
using System;

namespace CMS.Controllers
{
    public class FixturesController : ApiController
    {
        public DateTime StartDate { get; set; }

        // GET api/<controller>
        public IHttpActionResult Get()
        {
            Feed feed = new Feed();
            IList<Match> fixtures = new List<Match>();

            StartDate = new DateTime(2018, 8, 1);

            using (EFDbContext context = new EFDbContext())
            {
                foreach (var match in context.Matches.Where(x => x.Date > StartDate).OrderBy(x => x.Date))
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

            return Json(feed);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            Feed feed = new Feed();
            IList<Match> fixtures = new List<Match>();

            StartDate = new DateTime(2018, 8, 1);

            using (EFDbContext context = new EFDbContext())
            {
                foreach (var match in context.Matches.Where(x => (x.AwayTeamAPIId == id || x.HomeTeamAPIId == id) && x.Date > StartDate).OrderBy(x => x.Date))
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

                if (id == fixture.HomeTeamAPIId)
                {
                    feedFixture.TeamToPlay = awayTeam.Name;
                }
                else
                {
                    feedFixture.TeamToPlay = homeTeam.Name;
                }

                feedFixture.APIId = fixture.APIId;
                feedFixture.HomeTeamAPIId = homeTeam.APIId;
                feedFixture.HomeTeam = homeTeam.Name;
                feedFixture.AwayTeamAPIId = awayTeam.APIId;
                feedFixture.AwayTeam = awayTeam.Name;
                feedFixture.MatchDate = fixture.Date.Value.ToString("MMMM dd");
                feedFixture.FullTimeScore = fixture.FullTimeScore;

                feedFixtures.Add(feedFixture);
            }

            feed.Fixtures = feedFixtures;

            return Json(feed);
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