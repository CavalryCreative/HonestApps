﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Globalization;
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
        public static string CompId { get; set; }
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
            CompId = "1204";//1056 World Cup, 1204 Premier League

            UpdatePlayers();
            GetFixtures();
        }

        private static void GetFixtures()
        {
            //Clear today matches table
            DeleteMatchesToday();

            IDictionary<int, string> matchesToday = new Dictionary<int, string>();

            int currentYear = DateTime.Now.Year;
            DateTime startDate = new DateTime(currentYear, 8, 1);
            matchesToday = GetFixtures(DateTime.Now, DateTime.Now.AddDays(304));

            foreach (var kvp in matchesToday)
            {
                MatchesToday match = new MatchesToday();
                match.APIId = kvp.Key;
                match.KickOffTime = kvp.Value;

                SaveMatchToday(match);
            }

            DeleteLeagueStandings();
            GetLeagueStandings();

            //GetFixtures(DateTime.Now, DateTime.Now.AddDays(7));
        }

        private static void UpdatePlayers()
        {
            IList<Team> teams = GetAllTeams();

            foreach (var team in teams)
            {
                string uri = string.Format("http://api.football-api.com/2.0/team/{0}?Authorization=565ec012251f932ea4000001ce56c3d1cd08499276e255f4b481bd85", team.APIId.ToString());

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    JToken token = JObject.Parse(s);
                    string jPath = "squad";

                    var y = token.SelectTokens(jPath);

                    string Id;
                    string actionMessage;
                    string retMsg = string.Empty;
                    Byte squadNumberByte;
                    int playerAPIId;
                    bool result;

                    try
                    {
                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            Player player = new Player();

                            var playerId = childToken.SelectToken("id").ToString();
                            var playerName = childToken.SelectToken("name").ToString();
                            var playerNumber = childToken.SelectToken("number").ToString();
                            var playerPos = childToken.SelectToken("position").ToString();

                            result = Int32.TryParse(playerId, out playerAPIId);

                            //Check if player exists                           
                            if (result)
                                player = GetPlayerByAPIId(playerAPIId);

                            if (player == null)
                            {
                                player = new Player();

                                result = Byte.TryParse(playerNumber, out squadNumberByte);

                                if (result)
                                    player.SquadNumber = squadNumberByte;
                                else
                                    player.SquadNumber = 0;

                                player.Name = playerName;
                                player.Position = playerPos;
                                player.Id = System.Guid.Empty;
                                player.APIPlayerId = playerAPIId;

                                retMsg = SavePlayer(player, team.Id);

                                ReturnId(retMsg, out actionMessage, out Id);

                                if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                {
                                    player.Id = new Guid(Id);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SaveException(ex, string.Format("SavePlayer - FeedData"));
                    }
                }
            }
        }

        private static IDictionary<int, string> GetFixtures(DateTime startDate, DateTime endDate)
        {
            IDictionary<int, string> matchesToday = new Dictionary<int, string>();
            string retMsg = string.Empty;

            string startDateStr = startDate.ToShortDateString();
            string endDateStr = endDate.ToShortDateString();

            string[] startArr = startDateStr.Split('/');
            string[] endArr = endDateStr.Split('/');

            CultureInfo culture = new CultureInfo("en-US");

            string formatStart = string.Empty;
            string formatEnd = string.Empty;

            if (culture.Name == "en-US")
            {
                formatStart = string.Format("{0}.{1}.{2}", startArr[1], startArr[0], startArr[2]);
                formatEnd = string.Format("{0}.{1}.{2}", endArr[1], endArr[0], endArr[2]);
            }
            else
            {
                formatStart = string.Format("{0}.{1}.{2}", startArr[0], startArr[1], startArr[2]);
                formatEnd = string.Format("{0}.{1}.{2}", endArr[0], endArr[1], endArr[2]);
            }
       
            string uri = string.Empty;

            try
            {
                //string uri = string.Format("http://football-api.com/api/?Action=fixtures&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204&from_date={0}&to_date={1}", formatStart, formatEnd);  // <-- this returns formatted json
                uri = string.Format("http://api.football-api.com/2.0/matches?comp_id={0}&from_date={1}&to_date={2}&Authorization=565ec012251f932ea4000001ce56c3d1cd08499276e255f4b481bd85", CompId, formatStart, formatEnd);
                
                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return matchesToday;
                    }
                    
                    //Test
                    //string body = System.IO.File.ReadAllText(@"C:\Users\Wayne\Documents\GitHub\HonestApps\FeedData\fixtures.txt");

                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    JArray a = JArray.Parse(s);

                    bool pingRequestSent = false;

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

                                string matchDate = string.Format("{0} {1}", item["formatted_date"].ToString().Replace('.', '/'), item["time"].ToString());

                                if (culture.Name == "en-US")
                                {
                                    string[] matchDateArr = matchDate.Split('/');

                                    matchDate = string.Format("{0}/{1}/{2}", matchDateArr[1], matchDateArr[0], matchDateArr[2]);
                                }
    
                                match.Date = DateTime.Parse(matchDate, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                                match.EndDate = match.Date.Value.AddHours(2);
                                match.Active = DateTime.Now <= match.EndDate ? true : false;
                                match.IsToday = DateTime.Now.ToShortDateString() == match.Date.Value.ToShortDateString() ? true : false;                                                             
                                match.Time = item["time"].ToString();
                                match.IsLive = false;
                                match.HalfTimeScore = item["ht_score"].ToString();
                                match.FullTimeScore = item["ft_score"].ToString();

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

                                if (DateTime.Now.ToShortDateString() == match.Date.Value.ToShortDateString())
                                {
                                    matchesToday.Add(match.APIId, match.Time);

                                    match.IsToday = true;

                                    if (pingRequestSent == false)
                                    {
                                        try
                                        {
                                            WebClient http = new WebClient();
                                            string Result = http.DownloadString("http://honest-apps.elasticbeanstalk.com/");

                                            pingRequestSent = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            string Message = ex.Message;
                                        }
                                    }
                                }
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
                                {
                                    matchesToday.Add(retMatch.APIId, retMatch.Time);

                                    retMatch.IsToday = true;

                                    if (pingRequestSent == false)
                                    {
                                        try
                                        {
                                            WebClient http = new WebClient();
                                            string Result = http.DownloadString("http://honest-apps.elasticbeanstalk.com/");

                                            pingRequestSent = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            string Message = ex.Message;
                                        }
                                    }
                                }

                                retMsg = SaveMatch(retMatch);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                    //Console.WriteLine(string.Format("Inner Exception: {0}, Message: {1}, Stack Trace: {2}", ex.InnerException.ToString(), ex.Message, ex.StackTrace));
                    SaveException(ex, string.Format("SaveMatch - FeedData - {0}", uri));
            }

            return matchesToday;
        }

        private static string GetLeagueStandings()
        {
            string uri = string.Empty;
            string retMsg = string.Empty;

            try
            {
                uri = string.Format("http://api.football-api.com/2.0/standings/{0}?Authorization=565ec012251f932ea4000001ce56c3d1cd08499276e255f4b481bd85", CompId);

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Resources.Resources.RecordNull;
                    }

                    //Test
                    //string body = System.IO.File.ReadAllText(@"C:\Users\Wayne\Documents\GitHub\HonestApps\FeedData\fixtures.txt");

                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    JArray a = JArray.Parse(s);

                    foreach (var item in a)
                    {
                        try
                        {
                            LeagueStanding leagueStanding = new LeagueStanding();
                              
                            leagueStanding.Active = true;
                            leagueStanding.Name = item["team_name"].ToString();
                            leagueStanding.Position = Convert.ToByte(item["position"]);
                            leagueStanding.GamesPlayed = Convert.ToByte(item["overall_gp"]);
                            leagueStanding.GamesWon = Convert.ToByte(item["overall_w"]);
                            leagueStanding.GamesDrawn = Convert.ToByte(item["overall_d"]);
                            leagueStanding.GamesLost = Convert.ToByte(item["overall_l"]);
                            leagueStanding.GoalsScored = Convert.ToByte(item["overall_gs"]);
                            leagueStanding.GoalsConceded = Convert.ToByte(item["overall_ga"]);
                            leagueStanding.GoalDifference = Convert.ToInt32(item["gd"]);
                            leagueStanding.Points = Convert.ToByte(item["points"]);
                            leagueStanding.Description = item["description"].ToString();

                            var team = GetTeamByName(leagueStanding.Name);

                            if (team != null)
                            {
                                leagueStanding.APIId = team.APIId;
                            }

                            string Id;
                            string actionMessage;

                            retMsg = SaveLeagueStandings(leagueStanding);

                            ReturnId(retMsg, out actionMessage, out Id);

                            if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                            {
                                    leagueStanding.Id = new Guid(Id);
                            }
                         }
                         catch (Exception ex)
                         {
                            SaveException(ex, string.Format("SaveLeagueStandings - FeedData"));
                         }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(string.Format("Inner Exception: {0}, Message: {1}, Stack Trace: {2}", ex.InnerException.ToString(), ex.Message, ex.StackTrace));
                SaveException(ex, string.Format("SaveLeagueStandings - FeedData - {0}", uri));
            }

            return retMsg;
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
                recordToUpdate.HalfTimeScore = updatedRecord.HalfTimeScore;
                recordToUpdate.FullTimeScore = updatedRecord.FullTimeScore;

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

        private static string DeleteLeagueStandings()
        {
            HonestAppsEntities context = new HonestAppsEntities();

            var allRecords = context.LeagueStandings;

            if (allRecords != null)
            {
                foreach (var record in allRecords)
                {
                    context.LeagueStandings.Remove(record);
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

        private static string SaveLeagueStandings(LeagueStanding updatedRecord)
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

                context.LeagueStandings.Add(updatedRecord);
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

        private static string SavePlayer(Player updatedRecord, Guid teamId)
        {
            Guid Id = Guid.Empty;

            HonestAppsEntities context = new HonestAppsEntities();

            if (updatedRecord != null)
            {
                //Update record
                var recordToUpdate = context.Teams.Find(teamId);

                if (recordToUpdate == null)
                {
                    return Res.Resources.NotFound;
                }

                if (updatedRecord.Id == System.Guid.Empty)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    //recordToUpdate.Players.Clear();
                    recordToUpdate.Players.Add(updatedRecord);
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

        private static Player GetPlayerByAPIId(int id)
        {
            HonestAppsEntities context = new HonestAppsEntities();

            return context.Players.Where(x => (x.APIPlayerId == id)).FirstOrDefault();
        }

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

        private static Team GetTeamByName(string name)
        {
            HonestAppsEntities context = new HonestAppsEntities();

            return context.Teams.Where(x => (x.Name.Trim().ToLower() == name.Trim().ToLower())).FirstOrDefault();
        }

        private static IList<Team> GetAllTeams()
        {
            HonestAppsEntities context = new HonestAppsEntities();

            return context.Teams.Where(x => x.Active == true).ToList();
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
