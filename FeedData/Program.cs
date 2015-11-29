using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Res = Resources;

namespace FeedData
{
    //Fixtures
    //http://football-api.com/api/?Action=fixtures&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204&from_date=17.10.2015&to_date=27.10.2015

    //Today live match events - brings back same feed as above
    //http://football-api.com/api/?Action=today&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204

    //Commentaries
    //http://football-api.com/api/?Action=commentaries&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&match_id=2136144

    //Standings
    //http://football-api.com/api/?Action=standings&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204

    class Program
    {
        static void Main(string[] args)
        {
            IList<int> liveMatches = new List<int>();

            liveMatches = GetFixtures(DateTime.Now, DateTime.Now.AddDays(7));

          if (liveMatches.Count > 0)
          {
              foreach (var matchId in liveMatches)
              {
                  GetCommentaries(matchId);
              }
          }         
        }

        private static IList<int> GetFixtures(DateTime startDate, DateTime endDate)
        {
            IList<int> liveMatches = new List<int>();
            string retMsg = string.Empty;

            string startDateStr = startDate.ToShortDateString();
            string endDateStr = endDate.ToShortDateString();

            string[] startArr = startDateStr.Split('/');
            string[] endArr = endDateStr.Split('/');

            string formatStart = string.Format("{0}.{1}.{2}", startArr[0], startArr[1], startArr[2]);
            string formatEnd = string.Format("{0}.{1}.{2}", endArr[0], endArr[1], endArr[2]);

            try
            {
                string uri = string.Format("http://football-api.com/api/?Action=fixtures&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204&from_date={0}&to_date={1}", formatStart, formatEnd);  // <-- this returns formatted json

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    JObject obj = JObject.Parse(s);

                    var postTitles = from p in obj["matches"] 
                                     select new
                                         {
                                             MatchDate = (string)p["match_formatted_date"],
                                             APIId = (int)p["match_id"],
                                             Time = (string)p["match_time"],
                                             HomeTeamAPIId = (int)p["match_localteam_id"],
                                             HomeTeam = (string)p["match_localteam_name"],
                                             AwayTeamAPIId = (int)p["match_visitorteam_id"],
                                             AwayTeam = (string)p["match_visitorteam_name"]
                                         };

                    Entities context = new Entities();

                     foreach (var item in postTitles)
                     {
                         Match match = new Match();

                         match.APIId = item.APIId;
                         match.Date = Convert.ToDateTime(string.Format("{0} {1}", item.MatchDate.Replace('.', '/'), match.Time));
                         match.EndDate = match.Date.Value.AddHours(2);      
                         match.Active = DateTime.Now <= match.EndDate ? true : false;                                                                        
                         match.IsToday = DateTime.Now.ToShortDateString() == match.Date.Value.ToShortDateString() ? true : false;
                         match.IsLive = DateTime.Now >= match.Date && DateTime.Now <= match.EndDate ? true : false;
                         match.Time = item.Time;

                         if (match.IsLive)
                             liveMatches.Add(match.APIId);

                         var homeTeam = GetTeamByAPIId(item.HomeTeamAPIId);
                         var awayTeam = GetTeamByAPIId(item.AwayTeamAPIId);

                         if (homeTeam != null)
                         {
                             match.HomeTeamId = homeTeam.Id;
                             match.Stadium = homeTeam.Stadium;
                         }
                            
                         if (awayTeam != null)
                             match.AwayTeamId = awayTeam.Id;

                         match.HomeTeamAPIId = item.HomeTeamAPIId;
                         match.AwayTeamAPIId = item.AwayTeamAPIId;

                         SaveMatch(match);
                     }
                }
                else
                {
                    retMsg = "Error: " + string.Format("Status code == {0}, Content length == {1}", webResponse.StatusCode, webResponse.ContentLength);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                retMsg = "Error: " + ex.Message;
            }

            return liveMatches;
        }

