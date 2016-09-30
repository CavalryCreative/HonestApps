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
using Hangfire;
using System.Web;

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
            //GlobalConfiguration.Configuration.UseSqlServerStorage("HonestAppsEntities");

            //RecurringJob.AddOrUpdate(() => Fixtures(), Cron.Daily(9));

            ////GetCommentaries
            ////RecurringJob.AddOrUpdate(() => GetCommentaries(), Cron.Daily(9));

            //using (var server = new BackgroundJobServer())
            //{
            //    Console.WriteLine("Hangfire Server started. Press any key to exit...");
            //    Console.ReadKey();
            //}
           
            GetFixtures();
        }

        private static void GetFixtures()
        {
            ////Clear today matches table
            //DeleteMatchesToday();

            //IDictionary<int, string> matchesToday = new Dictionary<int, string>();

            //matchesToday = GetFixtures(DateTime.Now, DateTime.Now.AddDays(7));

            //foreach (var kvp in matchesToday)
            //{
            //    MatchesToday match = new MatchesToday();
            //    match.APIId = kvp.Key;
            //    match.KickOffTime = kvp.Value;

            //    SaveMatchToday(match);
            //}

            GetFixtures(DateTime.Now, DateTime.Now.AddDays(7));
        }

        private static IDictionary<int, string> GetFixtures(DateTime startDate, DateTime endDate)
        {
            IDictionary<int, string> matchesToday = new Dictionary<int, string>();
            string retMsg = string.Empty;

            string startDateStr = startDate.ToShortDateString();
            string endDateStr = endDate.ToShortDateString();

            string[] startArr = startDateStr.Split('/');
            string[] endArr = endDateStr.Split('/');

            string formatStart = string.Format("{0}.{1}.{2}", startArr[0], startArr[1], startArr[2]);
            string formatEnd = string.Format("{0}.{1}.{2}", endArr[0], endArr[1], endArr[2]);

            try
            {
                //string uri = string.Format("http://football-api.com/api/?Action=fixtures&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204&from_date={0}&to_date={1}", formatStart, formatEnd);  // <-- this returns formatted json
                string uri = string.Format("http://api.football-api.com/2.0/matches?Authorization=565ec012251f932ea4000001ce56c3d1cd08499276e255f4b481bd85&comp_id=1204&from_date={0}&to_date={1}", formatStart, formatEnd);

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    //Test
                    //string body = System.IO.File.ReadAllText(@"C:\Users\Wayne\Documents\GitHub\HonestApps\FeedData\fixtures.txt");

                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    JArray a = JArray.Parse(s);

                    foreach (var item in a)
                    {
                        //Check if Update History Match details is true/false
                        var updateHistory = GetUpdateHistoryByMatchAPIId(Convert.ToInt32(item["id"]));
                        bool matchDetailsAdded = false;

                        if (updateHistory != null)
                        {
                            matchDetailsAdded = updateHistory.MatchDetails;
                        }

                        if (matchDetailsAdded == false)
                        {
                            int matchAPIId = -1;

                            try
                            {
                                Match match = new Match();

                                match.APIId = Convert.ToInt32(item["id"]);
                                match.Date = Convert.ToDateTime(string.Format("{0} {1}", item["formatted_date"].ToString().Replace('.', '/'), item["time"].ToString()));
                                match.EndDate = match.Date.Value.AddHours(2);
                                match.Active = DateTime.Now <= match.EndDate ? true : false;
                                match.IsToday = DateTime.Now.ToShortDateString() == match.Date.Value.ToShortDateString() ? true : false;
                                //change this when live
                                //match.IsLive = true;
                                match.IsLive = DateTime.Now >= match.Date && DateTime.Now <= match.EndDate ? true : false;
                                match.Time = item["time"].ToString();

                                if (match.IsLive)
                                    matchesToday.Add(match.APIId, match.Time);

                                var homeTeam = GetTeamByAPIId((int)item["localteam_id"]);
                                var awayTeam = GetTeamByAPIId((int)item["visitorteam_id"]);

                                if (homeTeam != null)
                                {
                                    //match.HomeTeamId = homeTeam.Id;
                                    match.Stadium = homeTeam.Stadium;
                                }

                                if (awayTeam != null)
                                    //match.AwayTeamId = awayTeam.Id;

                                match.HomeTeamAPIId = (int)item["localteam_id"];
                                match.AwayTeamAPIId = (int)item["visitorteam_id"];

                                string Id;
                                string actionMessage;

                                retMsg = SaveMatch(match);

                                ReturnId(retMsg, out actionMessage, out Id);

                                if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                {
                                    match.Id = new Guid(Id);
                                }

                                //Add summary
                                Summary summary = new Summary();

                                summary.MatchId = match.Id;
                                summary.HomeTeam = homeTeam.Id;
                                summary.AwayTeam = awayTeam.Id;

                                SaveMatchSummary(summary);

                                //Add update history
                                UpdateHistory history = new UpdateHistory();

                                history.MatchAPIId = match.APIId;
                                history.MatchDetails = true;
                                history.Id = System.Guid.Empty;

                                SaveUpdateHistory(history);

                                matchAPIId = match.APIId;
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                SaveException(ex, string.Format("SaveMatch - FeedData, MatchAPIId: {0}", matchAPIId.ToString()));
                            }
                        }
                        else
                        {
                            var retMatch = GetMatchByAPIId(Convert.ToInt32(item["id"]));

                            if (retMatch != null)
                            {
                                if (DateTime.Now.ToShortDateString() == retMatch.Date.Value.ToShortDateString())
                                    matchesToday.Add(retMatch.APIId, retMatch.Time);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                    //Console.WriteLine(string.Format("Inner Exception: {0}, Message: {1}, Stack Trace: {2}", ex.InnerException.ToString(), ex.Message, ex.StackTrace));
                    SaveException(ex, string.Format("SaveMatch - FeedData"));
            }

            return matchesToday;
        }

        #region Save

        private static string SaveMatch(Match updatedRecord)
        {
            Guid Id = Guid.Empty;

            HonestAppsEntities context = new HonestAppsEntities();

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

        private static string SaveMatchSummary(Summary updatedRecord)
        {
            Guid Id = Guid.Empty;

           HonestAppsEntities context = new HonestAppsEntities();

                if (updatedRecord == null)
                {
                    return Res.Resources.NotFound;
                }
                //Create record
                updatedRecord.Id = Guid.NewGuid();
                updatedRecord.Active = true;
                updatedRecord.Deleted = false;
                updatedRecord.DateAdded = DateTime.Now;
                updatedRecord.DateUpdated = DateTime.Now;

                context.Summaries.Add(updatedRecord);
                Id = updatedRecord.Id;

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

        private static string SaveUpdateHistory(UpdateHistory updatedRecord)
        {
            bool isNewRecord = false;
            Guid Id;

            HonestAppsEntities context = new HonestAppsEntities();

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

                    context.UpdateHistories.Add(updatedRecord);
                    Id = updatedRecord.Id;

                    isNewRecord = true;
                }
                else
                {
                    //Update record
                    var recordToUpdate = context.UpdateHistories.Find(updatedRecord.Id);

                    if (recordToUpdate == null)
                    {
                        return Res.Resources.NotFound;
                    }

                    recordToUpdate.Active = updatedRecord.Active;
                    recordToUpdate.Lineups = updatedRecord.Lineups;
                    recordToUpdate.MatchAPIId = updatedRecord.MatchAPIId;
                    recordToUpdate.MatchDetails = updatedRecord.MatchDetails;
                    recordToUpdate.Deleted = updatedRecord.Deleted;
                    recordToUpdate.DateUpdated = DateTime.Now;
                    recordToUpdate.UpdatedByUserId = updatedRecord.UpdatedByUserId;
                    recordToUpdate.Active = updatedRecord.Active;

                    context.Entry(recordToUpdate).State = System.Data.Entity.EntityState.Modified;
                    Id = updatedRecord.Id;
                }
            }
            else
            {
                return Res.Resources.NotFound;
            }

            try
            {
                context.SaveChanges();

                return isNewRecord ? string.Format("{0}:{1}", Res.Resources.RecordAdded, Id.ToString())
                    : string.Format("{0}:{1}", Res.Resources.RecordUpdated, Id.ToString());
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        private static string SaveMatchToday(MatchesToday updatedRecord)
        {
            Guid Id = Guid.Empty;

            HonestAppsEntities context = new HonestAppsEntities();

            if (updatedRecord != null)
            {
                //Create record
                updatedRecord.Id = Guid.NewGuid();
                updatedRecord.Active = true;
                updatedRecord.Deleted = false;
                updatedRecord.DateAdded = DateTime.Now;
                updatedRecord.DateUpdated = DateTime.Now;

                context.MatchesTodays.Add(updatedRecord);
            }

            try
            {
                context.SaveChanges();

                return string.Format("{0}", Res.Resources.RecordAdded);
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        private static string DeleteMatchesToday()
        {
            HonestAppsEntities context = new HonestAppsEntities();

            var allRecords = context.MatchesTodays;

            if (allRecords != null)
            {
                foreach (var record in allRecords)
                {
                    context.MatchesTodays.Remove(record);
                }
            }

            try
            {
                context.SaveChanges();

                return string.Format("{0}", Res.Resources.RecordUpdated);
            }
            catch (Exception e)
            {
                return string.Format("Error: {0}", e.InnerException.ToString());
            }
        }

        private static void SaveException(Exception exception, string additionalInfo)
        {
            SiteException record = new SiteException();
            HonestAppsEntities context = new HonestAppsEntities();
      
                if (exception != null)
                {
                    //Create record
                    record.HResult = exception.HResult.ToString();
                    record.InnerException = record.InnerException != null ? exception.InnerException.ToString() : string.Empty;
                    record.Message = string.Format("Additional Info:{0}, Exception: {1} ", additionalInfo, exception.Message);
                    record.Source = exception.Source;
                    record.StackTrace = record.StackTrace != null ? string.Format("Stack Trace: {0}", exception.StackTrace.ToString()) : string.Empty;
                    record.TargetSite = record.TargetSite != null ? exception.TargetSite.ToString() : string.Empty;
                    record.DateAdded = DateTime.Now;

                    context.SiteExceptions.Add(record);
                }

                try
                {
                    context.SaveChanges();
                }
                catch { }
        }

        #endregion

        #region Get

        private static UpdateHistory GetUpdateHistoryByMatchAPIId(int id)
        {
            HonestAppsEntities context = new HonestAppsEntities();

            return context.UpdateHistories.Where(x => (x.MatchAPIId == id)).FirstOrDefault();
        }

        private static Team GetTeamByAPIId(int id)
        {
            HonestAppsEntities context = new HonestAppsEntities();

            return context.Teams.Where(x => (x.APIId == id)).FirstOrDefault();
        }

        private static Match GetMatchByAPIId(int id)
        {
            HonestAppsEntities context = new HonestAppsEntities();

            return context.Matches.Where(x => (x.APIId == id)).FirstOrDefault();
        }

        #endregion

        private static void ReturnId(string retMessage, out string recordMessage, out string Id)
        {
            string[] message = retMessage.Split(':');

            recordMessage = string.Empty;
            Id = string.Empty;

            if (message.Count() == 2)
            {
                recordMessage = message[0];
                Id = message[1];
            }
            else
            {
                recordMessage = message[0];
            }
        }
    }
}
