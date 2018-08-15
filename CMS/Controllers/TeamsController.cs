using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Concrete;

namespace CMS.Controllers
{
    public class TeamsController : ApiController
    {
        public IHttpActionResult Get()
        {
            Feed feed = new Feed();
            IList<Team> teams = new List<Team>();

            using (EFDbContext context = new EFDbContext())
            {
                foreach (var team in context.Teams.Where(x => x.Active == true))
                {
                    teams.Add(team);
                }
            }

            IList<FeedTeam> feedTeams = new List<FeedTeam>();

            foreach (var team in teams)
            {
                FeedTeam feedTeam = new FeedTeam();

                feedTeam.APIId = team.APIId;
                feedTeam.Name = team.Name;
                feedTeam.PrimaryColour = team.PrimaryColour;
                feedTeam.SecondaryColour = team.SecondaryColour;
                feedTeam.Stadium = team.Stadium;

                feedTeams.Add(feedTeam);
            }

            feed.Teams = feedTeams;

            return Json(feed);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            Feed feed = new Feed();
            IList<Team> teams = new List<Team>();

            using (EFDbContext context = new EFDbContext())
            {
                foreach (var team in context.Teams.Where(x => x.APIId == id && x.Active == true))
                {
                    teams.Add(team);
                }
            }

            IList<FeedTeam> feedTeams = new List<FeedTeam>();

            foreach (var team in teams)
            {
                FeedTeam feedTeam = new FeedTeam();

                feedTeam.APIId = team.APIId;
                feedTeam.Name = team.Name;
                feedTeam.PrimaryColour = team.PrimaryColour;
                feedTeam.SecondaryColour = team.SecondaryColour;
                feedTeam.Stadium = team.Stadium;

                feedTeams.Add(feedTeam);
            }

            feed.Teams = feedTeams;

            return Json(feed);
        }
    }
}