        private static string GetCommentaries(int matchId)
        {
            //const int POINT_OF_SATISFIED_CURIOSITY = 7;
            string retMsg = string.Empty;
            
            //System.Diagnostics.Debugger.Break();

            //Fixtures
            //http://football-api.com/api/?Action=fixtures&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204&from_date=17.10.2015&to_date=27.10.2015

            //Today live match events - brings back same feed as above
            //http://football-api.com/api/?Action=today&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204

            //Commentaries
            //http://football-api.com/api/?Action=commentaries&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&match_id=2136144

            //Standings
            //http://football-api.com/api/?Action=standings&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204

            try
            {
                string uri = "http://football-api.com/api/?Action=commentaries&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&match_id=" + matchId.ToString();  // <-- this returns formatted json

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    JToken token = JObject.Parse(s);

                    string jPath = "commentaries.[0].comm_commentaries.comment";
                    int lastUpdateId = 8961117;//TODO - save/retrieve lastupdateId

                    var events = token.SelectTokens(jPath);
                      
                    foreach (var childToken in events.Children())
                    {
                        var jeff = childToken.Children();

                        foreach(var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                        {
                            string id = evt["id"];
                            int eventId = Convert.ToInt32(id);

                            if (eventId <= lastUpdateId)
                            {
                                break;
                            }

                            Event commEvent = new Event();

                            string important = evt["important"];
                            string isgoal = evt["isgoal"];
                            string minute = evt["minute"];

                            if (minute.Contains('\''))
                            {
                                minute = minute.Remove(minute.Length - 1, 1);
                            }

                            string comment = evt["comment"];
                          
                            commEvent.Important = important == "True" ? true : false;
                            commEvent.Goal = isgoal == "True" ? true : false;

                            if (!string.IsNullOrWhiteSpace(minute))
                                commEvent.Minute = Convert.ToByte(minute);

                            commEvent.Comment = comment;
                            commEvent.APIId = eventId;
                            //commEvent.MatchId = matchId;

                            SaveEvent(commEvent);
                        }
                    }
                }
                else
                {
                    retMsg = "Error: " + string.Format("Status code == {0}, Content length == {1}", webResponse.StatusCode, webResponse.ContentLength);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                retMsg = "Error: " + ex.Message;
            }

            return retMsg;
        }

        private static string SaveMatch(Match updatedRecord)
        {
            Guid Id = Guid.Empty;

            Entities context = new Entities();

            if (updatedRecord == null)
            {
                return Res.Resources.NotFound;
            }
          
                //Update record
                var recordToUpdate = context.Matches.Where(x => x.APIId == updatedRecord.APIId).FirstOrDefault();

                if (recordToUpdate == null)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    context.Matches.Add(updatedRecord);
                    Id = updatedRecord.Id;
                }
                else
                {
                    recordToUpdate.Active = updatedRecord.Active;
                    recordToUpdate.APIId = updatedRecord.APIId;
                    recordToUpdate.Attendance = updatedRecord.Attendance;
                    recordToUpdate.AwayTeamAPIId = updatedRecord.AwayTeamAPIId;
                    recordToUpdate.AwayTeamId = updatedRecord.AwayTeamId;
                    recordToUpdate.Date = updatedRecord.Date;
                    recordToUpdate.EndDate = updatedRecord.EndDate;
                    recordToUpdate.HomeTeamAPIId = updatedRecord.HomeTeamAPIId;
                    recordToUpdate.HomeTeamId = updatedRecord.HomeTeamId;
                    recordToUpdate.DateUpdated = DateTime.Now;
                    recordToUpdate.IsLive = updatedRecord.IsLive;
                    recordToUpdate.IsToday = updatedRecord.IsToday;
                    recordToUpdate.Referee = updatedRecord.Referee;
                    recordToUpdate.Stadium = updatedRecord.Stadium;
                    recordToUpdate.Time = updatedRecord.Time;

                    context.Entry(recordToUpdate).State = System.Data.Entity.EntityState.Modified;
                    Id = updatedRecord.Id;
                }

            try
            {
                context.SaveChanges();

                return string.Format("{0}:{1}", Res.Resources.RecordAdded, Id.ToString());
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        private static string SaveEvent(Event updatedRecord)
        {
            Guid Id = Guid.Empty;

            Entities context = new Entities();

            if (updatedRecord != null)
            {
                if (updatedRecord.Id == System.Guid.Empty)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    context.Events.Add(updatedRecord);
                    Id = updatedRecord.Id;
                }
            }

            try
            {
                context.SaveChanges();

                return string.Format("{0}:{1}", Res.Resources.RecordAdded, Id.ToString());
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        private static Team GetTeamByAPIId(int id)
        {
            Entities context = new Entities();

            return context.Teams.Where(x => (x.APIId == id)).FirstOrDefault();
        }
    }
}
