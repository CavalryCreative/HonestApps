using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Hosting;
using CMS.Infrastructure.Concrete;
using Res = Resources;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;

//using System.Data.Entity;
//using System.Data.Entity.Core.Objects;
//using System.Data.Entity.Infrastructure;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedUpdate : IRegisteredObject
    {
        // Singleton instance
        private readonly static Lazy<FeedUpdate> _instance = new Lazy<FeedUpdate>(
            () => new FeedUpdate(GlobalHost.ConnectionManager.GetHubContext<FeedHub>().Clients));
       
        //private Timer matchTimer;
        private Timer eventsTimer;
        string IPAddress = string.Empty;

        private FeedUpdate(IHubConnectionContext<dynamic> clients)
        {
            HostingEnvironment.RegisterObject(this);

            Clients = clients;
            //matchTimer = new Timer(GetFixtures, null, TimeSpan.FromSeconds(1), TimeSpan.FromDays(1));
      
            eventsTimer = new Timer(BroadcastFeed, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(12000));
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public static FeedUpdate Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void Stop(bool immediate)
        {
            //matchTimer.Dispose();
            eventsTimer.Dispose();

            HostingEnvironment.UnregisterObject(this);
        }

        public void BroadcastFeed(object sender)
        {
            GetAllCommentaries();
            BroadcastFeed();

            //SaveBroadcastFeed("", GetIPAddress());
        }
        
        public void BroadcastFeed()
        {
            string feed = string.Empty;
            feed = GetFeed();

            Clients.All.showMessage(feed);
        }

        #region Private Methods

        private void GetAllCommentaries()
        {
            var todayMatches = GetLiveMatches();
            Object thisLock = new Object();

            if (todayMatches.Count > 0)
            {
                foreach (var match in todayMatches)
                {
                    lock (thisLock)
                    {
                        GetCommentaries(match);
                    }                    
                }
            }

            //if (DateTime.Now >= retMatch.Date && DateTime.Now <= retMatch.EndDate)
            //{

            //}
            //else
            //{
            //    match.IsLive = false;
            //}
        }

        private static string GetCommentaries(int matchId)
        {
            string retMsg = string.Empty;

            //Fixtures
            //http://football-api.com/api/?Action=fixtures&APIKey=565ec012251f932ea4000001ce56c3d1cd08499276e255f4b481bd85&comp_id=1204&from_date=17.10.2015&to_date=27.10.2015
            //http://api.football-api.com/2.0/matches?Authorization=565ec012251f932ea4000001393b4115a8bf4bf551672b0543e35683&comp_id=1204&from_date=25.09.2016&to_date=26.09.2016

            //Today live match events - brings back same feed as above
            //http://api.football-api.com/2.0/matches?Authorization=565ec012251f932ea4000001393b4115a8bf4bf551672b0543e35683&comp_id=1204
            //http://football-api.com/api/?Action=today&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204

            //Commentaries
            //http://football-api.com/api/?Action=commentaries&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&match_id=2136144
            //http://api.football-api.com/2.0/commentaries/2058958?Authorization=565ec012251f932ea4000001393b4115a8bf4bf551672b0543e35683

            //Standings
            //http://football-api.com/api/?Action=standings&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&comp_id=1204

            try
            {
                //http://api.football-api.com/2.0/commentaries/2058958?Authorization=565ec012251f932ea4000001393b4115a8bf4bf551672b0543e35683
                string uri = "http://api.football-api.com/2.0/commentaries/" + matchId.ToString()  + "?Authorization=565ec012251f932ea4000001393b4115a8bf4bf551672b0543e35683";  // <-- this returns formatted json

                //var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                //webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                //var webResponse = (HttpWebResponse)webRequest.GetResponse();

                //if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                //{
                //    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                //    {
                //        return "No commentaries found.";
                //    }

                //    var reader = new StreamReader(webResponse.GetResponseStream());
                //    string s = reader.ReadToEnd();

                    //Test
                    string s = System.IO.File.ReadAllText(@"C:\Users\Wayne\Documents\GitHub\HonestApps\CMS\MatchId2058953.txt");

                    JToken token = JObject.Parse(s);

                    var match = GetMatchByAPIId(matchId);

                    var homeTeam = GetTeamByAPIId(match.HomeTeamAPIId);
                    var awayTeam = GetTeamByAPIId(match.AwayTeamAPIId);
                    var summary = GetSummaryByMatchId(match.Id);

                    string jPath = string.Empty;

                    var checkIfExists = token.SelectToken("match_id");

                    if (checkIfExists != null)
                    {
                        //Check if Update History Lineups is true/false
                        var updateHistory = GetUpdateHistoryByMatchAPIId(matchId);
                        bool lineupsAdded = false;

                        if (updateHistory != null)
                        {
                            lineupsAdded = updateHistory.Lineups;
                        }

                        #region Lineups

                        if (lineupsAdded == false)
                        {
                            #region Home Team

                            jPath = "lineup.localteam";

                            var y = token.SelectTokens(jPath);

                            string Id;
                            string actionMessage;
                            Byte squadNumberByte;
                            int playerAPIId;
                            bool result;

                            try
                            {
                                foreach (var childToken in y.Children())
                                {
                                    var jeff = childToken.Children();

                                    Player homePlayer = new Player();

                                    var playerHomeId = childToken.SelectToken("id").ToString();
                                    var playerHomeName = childToken.SelectToken("name").ToString();
                                    var playerHomeNumber = childToken.SelectToken("number").ToString();
                                    var playerHomePos = childToken.SelectToken("pos").ToString();

                                    result = Int32.TryParse(playerHomeId, out playerAPIId);

                                    //Check if player exists                           
                                    if (result)
                                        homePlayer = GetPlayerByAPIId(playerAPIId);

                                    if (homePlayer == null)
                                        homePlayer = GetPlayerByName(playerHomeName);

                                    //Add to Players if not already added
                                    if (homePlayer == null)
                                    {
                                        homePlayer = new Player();

                                        result = Byte.TryParse(playerHomeNumber, out squadNumberByte);

                                        if (result)
                                            homePlayer.SquadNumber = squadNumberByte;
                                        else
                                            homePlayer.SquadNumber = 0;

                                        homePlayer.Name = playerHomeName;
                                        homePlayer.Position = playerHomePos;
                                        homePlayer.Id = System.Guid.Empty;
                                        homePlayer.APIPlayerId = playerAPIId;

                                        retMsg = SavePlayer(homePlayer, homeTeam.Id);

                                        ReturnId(retMsg, out actionMessage, out Id);

                                        if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                        {
                                            homePlayer.Id = new Guid(Id);
                                        }
                                    }

                                    if (homePlayer != null)
                                    {
                                        //add to Lineups
                                        Lineup lineup = new Lineup();

                                        lineup.IsHomePlayer = true;
                                        lineup.IsSub = false;
                                        lineup.MatchAPIId = matchId;
                                        lineup.PlayerId = homePlayer.Id;
                                        lineup.Position = playerHomePos;

                                        SavePlayerToMatchLineup(lineup);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                SaveException(ex, string.Format("SavePlayer - Home, Match Id: {0}", matchId.ToString()));
                            }

                            //System.Diagnostics.Debug.WriteLine("Home players complete");

                            #endregion

                            #region Home subs

                            jPath = "subs.localteam";

                            y = token.SelectTokens(jPath);

                            try
                            {
                                foreach (var childToken in y.Children())
                                {
                                    var jeff = childToken.Children();

                                    //Check if player exists
                                    Player homeSubPlayer = new Player();

                                    var playerHomeSubId = childToken.SelectToken("id").ToString();
                                    var playerHomeSubName = childToken.SelectToken("name").ToString();
                                    var playerHomeSubNumber = childToken.SelectToken("number").ToString();
                                    var playerHomeSubPos = childToken.SelectToken("pos").ToString();

                                    result = Int32.TryParse(playerHomeSubId, out playerAPIId);

                                    if (result)
                                        homeSubPlayer = GetPlayerByAPIId(playerAPIId);

                                    if (homeSubPlayer == null)
                                        homeSubPlayer = GetPlayerByName(playerHomeSubName);

                                    //Add to Players if not already added
                                    if (homeSubPlayer == null)
                                    {
                                        homeSubPlayer = new Player();

                                        result = Byte.TryParse(playerHomeSubNumber, out squadNumberByte);

                                        if (result)
                                            homeSubPlayer.SquadNumber = squadNumberByte;
                                        else
                                            homeSubPlayer.SquadNumber = 0;

                                        homeSubPlayer.Name = playerHomeSubName;
                                        homeSubPlayer.Position = playerHomeSubPos;
                                        homeSubPlayer.Id = System.Guid.Empty;
                                        homeSubPlayer.APIPlayerId = playerAPIId;

                                        retMsg = SavePlayer(homeSubPlayer, homeTeam.Id);

                                        ReturnId(retMsg, out actionMessage, out Id);

                                        if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                        {
                                            homeSubPlayer.Id = new Guid(Id);
                                        }
                                    }

                                    if (homeSubPlayer != null)
                                    {
                                        //add to Lineups
                                        Lineup lineup = new Lineup();

                                        lineup.IsHomePlayer = true;
                                        lineup.IsSub = true;
                                        lineup.MatchAPIId = matchId;
                                        lineup.PlayerId = homeSubPlayer.Id;
                                        lineup.Position = playerHomeSubPos;

                                        SavePlayerToMatchLineup(lineup);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                SaveException(ex, string.Format("SavePlayer - Home Sub, Match Id: {0}", matchId.ToString()));
                            }


                            //System.Diagnostics.Debug.WriteLine("Home players subs complete");

                            #endregion

                            #region Away Team

                            jPath = "lineup.visitorteam";

                            y = token.SelectTokens(jPath);

                            try
                            {
                                foreach (var childToken in y.Children())
                                {
                                    var jeff = childToken.Children();

                                    var playerAwayId = childToken.SelectToken("id").ToString();
                                    var playerAwayName = childToken.SelectToken("name").ToString();
                                    var playerAwayNumber = childToken.SelectToken("number").ToString();
                                    var playerAwayPos = childToken.SelectToken("pos").ToString();

                                    result = Int32.TryParse(playerAwayId, out playerAPIId);

                                    //Check if player exists
                                    Player awayPlayer = new Player();

                                    if (result)
                                        awayPlayer = GetPlayerByAPIId(playerAPIId);

                                    if (awayPlayer == null)
                                        awayPlayer = GetPlayerByName(playerAwayName);

                                    //Add to Players if not already added
                                    if (awayPlayer == null)
                                    {
                                        awayPlayer = new Player();

                                        result = Byte.TryParse(playerAwayNumber, out squadNumberByte);

                                        if (result)
                                            awayPlayer.SquadNumber = squadNumberByte;
                                        else
                                            awayPlayer.SquadNumber = 0;

                                        awayPlayer.Name = playerAwayName;
                                        awayPlayer.Position = playerAwayPos;
                                        awayPlayer.Id = System.Guid.Empty;
                                        awayPlayer.APIPlayerId = playerAPIId;

                                        retMsg = SavePlayer(awayPlayer, awayTeam.Id);

                                        ReturnId(retMsg, out actionMessage, out Id);

                                        if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                        {
                                            awayPlayer.Id = new Guid(Id);
                                        }
                                    }

                                    if (awayPlayer != null)
                                    {
                                        //add to Lineups
                                        Lineup lineup = new Lineup();

                                        lineup.IsHomePlayer = false;
                                        lineup.IsSub = false;
                                        lineup.MatchAPIId = matchId;
                                        lineup.PlayerId = awayPlayer.Id;
                                        lineup.Position = playerAwayPos;

                                        SavePlayerToMatchLineup(lineup);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                SaveException(ex, string.Format("SavePlayer - Away, Match Id: {0}", matchId.ToString()));
                            }
                            //System.Diagnostics.Debug.WriteLine("Away players complete");

                            #endregion

                            #region Away subs

                            jPath = "subs.visitorteam";

                            y = token.SelectTokens(jPath);

                            try
                            {
                                foreach (var childToken in y.Children())
                                {
                                    var jeff = childToken.Children();

                                    var playerAwaySubId = childToken.SelectToken("id").ToString();
                                    var playerAwaySubName = childToken.SelectToken("name").ToString();
                                    var playerAwaySubNumber = childToken.SelectToken("number").ToString();
                                    var playerAwaySubPos = childToken.SelectToken("pos").ToString();

                                    result = Int32.TryParse(playerAwaySubId, out playerAPIId);

                                    //Check if player exists
                                    Player awaySubPlayer = new Player();

                                    if (result)
                                        awaySubPlayer = GetPlayerByAPIId(playerAPIId);

                                    if (awaySubPlayer == null)
                                        awaySubPlayer = GetPlayerByName(playerAwaySubName);

                                    //Add to Players if not already added
                                    if (awaySubPlayer == null)
                                    {
                                        awaySubPlayer = new Player();

                                        result = Byte.TryParse(playerAwaySubNumber, out squadNumberByte);

                                        if (result)
                                            awaySubPlayer.SquadNumber = squadNumberByte;
                                        else
                                            awaySubPlayer.SquadNumber = 0;

                                        awaySubPlayer.Name = playerAwaySubName;
                                        awaySubPlayer.Position = playerAwaySubPos;
                                        awaySubPlayer.Id = System.Guid.Empty;
                                        awaySubPlayer.APIPlayerId = playerAPIId;

                                        retMsg = SavePlayer(awaySubPlayer, awayTeam.Id);

                                        ReturnId(retMsg, out actionMessage, out Id);

                                        if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                        {
                                            awaySubPlayer.Id = new Guid(Id);
                                        }
                                    }

                                    if (awaySubPlayer != null)
                                    {
                                        //add to Lineups
                                        Lineup lineup = new Lineup();

                                        lineup.IsHomePlayer = false;
                                        lineup.IsSub = true;
                                        lineup.MatchAPIId = matchId;
                                        lineup.PlayerId = awaySubPlayer.Id;
                                        lineup.Position = playerAwaySubPos;

                                        SavePlayerToMatchLineup(lineup);

                                        UpdateHistory history = new UpdateHistory();

                                        history.Active = true;
                                        history.Deleted = false;
                                        history.MatchDetails = true;
                                        history.MatchAPIId = matchId;
                                        history.Lineups = true;

                                        SaveUpdateHistory(history);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                SaveException(ex, string.Format("SavePlayer - Away Sub, Match Id: {0} ", matchId.ToString()));
                            }
                            //System.Diagnostics.Debug.WriteLine("Away players subs complete");

                            #endregion
                        }

                        #endregion

                        int APIId = 0;
                        Byte numberByte;
                        bool isValid;

                    #region Summaries

                    #region HomeTeam - Goals

                    //jPath = "commentaries.[0].comm_match_summary.localteam.goals.player";

                    //var goals = token.SelectTokens(jPath);
                    //int APIId = 0;
                    //Byte numberByte;
                    //bool isValid;

                    //try
                    //{
                    //    JObject obj = JObject.Parse(s);

                    //    try
                    //    {
                    //        isValid = Int32.TryParse(obj.SelectToken("commentaries.[0].comm_match_summary.localteam.goals.player.id").ToString(), out APIId);
                    //    }
                    //    catch
                    //    {
                    //        isValid = false;
                    //    }                     

                    //    if (isValid)//Just one record
                    //    {
                    //        string minute = obj.SelectToken("commentaries.[0].comm_match_summary.localteam.goals.player.minute").ToString();

                    //        Goal homeGoal = new Goal();

                    //        if (homeGoal.Penalty == true)
                    //        {
                    //            minute = minute.Trim().Remove(0, 4);
                    //        }

                    //        isValid = Byte.TryParse(minute, out numberByte);

                    //        if (isValid)
                    //            homeGoal.Minute = numberByte;
                    //        else
                    //            homeGoal.Minute = 0;

                    //        //Check if goal exists                       
                    //        var goalExists = GetGoalByAPIId(APIId, homeGoal.Minute);

                    //        if (goalExists == null)
                    //        {
                    //            homeGoal.APIId = APIId;
                    //            homeGoal.SummaryId = summary.Id;
                    //            homeGoal.IsHomeTeam = true;
                    //            var player = GetPlayerByName(obj.SelectToken("commentaries.[0].comm_match_summary.localteam.goals.player.name").ToString());

                    //            if (player != null)
                    //            {
                    //                homeGoal.PlayerName = player.Name;
                    //                homeGoal.APIPlayerId = player.APIPlayerId;
                    //            }
                    //            else
                    //            {
                    //                homeGoal.PlayerName = player.Name;
                    //                homeGoal.APIPlayerId = 0;
                    //            }

                    //            homeGoal.OwnGoal = obj.SelectToken("commentaries.[0].comm_match_summary.localteam.goals.player.owngoal").ToString() == "False" ? false : true;
                    //            homeGoal.Penalty = obj.SelectToken("commentaries.[0].comm_match_summary.localteam.goals.player.penalty").ToString() == "False" ? false : true;

                    //            retMsg = SaveGoal(homeGoal);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (var childToken in goals.Children())
                    //        {
                    //            var jeff = childToken.Children();

                    //            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                    //            {
                    //                isValid = Int32.TryParse(evt["id"], out APIId);

                    //                if (isValid)
                    //                {
                    //                    Goal homeGoal = new Goal();

                    //                    string minute = evt["minute"];
                    //                    homeGoal.OwnGoal = evt["owngoal"] == "False" ? false : true;
                    //                    homeGoal.Penalty = evt["penalty"] == "False" ? false : true;                                           

                    //                    if (homeGoal.Penalty == true)
                    //                    {
                    //                        minute = minute.Trim().Remove(0, 4);
                    //                    }

                    //                    isValid = Byte.TryParse(minute, out numberByte);

                    //                    if (isValid)
                    //                        homeGoal.Minute = numberByte;
                    //                    else
                    //                        homeGoal.Minute = 0;

                    //                    //Check if goal exists
                    //                    var goalExists = GetGoalByAPIId(APIId, homeGoal.Minute);

                    //                    if (goalExists == null)
                    //                    {                                           
                    //                        homeGoal.APIId = APIId;
                    //                        homeGoal.SummaryId = summary.Id;
                    //                        homeGoal.IsHomeTeam = true;
                    //                        var player = GetPlayerByName(evt["name"]);

                    //                        if (player != null)
                    //                        {
                    //                            homeGoal.PlayerName = player.Name;
                    //                            homeGoal.APIPlayerId = player.APIPlayerId;
                    //                        }
                    //                        else
                    //                        {
                    //                            homeGoal.PlayerName = player.Name;
                    //                            homeGoal.APIPlayerId = 0;
                    //                        }

                    //                        retMsg = SaveGoal(homeGoal);
                    //                    }
                    //                }
                    //            }
                    //        }//
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                    //    SaveException(ex, string.Format("HomeGoal, Match Id: {0} ", matchId.ToString()));
                    //}
                    #endregion

                    #region AwayTeam - Goals

                    //jPath = "commentaries.[0].comm_match_summary.visitorteam.goals.player";

                    //goals = token.SelectTokens(jPath);

                    // try
                    // {
                    //       JObject obj = JObject.Parse(s);

                    //       try 
                    //       {
                    //           //Check if only one record exists
                    //           isValid = Int32.TryParse(obj.SelectToken("commentaries.[0].comm_match_summary.visitorteam.goals.player.id").ToString(), out APIId);
                    //       }
                    //       catch 
                    //       {
                    //           isValid = false;
                    //       }

                    //         if (isValid)
                    //         {
                    //             Goal awayGoal = new Goal();

                    //             awayGoal.OwnGoal = obj.SelectToken("commentaries.[0].comm_match_summary.visitorteam.goals.player.owngoal").ToString() == "False" ? false : true;
                    //             awayGoal.Penalty = obj.SelectToken("commentaries.[0].comm_match_summary.visitorteam.goals.player.penalty").ToString() == "False" ? false : true;

                    //             string minute = obj.SelectToken("commentaries.[0].comm_match_summary.visitorteam.goals.player.minute").ToString();

                    //             if (awayGoal.Penalty == true)
                    //             {
                    //                 minute = minute.Trim().Remove(0, 4);
                    //             }

                    //             isValid = Byte.TryParse(minute, out numberByte);

                    //             if (isValid)
                    //                 awayGoal.Minute = numberByte;
                    //             else
                    //                 awayGoal.Minute = 0;

                    //             //Check if goal exists
                    //             var goalExists = GetGoalByAPIId(APIId, awayGoal.Minute);

                    //             if (goalExists == null)
                    //             {
                    //                 awayGoal.APIId = APIId;
                    //                 awayGoal.SummaryId = summary.Id;
                    //                 awayGoal.IsHomeTeam = false;
                    //                 var player = GetPlayerByName(obj.SelectToken("commentaries.[0].comm_match_summary.visitorteam.goals.player.name").ToString());

                    //                 if (player != null)
                    //                 {
                    //                     awayGoal.PlayerName = player.Name;
                    //                     awayGoal.APIPlayerId = player.APIPlayerId;
                    //                 }
                    //                 else
                    //                 {
                    //                     awayGoal.PlayerName = player.Name;
                    //                     awayGoal.APIPlayerId = 0;
                    //                 }

                    //                 retMsg = SaveGoal(awayGoal);
                    //             }
                    //         }
                    //         else
                    //         {
                    //             foreach (var childToken in goals.Children())
                    //             {
                    //                 var jeff = childToken.Children();

                    //                 foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                    //                 {
                    //                     isValid = Int32.TryParse(evt["id"], out APIId);

                    //                     if (isValid)
                    //                     {
                    //                         string minute = evt["minute"];
                    //                         Goal awayGoal = new Goal();

                    //                         if (awayGoal.Penalty == true)
                    //                         {
                    //                             minute = minute.Trim().Remove(0, 4);
                    //                         }

                    //                         isValid = Byte.TryParse(minute, out numberByte);

                    //                         if (isValid)
                    //                             awayGoal.Minute = numberByte;
                    //                         else
                    //                             awayGoal.Minute = 0;

                    //                         //Check if goal exists   
                    //                         var goalExists = GetGoalByAPIId(APIId, awayGoal.Minute);

                    //                         if (goalExists == null)
                    //                         {                                              
                    //                             awayGoal.APIId = APIId;
                    //                             awayGoal.SummaryId = summary.Id;
                    //                             awayGoal.IsHomeTeam = false;
                    //                             var player = GetPlayerByName(evt["name"]);

                    //                             if (player != null)
                    //                             {
                    //                                 awayGoal.PlayerName = player.Name;
                    //                                 awayGoal.APIPlayerId = player.APIPlayerId;
                    //                             }
                    //                             else
                    //                             {
                    //                                 awayGoal.PlayerName = player.Name;
                    //                                 awayGoal.APIPlayerId = 0;
                    //                             }

                    //                             awayGoal.OwnGoal = evt["owngoal"] == "False" ? false : true;
                    //                             awayGoal.Penalty = evt["penalty"] == "False" ? false : true;                                                

                    //                             retMsg = SaveGoal(awayGoal);
                    //                         }
                    //                     }
                    //                 }
                    //             }
                    //         }
                    //}
                    // catch (Exception ex)
                    // {
                    //     //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                    //     SaveException(ex, string.Format("HomeGoal, Match Id: {0} ", matchId.ToString()));
                    // }
                    #endregion

                    #endregion

                    #region Match Stats

                    try
                        {
                            Stat stat = new Stat();
                            stat.MatchId = match.Id;

                            var matchStats = GetMatchStatsByAPIId(match.Id);

                            if (matchStats != null)
                                stat.Id = matchStats.Id;

                            JObject obj = JObject.Parse(s);

                            jPath = "match_stats.localteam";

                            var homeMatchStatsToken = token.SelectTokens(jPath);

                            if (homeMatchStatsToken != null)
                            {
                                foreach (var statToken in homeMatchStatsToken.Children())
                                {
                                    stat.HomeTeamCorners = Convert.ToByte(statToken.SelectToken("corners").ToString());
                                    stat.HomeTeamOffsides = Convert.ToByte(statToken.SelectToken("offsides").ToString());
                                    stat.HomeTeamPossessionTime = statToken.SelectToken("possesiontime").ToString();
                                    stat.HomeTeamSaves = Convert.ToByte(statToken.SelectToken("saves").ToString());
                                    stat.HomeTeamOnGoalShots = Convert.ToByte(statToken.SelectToken("shots_ongoal").ToString());
                                    stat.HomeTeamTotalShots = Convert.ToByte(statToken.SelectToken("shots_total").ToString());
                                    stat.HomeTeamFouls = Convert.ToByte(statToken.SelectToken("fouls").ToString());
                                    stat.HomeTeamRedCards = Convert.ToByte(statToken.SelectToken("redcards").ToString());
                                    stat.HomeTeamYellowCards = Convert.ToByte(statToken.SelectToken("yellowcards").ToString());
                                }
                            }                            

                            jPath = "match_stats.visitorteam";

                            var awayMatchStatsToken = token.SelectTokens(jPath);

                            if (awayMatchStatsToken != null)
                            {
                                foreach (var statToken in awayMatchStatsToken.Children())
                                {
                                    stat.AwayTeamCorners = Convert.ToByte(statToken.SelectToken("corners").ToString());
                                    stat.AwayTeamOffsides = Convert.ToByte(statToken.SelectToken("offsides").ToString());
                                    stat.AwayTeamPossessionTime = statToken.SelectToken("possesiontime").ToString();
                                    stat.AwayTeamSaves = Convert.ToByte(statToken.SelectToken("saves").ToString());
                                    stat.AwayTeamOnGoalShots = Convert.ToByte(statToken.SelectToken("shots_ongoal").ToString());
                                    stat.AwayTeamTotalShots = Convert.ToByte(statToken.SelectToken("shots_total").ToString());
                                    stat.AwayTeamFouls = Convert.ToByte(statToken.SelectToken("fouls").ToString());
                                    stat.AwayTeamRedCards = Convert.ToByte(statToken.SelectToken("redcards").ToString());
                                    stat.AwayTeamYellowCards = Convert.ToByte(statToken.SelectToken("yellowcards").ToString());
                                }
                            }

                            SaveMatchStats(stat);
                        }
                        catch (Exception ex)
                        {
                            //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));

                            SaveException(ex, string.Format("SaveMatchStats, MatchId: ", match.Id.ToString()));
                        }

                        //System.Diagnostics.Debug.WriteLine("Match stats complete");

                        #endregion

                        #region Player Stats

                        #region Home players

                        //Home players
                        jPath = "player_stats.localteam.player";

                        var playerStats = token.SelectTokens(jPath);

                        foreach (var statToken in playerStats.Children())
                        {
                            //var fred = statToken.Children();

                            //foreach (var evt in fred.Select(x => x.ToObject<Dictionary<string, string>>()))
                            //{
                            PlayerStat playerStat = new PlayerStat();

                            isValid = Int32.TryParse(statToken.SelectToken("id").ToString(), out APIId);

                            if (isValid)
                            {
                                playerStat.APIId = APIId;

                                Player player = new Player();
                                player = GetPlayerByAPIId(playerStat.APIId);

                                if (player != null)
                                {
                                    playerStat.Assists = string.IsNullOrWhiteSpace(statToken.SelectToken("assists").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("assists").ToString());
                                    playerStat.FoulsCommitted = string.IsNullOrWhiteSpace(statToken.SelectToken("fouls_committed").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("fouls_committed").ToString());
                                    playerStat.FoulsDrawn = string.IsNullOrWhiteSpace(statToken.SelectToken("fouls_drawn").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("fouls_drawn"));
                                    playerStat.Goals = string.IsNullOrWhiteSpace(statToken.SelectToken("goals").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("goals").ToString());
                                    playerStat.MatchId = match.Id;
                                    playerStat.Offsides = string.IsNullOrWhiteSpace(statToken.SelectToken("offsides").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("offsides").ToString());
                                    playerStat.PenaltiesMissed = string.IsNullOrWhiteSpace(statToken.SelectToken("pen_miss").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("pen_miss").ToString());
                                    playerStat.PenaltiesScored = string.IsNullOrWhiteSpace(statToken.SelectToken("pen_score").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("pen_score").ToString());
                                    playerStat.PlayerId = player.Id;
                                    playerStat.PositionX = string.IsNullOrWhiteSpace(statToken.SelectToken("posx").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("posx").ToString());
                                    playerStat.PositionY = string.IsNullOrWhiteSpace(statToken.SelectToken("posy").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("posy").ToString());
                                    playerStat.RedCards = string.IsNullOrWhiteSpace(statToken.SelectToken("redcards").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("redcards").ToString());
                                    playerStat.Saves = string.IsNullOrWhiteSpace(statToken.SelectToken("saves").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("saves").ToString());
                                    playerStat.ShotsOnGoal = string.IsNullOrWhiteSpace(statToken.SelectToken("shots_on_goal").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("shots_on_goal").ToString());
                                    playerStat.TotalShots = string.IsNullOrWhiteSpace(statToken.SelectToken("shots_total").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("shots_total").ToString());
                                    playerStat.YellowCards = string.IsNullOrWhiteSpace(statToken.SelectToken("yellowcards").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("yellowcards").ToString());

                                    try
                                    {
                                        SavePlayerStats(playerStat);
                                    }
                                    catch (Exception ex)
                                    {
                                        //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                        SaveException(ex, string.Format("SavePlayerStats - Home, PlayerAPIId: ", player.APIPlayerId.ToString()));
                                    }
                                }
                            }
                            //}
                        }

                    //System.Diagnostics.Debug.WriteLine("Home player stats complete");

                    #endregion

                    #region Away players

                    //Home players
                    jPath = "player_stats.visitorteam.player";

                    playerStats = token.SelectTokens(jPath);

                    foreach (var statToken in playerStats.Children())
                    {
                        //var fred = statToken.Children();

                        //foreach (var evt in fred.Select(x => x.ToObject<Dictionary<string, string>>()))
                        //{
                        PlayerStat playerStat = new PlayerStat();

                        isValid = Int32.TryParse(statToken.SelectToken("id").ToString(), out APIId);

                        if (isValid)
                        {
                            playerStat.APIId = APIId;

                            Player player = new Player();
                            player = GetPlayerByAPIId(playerStat.APIId);

                            if (player != null)
                            {
                                playerStat.Assists = string.IsNullOrWhiteSpace(statToken.SelectToken("assists").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("assists").ToString());
                                playerStat.FoulsCommitted = string.IsNullOrWhiteSpace(statToken.SelectToken("fouls_committed").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("fouls_committed").ToString());
                                playerStat.FoulsDrawn = string.IsNullOrWhiteSpace(statToken.SelectToken("fouls_drawn").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("fouls_drawn"));
                                playerStat.Goals = string.IsNullOrWhiteSpace(statToken.SelectToken("goals").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("goals").ToString());
                                playerStat.MatchId = match.Id;
                                playerStat.Offsides = string.IsNullOrWhiteSpace(statToken.SelectToken("offsides").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("offsides").ToString());
                                playerStat.PenaltiesMissed = string.IsNullOrWhiteSpace(statToken.SelectToken("pen_miss").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("pen_miss").ToString());
                                playerStat.PenaltiesScored = string.IsNullOrWhiteSpace(statToken.SelectToken("pen_score").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("pen_score").ToString());
                                playerStat.PlayerId = player.Id;
                                playerStat.PositionX = string.IsNullOrWhiteSpace(statToken.SelectToken("posx").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("posx").ToString());
                                playerStat.PositionY = string.IsNullOrWhiteSpace(statToken.SelectToken("posy").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("posy").ToString());
                                playerStat.RedCards = string.IsNullOrWhiteSpace(statToken.SelectToken("redcards").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("redcards").ToString());
                                playerStat.Saves = string.IsNullOrWhiteSpace(statToken.SelectToken("saves").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("saves").ToString());
                                playerStat.ShotsOnGoal = string.IsNullOrWhiteSpace(statToken.SelectToken("shots_on_goal").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("shots_on_goal").ToString());
                                playerStat.TotalShots = string.IsNullOrWhiteSpace(statToken.SelectToken("shots_total").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("shots_total").ToString());
                                playerStat.YellowCards = string.IsNullOrWhiteSpace(statToken.SelectToken("yellowcards").ToString()) ? (byte)0 : Convert.ToByte(statToken.SelectToken("yellowcards").ToString());

                                try
                                {
                                    SavePlayerStats(playerStat);
                                }
                                catch (Exception ex)
                                {
                                    //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                    SaveException(ex, string.Format("SavePlayerStats - Away, PlayerAPIId: ", player.APIPlayerId.ToString()));
                                }
                            }
                        }
                        //}
                    }

                    //System.Diagnostics.Debug.WriteLine("Away players stats complete");

                    #endregion

                    #endregion

                    #region Events

                    //Retrieve lastupdateId
                    int lastUpdateId = GetLatestEventId(match.Id);

                    //    JObject feed = JObject.Parse(s);
                    //    string id = string.Empty;
                    //    int eventId = 0;

                    //    try
                    //    {
                    //        isValid = Int32.TryParse(feed.SelectToken("comments.id").ToString(), out eventId);
                    //    }
                    //    catch
                    //    {
                    //        isValid = false;
                    //    }

                    #region Remove
                    //if (isValid)
                    //    {
                    //        if (eventId >= lastUpdateId)
                    //        {
                    //            Event commEvent = new Event();

                    //            string important = feed.SelectToken("comments.important").ToString();
                    //            string isgoal = feed.SelectToken("comments.isgoal").ToString();
                    //            string minute = feed.SelectToken("comments.minute").ToString();

                    //            if (minute.Contains('\''))
                    //            {
                    //                minute = minute.Remove(minute.Length - 1, 1);
                    //            }

                    //            string comment = feed.SelectToken("comments.comment").ToString();

                    //            commEvent.Important = important == "True" ? true : false;

                    //            if (isgoal == "True")
                    //            {
                    //                commEvent.Goal = true;

                    //                //Get player and team from comment
                    //                string playerName = string.Empty;
                    //                string teamName = string.Empty;
                    //                string score = string.Empty;

                    //                GetPlayerAndTeamFromComment(EventType.Goal, comment, out playerName, out teamName, out score);

                    //                Goal goal = new Goal();

                    //                goal.OwnGoal = false; //TODO - find a way of checking if own goal
                    //                goal.Penalty = false; //TODO - find a way of checking if penalty     

                    //                isValid = Byte.TryParse(minute, out numberByte);

                    //                if (isValid)
                    //                    goal.Minute = numberByte;
                    //                else
                    //                    goal.Minute = 0;

                    //                goal.APIId = APIId;
                    //                goal.SummaryId = summary.Id;

                    //                var player = GetPlayerByName(playerName);

                    //                if (player != null)
                    //                {
                    //                    goal.PlayerName = player.Name;
                    //                    goal.APIPlayerId = player.APIPlayerId;
                    //                }
                    //                else
                    //                {
                    //                    goal.PlayerName = player.Name;
                    //                    goal.APIPlayerId = 0;
                    //                }

                    //                if (teamName.Trim().ToLower() == homeTeam.Name.Trim().ToLower())
                    //                {
                    //                    goal.IsHomeTeam = true;
                    //                }
                    //                else
                    //                {
                    //                    goal.IsHomeTeam = false;
                    //                }

                    //                retMsg = SaveGoal(goal);
                    //            }
                    //            else
                    //            {
                    //                commEvent.Goal = false;
                    //            }

                    //            byte min = 0;

                    //            isValid = Byte.TryParse(minute, out min);

                    //            commEvent.Minute = min;
                    //            commEvent.Comment = comment;
                    //            commEvent.APIId = eventId;
                    //            commEvent.MatchId = match.Id;
                    //            commEvent.Score = GetMatchScore(match.Id, homeTeam.Name, awayTeam.Name);

                    //            byte homeRating = 0;
                    //            byte awayRating = 0;

                    //            GetTeamRatings(match.Id, out homeRating, out awayRating);

                    //            commEvent.HomeTeamMatchRating = homeRating;
                    //            commEvent.AwayTeamMatchRating = awayRating;

                    //            SaveEvent(commEvent);
                    //        }
                    //    }
                    #endregion

                    //else
                    //    {

                    string id = string.Empty;
                    int eventId = 0;

                    jPath = "comments";

                    var events = token.SelectTokens(jPath);

                    foreach (var childToken in events.Children())
                    {
                        var jeff = childToken.Children();

                        var evtId = childToken.SelectToken("id").ToString();
                        var evtImportant = childToken.SelectToken("important").ToString();
                        var evtIsGoal = childToken.SelectToken("isgoal").ToString();
                        var evtMinute = childToken.SelectToken("minute").ToString();
                        var evtComment = childToken.SelectToken("comment").ToString();

                        try
                        {
                            isValid = Int32.TryParse(evtId, out eventId);

                            //Check if event exists                           
                            if (!isValid)
                                eventId = 0;

                            if (eventId <= lastUpdateId)
                            {
                                break;
                            }

                            Event commEvent = new Event();

                            string important = evtImportant;
                            string isgoal = evtIsGoal;
                            string minute = evtMinute.Replace("'", "");

                            if (minute.Contains('-'))
                            {
                                minute = "0";
                            }
                            else if (minute.Contains('+'))
                            {
                                string[] minArr = minute.Split('+');
                                byte minByte0 = Convert.ToByte(minArr[0].ToString());
                                byte minByte1 = Convert.ToByte(minArr[1].ToString());

                                minute = (minByte0 + minByte1).ToString();
                            }

                            string comment = evtComment;

                            commEvent.Important = important == "True" ? true : false;

                            if (isgoal == "1")
                            {
                                commEvent.Goal = true;

                                //Get player and team from comment
                                string playerName = string.Empty;
                                string teamName = string.Empty;
                                string score = string.Empty;

                                GetPlayerAndTeamFromComment(EventType.Goal, comment, out playerName, out teamName, out score);

                                Goal goal = new Goal();

                                goal.OwnGoal = false; //TODO - find a way of checking if own goal
                                goal.Penalty = false; //TODO - find a way of checking if penalty     

                                isValid = Byte.TryParse(minute, out numberByte);

                                if (isValid)
                                    goal.Minute = numberByte;
                                else
                                    goal.Minute = 0;

                                goal.APIId = APIId;
                                goal.SummaryId = summary.Id;

                                var player = GetPlayerByNameAndTeam(playerName, teamName);

                                if (player != null)
                                {
                                    goal.PlayerName = player.Name;
                                    goal.APIPlayerId = player.APIPlayerId;
                                }
                                else
                                {
                                    goal.PlayerName = player.Name;
                                    goal.APIPlayerId = 0;
                                }

                                if (teamName.Trim().ToLower() == homeTeam.Name.Trim().ToLower())
                                {
                                    goal.IsHomeTeam = true;
                                }
                                else
                                {
                                    goal.IsHomeTeam = false;
                                }

                                retMsg = SaveGoal(goal);
                            }
                            else
                            {
                                commEvent.Goal = false;
                            }

                            commEvent.Minute = !string.IsNullOrWhiteSpace(minute) ? Convert.ToByte(minute) : (byte)0;
                            commEvent.Comment = comment;
                            commEvent.APIId = eventId;
                            commEvent.MatchId = match.Id;
                            commEvent.Score = GetMatchScore(match.Id, homeTeam.Name, awayTeam.Name);

                            byte homeRating = 0;
                            byte awayRating = 0;

                            GetTeamRatings(match.Id, out homeRating, out awayRating);

                            commEvent.HomeTeamMatchRating = homeRating;
                            commEvent.AwayTeamMatchRating = awayRating;

                            SaveEvent(commEvent);
                        }
                        catch (Exception ex)
                        {
                                    //System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                                    SaveException(ex, string.Format("SaveEvent, MatchId: ", match.Id.ToString()));
                        }
                    }
                            //}//
                        //}

                        //System.Diagnostics.Debug.WriteLine("Events complete");
                        #endregion
                }

                    //}
                    //else
                    //{
                    //    retMsg = "Error: " + string.Format("Status code == {0}, Content length == {1}", webResponse.StatusCode, webResponse.ContentLength);
                    //}
                //}// uncomment
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                SaveException(ex, string.Format("GetCommentaries, no match id"));
            }

            return retMsg;
        }

        private static string GetFeed()
        {
            var jsonObject = new JObject();

            jsonObject.Add("Updated", DateTime.Now);

            dynamic feed = jsonObject;
            feed.Events = new JArray() as dynamic;
           
            var todaysMatches = GetTodayMatches();

            string homeTeamComment = string.Empty;
            string awayTeamComment = string.Empty;

            foreach (var match in todaysMatches)
            {
                var matchDetails = GetMatchByAPIId(match.APIId);
                var latestEvents = GetLatestEvents(matchDetails.Id);

                foreach (var evt in latestEvents)
                {
                    dynamic feedEvent = new JObject();

                    GetComment(matchDetails.Id, matchDetails.HomeTeamAPIId, matchDetails.AwayTeamAPIId, evt.Comment, out homeTeamComment, out awayTeamComment);

                    feedEvent.EventComment = evt.Comment;
                    feedEvent.Score = evt.Score;
                    feedEvent.Minute = evt.Minute;
                    feedEvent.EventAPIId = evt.APIId;
                    feedEvent.MatchAPIId = matchDetails.APIId;
                    feedEvent.HomeComment = homeTeamComment;
                    feedEvent.HomeTeamAPIId = matchDetails.HomeTeamAPIId;
                    feedEvent.AwayComment = awayTeamComment;
                    feedEvent.AwayTeamAPIId = matchDetails.AwayTeamAPIId;

                    feed.Events.Add(feedEvent);
                }               
            }
            
            return feed.ToString();
        }

        #region Save
        private static string SaveMatch(Match updatedRecord)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
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
                    //recordToUpdate.AwayTeamId = updatedRecord.AwayTeamId;
                    recordToUpdate.Date = updatedRecord.Date;
                    recordToUpdate.EndDate = updatedRecord.EndDate;
                    recordToUpdate.HomeTeamAPIId = updatedRecord.HomeTeamAPIId;
                    //recordToUpdate.HomeTeamId = updatedRecord.HomeTeamId;
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
        }

        private static string SaveMatchStats(Stat updatedRecord)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
                if (updatedRecord == null)
                {
                    return Res.Resources.NotFound;
                }

                //Update record
                var recordToUpdate = context.Stats.Where(x => x.Id == updatedRecord.Id).FirstOrDefault();

                if (recordToUpdate == null)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;
                    updatedRecord.MatchRating = 3;

                    context.Stats.Add(updatedRecord);
                    Id = updatedRecord.Id;
                }
                else
                {
                    recordToUpdate.Active = updatedRecord.Active;
                    recordToUpdate.AwayTeamCorners = updatedRecord.AwayTeamCorners;
                    recordToUpdate.AwayTeamFouls = updatedRecord.AwayTeamFouls;
                    recordToUpdate.AwayTeamOffsides = updatedRecord.AwayTeamOffsides;
                    recordToUpdate.AwayTeamOnGoalShots = updatedRecord.AwayTeamOnGoalShots;
                    recordToUpdate.AwayTeamPossessionTime = updatedRecord.AwayTeamPossessionTime;
                    recordToUpdate.AwayTeamRedCards = updatedRecord.AwayTeamRedCards;
                    recordToUpdate.AwayTeamSaves = updatedRecord.AwayTeamSaves;
                    recordToUpdate.AwayTeamTotalShots = updatedRecord.AwayTeamTotalShots;
                    recordToUpdate.AwayTeamYellowCards = updatedRecord.AwayTeamYellowCards;
                    recordToUpdate.DateUpdated = DateTime.Now;
                    recordToUpdate.Deleted = false;
                    recordToUpdate.HomeTeamCorners = updatedRecord.HomeTeamCorners;
                    recordToUpdate.HomeTeamFouls = updatedRecord.HomeTeamFouls;
                    recordToUpdate.HomeTeamOffsides = updatedRecord.HomeTeamOffsides;
                    recordToUpdate.HomeTeamOnGoalShots = updatedRecord.HomeTeamOnGoalShots;
                    recordToUpdate.HomeTeamPossessionTime = updatedRecord.HomeTeamPossessionTime;
                    recordToUpdate.HomeTeamRedCards = updatedRecord.HomeTeamRedCards;
                    recordToUpdate.HomeTeamSaves = updatedRecord.HomeTeamSaves;
                    recordToUpdate.HomeTeamTotalShots = updatedRecord.HomeTeamTotalShots;
                    recordToUpdate.HomeTeamYellowCards = updatedRecord.HomeTeamYellowCards;
                    //recordToUpdate.Match = updatedRecord.Match;
                    recordToUpdate.MatchId = updatedRecord.MatchId;
                    recordToUpdate.UpdatedByUserId = updatedRecord.UpdatedByUserId;
                    recordToUpdate.HomeTeamRating = updatedRecord.HomeTeamRating;
                    recordToUpdate.AwayTeamRating = updatedRecord.AwayTeamRating;
                    recordToUpdate.MatchRating = updatedRecord.MatchRating;

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
        }

        private static string SaveGoal(Goal updatedRecord)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
                if (updatedRecord == null)
                {
                    return Res.Resources.RecordNull;
                }

                //Update record
                var recordToUpdate = context.Goals.Where(x => x.APIId == updatedRecord.APIId && x.Minute == updatedRecord.Minute).FirstOrDefault();

                if (recordToUpdate != null)
                {
                    return Res.Resources.RecordFound;
                }

                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;

                    context.Goals.Add(updatedRecord);
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
        }

        private static string SavePlayerStats(PlayerStat updatedRecord)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
                if (updatedRecord == null)
                {
                    return Res.Resources.NotFound;
                }

                //Update record
                var recordToUpdate = context.PlayerStats.Where(x => x.PlayerId == updatedRecord.PlayerId && x.MatchId == updatedRecord.MatchId).FirstOrDefault();

                if (recordToUpdate == null)
                {
                    //Create record
                    updatedRecord.Id = Guid.NewGuid();
                    updatedRecord.Active = true;
                    updatedRecord.Deleted = false;
                    updatedRecord.DateAdded = DateTime.Now;
                    updatedRecord.DateUpdated = DateTime.Now;
                    updatedRecord.Rating = 3;

                    context.PlayerStats.Add(updatedRecord);
                    Id = updatedRecord.Id;
                }
                else
                {
                    recordToUpdate.Assists = updatedRecord.Assists;
                    recordToUpdate.FoulsCommitted = updatedRecord.FoulsCommitted;
                    recordToUpdate.FoulsDrawn = updatedRecord.FoulsDrawn;
                    recordToUpdate.Goals = updatedRecord.Goals;
                    recordToUpdate.Offsides = updatedRecord.Offsides;
                    recordToUpdate.PenaltiesMissed = updatedRecord.PenaltiesMissed;
                    recordToUpdate.PenaltiesScored = updatedRecord.PenaltiesScored;
                    //recordToUpdate.Player = updatedRecord.Player;
                    recordToUpdate.PlayerId = updatedRecord.PlayerId;
                    recordToUpdate.PositionX = updatedRecord.PositionX;
                    recordToUpdate.PositionY = updatedRecord.PositionY;
                    recordToUpdate.RedCards = updatedRecord.RedCards;
                    recordToUpdate.Saves = updatedRecord.Saves;
                    recordToUpdate.ShotsOnGoal = updatedRecord.ShotsOnGoal;
                    recordToUpdate.TotalShots = updatedRecord.TotalShots;
                    recordToUpdate.YellowCards = updatedRecord.YellowCards;
                    recordToUpdate.Active = updatedRecord.Active;
                    recordToUpdate.DateUpdated = DateTime.Now;
                    recordToUpdate.Deleted = false;
                    //recordToUpdate.Match = updatedRecord.Match;
                    recordToUpdate.MatchId = updatedRecord.MatchId;
                    recordToUpdate.UpdatedByUserId = updatedRecord.UpdatedByUserId;
                    recordToUpdate.Rating = updatedRecord.Rating;

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
                    //System.Diagnostics.Debug.WriteLine(string.Format("SavePlayerStats: {0}, Message: {1}, Player Id: {2}", e.InnerException, e.Message, updatedRecord.APIId));
                            
                    return string.Format("Error: {0}", e.InnerException.ToString());
                }
            }          
        }

        private static string SaveEvent(Event updatedRecord)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
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
        }

        private static string SavePlayer(Player updatedRecord, Guid teamId)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
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
        }

        private static string SavePlayerToMatchLineup(Lineup updatedRecord)
        {
            Guid Id = Guid.Empty;

            using (EFDbContext context = new EFDbContext())
            {
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
                      
                        context.Lineups.Add(updatedRecord);
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
        }

        private static string SaveUpdateHistory(UpdateHistory updatedRecord)
        {
            bool isNewRecord = false;
            Guid Id;

            using (EFDbContext context = new EFDbContext())
            {
                if (updatedRecord != null)
                {                 
                        //Update record
                    var recordToUpdate = context.UpdateHistory.Where(x => x.MatchAPIId == updatedRecord.MatchAPIId).FirstOrDefault();

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
        }

        private static void SaveException(Exception exception, string additionalInfo)
        {
            SiteException record = new SiteException();

            using (EFDbContext context = new EFDbContext())
            {
                if (exception != null)
                {
                    //Create record
                    record.HResult =exception.HResult.ToString();
                    record.InnerException = record.InnerException != null ? exception.InnerException.ToString() : string.Empty;
                    record.Message = string.Format("Additional Info:{0}, Exception: {1} ", additionalInfo, exception.Message);
                    record.Source = exception.Source;
                    record.StackTrace = record.StackTrace != null ? string.Format("Stack Trace: {0}", exception.StackTrace.ToString()) : string.Empty;
                    record.TargetSite = record.TargetSite != null ? exception.TargetSite.ToString() : string.Empty;
                    record.DateAdded = DateTime.Now;

                    context.SiteException.Add(record);
                }

                try
                {
                    context.SaveChanges();
                }
                catch { }
            }
        }

        private static void SaveBroadcastFeed(string message, string ipAddress)
        {
            BroadcastFeed record = new BroadcastFeed();

             using (EFDbContext context = new EFDbContext())
            {
                 //Create record
                 record.Message = message;
                 record.IPAddress = ipAddress;
                 record.DateAdded = DateTime.Now;

                 context.BroadcastFeed.Add(record);
               
                try
                {
                    context.SaveChanges();
                }
                catch { }
             }
        }
              
        #endregion

        #region Get

        private static Stat GetMatchStatsByAPIId(Guid matchGuid)
        {
            Stat stat = new Stat();

            using (EFDbContext context = new EFDbContext())
            {
                stat = context.Stats.Where(x => (x.MatchId == matchGuid)).FirstOrDefault();
            }

            return stat;
        }

        private static UpdateHistory GetUpdateHistoryByMatchAPIId(int id)
        {
            UpdateHistory hist = new UpdateHistory();

            using (EFDbContext context = new EFDbContext())
            {
                hist = context.UpdateHistory.Where(x => (x.MatchAPIId == id)).FirstOrDefault();
            }

            return hist;
        }

        private static Goal GetGoalByAPIId(int id, byte minute)
        {
            Goal goal = new Goal();

            using (EFDbContext context = new EFDbContext())
            {
                goal = context.Goals.Where(x => (x.APIId == id) && (x.Minute == minute)).FirstOrDefault();
            }

            return goal;
        }

        private static Player GetPlayerByAPIId(int id)
        {
            Player player = new Player();

            using (EFDbContext context = new EFDbContext())
            {
                player = context.Players.Where(x => (x.APIPlayerId == id)).FirstOrDefault();
            }

            return player;
        }

        private static Player GetPlayerByName(string name)
        {
            Player player = new Player();

            using (EFDbContext context = new EFDbContext())
            {
                player = context.Players.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            }

            return player;
        }

        private static Player GetPlayerByNameAndTeam(string name, string teamName)
        {
            IList<Player> players = new List<Player>();
            Team team = new Team();
            string lastname = string.Empty;

            string[] allnames = name.Split(' ');
            int count = allnames.Count();

            if (count > 0)
            {
                lastname = allnames[count - 1];

                using (EFDbContext context = new EFDbContext())
                {
                    team = context.Teams.Where(x => x.Name.ToLower().Trim() == teamName.ToLower().Trim()).FirstOrDefault();

                    if (team != null)
                        players = team.Players.Where(x => x.Name.Any(y => x.Name.Contains(lastname))).ToList();
                }
            }
           
            return players.FirstOrDefault();
        }

        private static PlayerStat GetPlayerStatsByIdAndMatch(Guid playerId, Guid matchId)
        {
            PlayerStat playerStat = new PlayerStat();

            using (EFDbContext context = new EFDbContext())
            {
                playerStat = context.PlayerStats.Where(x => (x.PlayerId == playerId) && (x.MatchId == matchId)).FirstOrDefault();
            }

            return playerStat;
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

        private static Summary GetSummaryByMatchId(Guid id)
        {
            Summary summary = new Summary();

            using (EFDbContext context = new EFDbContext())
            {
                summary = context.Summaries.Where(x => (x.MatchId == id)).FirstOrDefault();
            }

            return summary;
        }

        private static Match GetMatchByAPIId(int id)
        {
            Match match = new Match();

            using (EFDbContext context = new EFDbContext())
            {
                match = context.Matches.Where(x => (x.APIId == id)).FirstOrDefault();
            }

            return match;
        }

        public static IList<int> GetLiveMatches()
        {
            IList<int> liveMatches = new List<int>();

            using (EFDbContext context = new EFDbContext())
            {
                foreach(var match in context.MatchesToday)
                {
                    liveMatches.Add(match.APIId);
                }
            }

            return liveMatches;
        }

        private static int GetLatestEventId(Guid id)
        {
            int retInt = 0;

            using (EFDbContext context = new EFDbContext())
            {
                var evt = context.Events.Where(x => (x.MatchId == id)).OrderByDescending(x => x.DateAdded).FirstOrDefault();

                if (evt != null)
                    retInt = evt.APIId;
            }    

            return retInt;
        }

        private static IList<Event> GetLatestEvents(Guid id)
        {
            IList<Event> latestEvents = new List<Event>();

            using (EFDbContext context = new EFDbContext())
            {
                latestEvents = context.Events.Where(x => (x.MatchId == id)).OrderByDescending(x => x.APIId).Take(5).ToList();
            }

            return latestEvents;
        }

        private static IList<MatchesToday> GetTodayMatches()
        {
            IList<MatchesToday> matchesToday = new List<MatchesToday>();

            using (EFDbContext context = new EFDbContext())
            {
                matchesToday = context.MatchesToday.ToList();
            }

            return matchesToday;
        }

        private static void GetTeamRatings(Guid matchId, out byte homeRating, out byte awayRating)
        {
            homeRating = 0;
            awayRating = 0;

            var matchstat = GetMatchStatsByAPIId(matchId);

            if (matchstat != null)
            {
                decimal homeCalcRating = matchstat.HomeTeamRating;
                decimal awayCalcRating = matchstat.AwayTeamRating;

                var summary = GetSummaryByMatchId(matchId);

                int homeGoals = 0;
                int awayGoals = 0;

                if (summary != null)
                {
                    using (EFDbContext context = new EFDbContext())
                    {
                        homeGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == true)).Count();
                        awayGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == false)).Count();
                    }
                }

                #region Home team
                decimal shotsFactor = 0;

                if (matchstat.HomeTeamTotalShots > 2)
                {
                    var percentageOnTarget = (decimal)matchstat.HomeTeamOnGoalShots / (decimal)matchstat.HomeTeamTotalShots * (decimal)100;

                    if (percentageOnTarget >= 0 && percentageOnTarget <= 25)
                    {
                        shotsFactor = -0.5M;
                    }
                    else if (percentageOnTarget > 25 && percentageOnTarget <= 50)
                    {
                        shotsFactor = -0.3M;
                    }
                    else if (percentageOnTarget > 50 && percentageOnTarget <= 75)
                    {
                        shotsFactor = 0.3M;
                    }
                    else if (percentageOnTarget > 75 && percentageOnTarget <= 100)
                    {
                        shotsFactor = 0.5M;
                    }
                }

                //Possession
                byte possession = 0;
                decimal possessionFactor = 0;

                if (!string.IsNullOrWhiteSpace(matchstat.HomeTeamPossessionTime))
                {
                    var possStr = matchstat.HomeTeamPossessionTime.Remove(matchstat.HomeTeamPossessionTime.Trim().Length - 1, 1);

                    Byte.TryParse(possStr, out possession);
                }

                if (possession > 0)
                {
                    if (possession >= 0 && possession <= 25)
                    {
                        possessionFactor = -0.5M;
                    }
                    else if (possession > 25 && possession <= 50)
                    {
                        possessionFactor = -0.3M;
                    }
                    else if (possession > 50 && possession <= 75)
                    {
                        possessionFactor = 0.3M;
                    }
                    else if (possession > 75 && possession <= 100)
                    {
                        possessionFactor = 0.5M;
                    }
                }

                homeCalcRating = homeCalcRating +
                    (decimal)(awayGoals * -0.5) +
                    (decimal)(matchstat.HomeTeamCorners * 0.1) +
                    (decimal)(matchstat.HomeTeamFouls * -0.1) +
                    (decimal)(matchstat.HomeTeamOffsides * -0.1) +
                    (decimal)(matchstat.HomeTeamRedCards * -0.5) +
                    (decimal)(matchstat.HomeTeamSaves * 0.1) +
                    (decimal)(matchstat.HomeTeamYellowCards * -0.2) +
                    (decimal)(homeGoals * 0.5) +
                    shotsFactor +
                    possessionFactor;

                if (homeCalcRating <= 0)
                {
                    matchstat.HomeTeamRating = 1;
                }
                else
                {
                    matchstat.HomeTeamRating = Convert.ToByte(homeCalcRating);
                }

                homeRating = matchstat.HomeTeamRating;

                #endregion

                #region Away team

                shotsFactor = 0;

                if (matchstat.AwayTeamTotalShots > 2)
                {
                    var percentageOnTarget = (decimal)matchstat.AwayTeamOnGoalShots / (decimal)matchstat.AwayTeamTotalShots * (decimal)100;

                    if (percentageOnTarget >= 0 && percentageOnTarget <= 25)
                    {
                        shotsFactor = -0.5M;
                    }
                    else if (percentageOnTarget > 25 && percentageOnTarget <= 50)
                    {
                        shotsFactor = -0.3M;
                    }
                    else if (percentageOnTarget > 50 && percentageOnTarget <= 75)
                    {
                        shotsFactor = 0.3M;
                    }
                    else if (percentageOnTarget > 75 && percentageOnTarget <= 100)
                    {
                        shotsFactor = 0.5M;
                    }
                }

                //Possession
                possession = 0;
                possessionFactor = 0;

                if (!string.IsNullOrWhiteSpace(matchstat.AwayTeamPossessionTime))
                {
                    var possStr = matchstat.AwayTeamPossessionTime.Remove(matchstat.AwayTeamPossessionTime.Trim().Length - 1, 1);

                    Byte.TryParse(possStr, out possession);
                }

                if (possession > 0)
                {
                    if (possession >= 0 && possession <= 25)
                    {
                        possessionFactor = -0.5M;
                    }
                    else if (possession > 25 && possession <= 50)
                    {
                        possessionFactor = -0.3M;
                    }
                    else if (possession > 50 && possession <= 75)
                    {
                        possessionFactor = 0.3M;
                    }
                    else if (possession > 75 && possession <= 100)
                    {
                        possessionFactor = 0.5M;
                    }
                }

                awayCalcRating = awayCalcRating +
                    (decimal)(awayGoals * -0.5) +
                    (decimal)(matchstat.HomeTeamCorners * 0.1) +
                    (decimal)(matchstat.HomeTeamFouls * -0.1) +
                    (decimal)(matchstat.HomeTeamOffsides * -0.1) +
                    (decimal)(matchstat.HomeTeamRedCards * -0.5) +
                    (decimal)(matchstat.HomeTeamSaves * 0.1) +
                    (decimal)(matchstat.HomeTeamYellowCards * -0.2) +
                    (decimal)(homeGoals * 0.5) +
                    shotsFactor +
                    possessionFactor;

                if (awayCalcRating <= 0)
                {
                    matchstat.AwayTeamRating = 1;
                }
                else
                {
                    matchstat.AwayTeamRating = Convert.ToByte(awayCalcRating);
                }

                awayRating = matchstat.AwayTeamRating;

                #endregion
            }
            else
            {
                matchstat = new Stat();
                matchstat.MatchId = matchId;
                matchstat.HomeTeamRating = 3;
                matchstat.AwayTeamRating = 3;

                homeRating = 3;
                awayRating = 3;
            }
                  
            //update playerstat object with updated rating
            SaveMatchStats(matchstat);
        }

        private static byte GetPlayerRating(Player player, byte goalsConceeded, Guid matchId)
        {
            byte playerRating = 0;

            if (player != null)
            {
                var plstat = GetPlayerStatsByIdAndMatch(player.Id, matchId);
                
                if (plstat != null)
                {
                    decimal calcRating = plstat.Rating;

                    switch (player.Position)
                    {
                        case "G":
                            //saves +0.2
                            //goals conceeded -0.4
                            //fouls committed -0.1
                            //yellow cards -0.3
                            //red card -1
                            //penalties saved +0.3

                            calcRating = calcRating +
                                 (decimal)(goalsConceeded * -0.4) +
                                (decimal)(plstat.Saves * 0.2) + 
                                (decimal)(plstat.FoulsCommitted * -0.1) +
                                (decimal)(plstat.FoulsDrawn * 0.1) +
                                (decimal)(plstat.YellowCards * -0.3) + 
                                (decimal)(plstat.RedCards * -1);
                            //penalties saved

                            break;
                        case "D":
                            //goals conceeded -0.4
                            //fouls committed -0.1
                            //fouls drawn +0.1
                            //yellow cards  -0.3
                            //red card -1
                            //penalties scored +0.5
                            //penalties missed -0.3
                            //goals scored +0.5
                            //assists +0.3

                            calcRating = calcRating +
                                 (decimal)(goalsConceeded * -0.4) +
                                (decimal)(plstat.FoulsCommitted * -0.1) +
                                (decimal)(plstat.FoulsDrawn * 0.1) +
                                (decimal)(plstat.YellowCards * -0.3) + 
                                (decimal)(plstat.RedCards * -1) +
                                (decimal)(plstat.PenaltiesScored * 0.5) +
                                (decimal)(plstat.PenaltiesMissed * -0.3) +
                                (decimal)(plstat.Goals * 0.5) +
                                (decimal)(plstat.Assists * 0.3);

                            break;
                        case "M":
                            //goals conceeded -0.2
                            //fouls committed -0.1
                            //fouls drawn +0.1
                            //yellow cards  -0.3
                            //red card -1
                            //penalties scored +0.5
                            //penalties missed -0.3
                            //goals scored +0.5
                            //assists +0.3
                            //total shots +/-
                            //shots on goal +/-
                            //If total shots > 2
                            //0-25% = -0.3, 25-50% = -0.1, 50-75% = +0.1, 75-100% = +0.3
                            //offsides -0.1

                           
                            decimal shotsFactor = 0;

                            if (plstat.TotalShots > 2)
                            {
                                var percentageOnTarget = (decimal)plstat.ShotsOnGoal / (decimal)plstat.TotalShots * (decimal)100;

                                if (percentageOnTarget >= 0 && percentageOnTarget <= 25)
                                {
                                    shotsFactor = -0.3M;
                                }
                                else if (percentageOnTarget > 25 && percentageOnTarget <= 50)
                                {
                                    shotsFactor = -0.1M;
                                }
                                else if (percentageOnTarget > 50 && percentageOnTarget <= 75)
                                {
                                    shotsFactor = 0.1M;
                                }
                                else if (percentageOnTarget > 75 && percentageOnTarget <= 100)
                                {
                                    shotsFactor = 0.3M;
                                }
                            }

                            calcRating = calcRating +
                                 (decimal)(goalsConceeded * -0.2) +
                                (decimal)(plstat.FoulsCommitted * -0.1) +
                                (decimal)(plstat.FoulsDrawn * 0.1) +
                                (decimal)(plstat.YellowCards * -0.3) +
                                (decimal)(plstat.RedCards * -1) +
                                (decimal)(plstat.PenaltiesScored * 0.5) +
                                (decimal)(plstat.PenaltiesMissed * -0.3) +
                                (decimal)(plstat.Goals * 0.5) +
                                (decimal)(plstat.Assists * 0.3) +
                                (decimal)(plstat.Offsides * -0.1) +
                                shotsFactor;

                            break;
                        case "F":
                            //goals conceeded -0.1
                            //fouls committed -0.1
                            //fouls drawn +0.1
                            //yellow cards  -0.3
                            //red card -1
                            //penalties scored +0.5
                            //penalties missed -0.3
                            //assists +0.3
                            //goals scored +0.5
                            //total shots +/-
                            //shots on goal +/-
                            //If total shots > 2
                            //0-25% = -0.5, 25-50% = -0.25, 50-75% = +0.25, 75-100% = +0.5
                            //offsides -0.1

                            shotsFactor = 0;

                            if (plstat.TotalShots > 2)
                            {
                                var percentageOnTarget = (decimal)plstat.ShotsOnGoal / (decimal)plstat.TotalShots * (decimal)100;

                                if (percentageOnTarget >= 0 && percentageOnTarget <= 25)
                                {
                                    shotsFactor = -0.5M;
                                }
                                else if (percentageOnTarget > 25 && percentageOnTarget <= 50)
                                {
                                    shotsFactor = -0.3M;
                                }
                                else if (percentageOnTarget > 50 && percentageOnTarget <= 75)
                                {
                                    shotsFactor = 0.3M;
                                }
                                else if (percentageOnTarget > 75 && percentageOnTarget <= 100)
                                {
                                    shotsFactor = 0.5M;
                                }
                            }

                            calcRating = calcRating +
                                (decimal)(goalsConceeded * -0.1) +
                                (decimal)(plstat.FoulsCommitted * -0.1) +
                                (decimal)(plstat.FoulsDrawn * 0.1) +
                                (decimal)(plstat.YellowCards * -0.3) +
                                (decimal)(plstat.RedCards * -1) +
                                (decimal)(plstat.PenaltiesScored * 0.5) +
                                (decimal)(plstat.PenaltiesMissed * -0.3) +
                                (decimal)(plstat.Goals * 0.5) +
                                (decimal)(plstat.Assists * 0.3) +
                                (decimal)(plstat.Offsides * -0.1) +
                                shotsFactor;

                            break;

                        default:
                            break;
                    }

                    //update playerstat object with updated rating
                    if (calcRating <= 0)
                        plstat.Rating = 1;
                    else
                        plstat.Rating = Convert.ToByte(calcRating);

                    SavePlayerStats(plstat);
                }
            }

            return playerRating;
        }

        private static string GetMatchScore(Guid matchId, string homeTeam, string awayTeam)
        {
            string score = string.Empty;
            var summary = GetSummaryByMatchId(matchId);

            int homeGoals = 0;
            int awayGoals = 0;

            if (summary != null)
            {
                using (EFDbContext context = new EFDbContext())
                {
                    homeGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == true)).Count();
                    awayGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == false)).Count();
                }               
            }

            score = string.Format("{0} {1} - {2} {3}", homeTeam, homeGoals, awayGoals, awayTeam);

            return score;
        }

        private static void GetComment(Guid matchId, int homeTeamAPIId, int awayTeamAPIId, string feedComment, out string homeTeamComment, out string awayTeamComment)
        {
            homeTeamComment = string.Empty;
            awayTeamComment = string.Empty;
           
           if (feedComment.StartsWith("Attempt blocked."))//Player
           {
               feedComment = feedComment.Replace("Attempt blocked.", "").Trim();
  
               GeneratePlayerComment(
                   matchId, 
                   homeTeamAPIId, 
                   awayTeamAPIId, 
                   feedComment, 
                   false,
                   CommentType.Player, 
                   EventType.AttemptBlocked, 
                   out homeTeamComment, 
                   out awayTeamComment);
           }
           else if (feedComment.StartsWith("Attempt missed."))//Player
           {
                feedComment = feedComment.Replace("Attempt missed.", "").Trim();

               if (feedComment.Contains("is too high"))
               {
                   GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.AttemptMissedTooHigh,
                       out homeTeamComment,
                       out awayTeamComment);
               }
               else if (feedComment.Contains("is high and wide"))
               {
                   GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.AttemptMissedHighAndWide,
                       out homeTeamComment,
                       out awayTeamComment);
               }
               else if (feedComment.Contains("misses to the"))
               {
                   GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.AttemptMissesToRightOrLeft,
                       out homeTeamComment,
                       out awayTeamComment);
               }
               else if (feedComment.Contains("just a bit too high"))
               {
                   GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.AttemptMissesJustABitTooHigh,
                       out homeTeamComment,
                       out awayTeamComment);
               }
               else
               {
                   GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.AttemptMissed,
                       out homeTeamComment,
                       out awayTeamComment);
               }
           }
            else if (feedComment.StartsWith("Attempt saved."))//Player attacker/GK
            {
                feedComment = feedComment.Replace("Attempt saved.", "").Trim();

                GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.AttemptSaved,
                       out homeTeamComment,
                       out awayTeamComment);
            }
           else if (feedComment.Contains("is shown the yellow card."))//Player
            {
                GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.YellowCard,
                       out homeTeamComment,
                       out awayTeamComment);
            }
           else if (feedComment.Contains("is shown the red card."))//Player
            {
                GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.RedCard,
                       out homeTeamComment,
                       out awayTeamComment);
            }
           else if (feedComment.StartsWith("Corner,"))//Player or Match
            {
               feedComment = feedComment.Replace("Corner, ", "").Trim();
 
               GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       true,
                       CommentType.Player,
                       EventType.Corner,
                       out homeTeamComment,
                       out awayTeamComment);
            }
            else if (feedComment.StartsWith("Delay in match"))//Match
            {
               GenerateMatchComment(
                   matchId, 
                   homeTeamAPIId, 
                   awayTeamAPIId, 
                   feedComment,
                   CommentType.Match, 
                   EventType.Delay, 
                   out homeTeamComment,
                   out awayTeamComment);
            }
           else if (feedComment.StartsWith("Delay over"))//Match
            {
                GenerateMatchComment(
                   matchId,
                   homeTeamAPIId,
                   awayTeamAPIId,
                   feedComment,
                   CommentType.Match,
                   EventType.DelayEnds,
                   out homeTeamComment,
                   out awayTeamComment);
            }
           else if (feedComment.StartsWith("First Half begins"))//Team
            {
               GenerateTeamComment(
                   matchId,
                   homeTeamAPIId,
                   awayTeamAPIId,
                   feedComment,
                   CommentType.Team,
                   EventType.FirstHalfBegins,
                   out homeTeamComment,
                   out awayTeamComment);
            }
           else if (feedComment.StartsWith("First Half ends"))//Team
            {
                feedComment = feedComment.Replace("First Half ends,", "").Trim();

                GenerateTeamComment(
                   matchId,
                   homeTeamAPIId,
                   awayTeamAPIId,
                   feedComment,
                   CommentType.Team,
                   EventType.FirstHalfEnds,
                   out homeTeamComment,
                   out awayTeamComment);
            }
           else if (feedComment.StartsWith("Second Half ends"))//Team
           {
               feedComment = feedComment.Replace("Second Half ends,", "").Trim();

               GenerateTeamComment(
                  matchId,
                  homeTeamAPIId,
                  awayTeamAPIId,
                  feedComment,
                  CommentType.Team,
                  EventType.SecondHalfEnds,
                  out homeTeamComment,
                  out awayTeamComment);
           }
           else if (feedComment.StartsWith("Foul by"))//Player
            {
                feedComment = feedComment.Replace("Foul by ", "").Trim();
 
               GeneratePlayerComment(
                        matchId,
                        homeTeamAPIId,
                        awayTeamAPIId,
                        feedComment,
                        false,
                        CommentType.Player,
                        EventType.Foul,
                        out homeTeamComment,
                        out awayTeamComment);
            }
           else if (feedComment.StartsWith("Goal!"))//Player or Match or team
            {
                feedComment = feedComment.Replace("Goal! ", "").Trim();
 
               GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       true,
                       CommentType.Player,
                       EventType.Goal,
                       out homeTeamComment,
                       out awayTeamComment);
            }
           else if (feedComment.StartsWith("Lineups are announced"))//Team
            {
                GenerateTeamComment(
                    matchId,
                    homeTeamAPIId,
                    awayTeamAPIId,
                    feedComment,
                    CommentType.Team,
                    EventType.LineupsAnnounced,
                    out homeTeamComment,
                    out awayTeamComment);
            }
           else if (feedComment.StartsWith("Offside"))//Player
            {
                feedComment = feedComment.Replace("Offside, ", "").Trim();
 
               GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.Offside,
                       out homeTeamComment,
                       out awayTeamComment);
            }
           else if (feedComment.Contains("hits the bar"))//Player
            {
                GeneratePlayerComment(
                        matchId,
                        homeTeamAPIId,
                        awayTeamAPIId,
                        feedComment,
                        false,
                        CommentType.Player,
                        EventType.HitsTheBar,
                        out homeTeamComment,
                        out awayTeamComment);
            }
           else if ((feedComment.Contains("hits the left post") || feedComment.Contains("hits the right post")))//Player
            {
                GeneratePlayerComment(
                        matchId,
                        homeTeamAPIId,
                        awayTeamAPIId,
                        feedComment,
                        false,
                        CommentType.Player,
                        EventType.HitsThePost,
                        out homeTeamComment,
                        out awayTeamComment);
            }
           else if (feedComment.StartsWith("Second Half begins"))//Match
            {
                feedComment = feedComment.Replace("Second Half begins", "").Trim();
 
               GenerateMatchComment(
                    matchId,
                    homeTeamAPIId,
                    awayTeamAPIId,
                    feedComment,
                    CommentType.Match,
                    EventType.SecondHalfBegins,
                    out homeTeamComment,
                    out awayTeamComment);
            }
           else if (feedComment.StartsWith("Match ends,"))//Match
           {
               feedComment = feedComment.Replace("Match ends,", "").Trim();

               GenerateMatchComment(
                    matchId,
                    homeTeamAPIId,
                    awayTeamAPIId,
                    feedComment,
                    CommentType.Match,
                    EventType.MatchEnds,
                    out homeTeamComment,
                    out awayTeamComment);
           }
           else if (feedComment.StartsWith("Substitution"))//Player/Match/Team
            {
                feedComment = feedComment.Replace("Substitution, ", "").Trim();
 
               GeneratePlayerComment(
                        matchId,
                        homeTeamAPIId,
                        awayTeamAPIId,
                        feedComment,
                        true,
                        CommentType.Player,
                        EventType.Substitution,
                        out homeTeamComment,
                        out awayTeamComment);
            }
            else if (feedComment.StartsWith("Hand ball by"))//Player
            {
                feedComment = feedComment.Replace("Hand ball by ", "").Trim();

                GeneratePlayerComment(
                       matchId,
                       homeTeamAPIId,
                       awayTeamAPIId,
                       feedComment,
                       false,
                       CommentType.Player,
                       EventType.Handball,
                       out homeTeamComment,
                       out awayTeamComment);
            }
           else if (feedComment.Contains("wins a free kick"))//Player
           {
               GeneratePlayerComment(
                        matchId,
                        homeTeamAPIId,
                        awayTeamAPIId,
                        feedComment,
                        true,
                        CommentType.Player,
                        EventType.FreeKick,
                        out homeTeamComment,
                        out awayTeamComment);
           }
           else//Match/Team - Match
           {
               GenerateMatchComment(
                    matchId,
                    homeTeamAPIId,
                    awayTeamAPIId,
                    feedComment,
                    CommentType.Match,
                    EventType.General,
                    out homeTeamComment,
                    out awayTeamComment);
           }
        }

        private static void GeneratePlayerComment
            (
            Guid matchId,
            int homeTeamAPIId, 
            int awayTeamAPIId,
            string feedComment,
            bool isPositiveEvent,
            CommentType commentType, 
            EventType eventType, 
            out string homeTeamComment,
            out string awayTeamComment
            )
        {
            homeTeamComment = string.Empty;
            awayTeamComment = string.Empty;
            string comment = string.Empty;
            string playerName = string.Empty;
            string teamName = string.Empty;
            string score = string.Empty;

            var summary = GetSummaryByMatchId(matchId);

            int homeGoals = 0;
            int awayGoals = 0;
            byte goalsConceeded = 0;

            if (summary != null)
            {
                using (EFDbContext context = new EFDbContext())
                {
                    homeGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == true)).Count();
                    awayGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == false)).Count();
                }
            }

            //Get teams
            var homeTeam = GetTeamByAPIId(homeTeamAPIId);
            var awayTeam = GetTeamByAPIId(awayTeamAPIId);

            //Get player and team from comment
            GetPlayerAndTeamFromComment(eventType, feedComment, out playerName, out teamName, out score);

            var player = GetPlayerByName(playerName);

            if (player == null)
            {
                player = GetPlayerByNameAndTeam(playerName, teamName);
            }

            //Get player position
            var position = string.Empty;
            byte playerRating = 0;
            Position pos = Position.All;

            if (player != null)
            {
                position = player.Position;
            
                switch (position)
                {
                    case "G":
                        pos = Position.Goalkeeper;
                        break;

                    case "D":
                    case "CD-L":
                    case "CD-R":
                    case "LB":
                    case "RB":
                        pos = Position.Defender;
                        break;

                    case "M":
                    case "CM-L":
                    case "CM-R":
                    case "CM":
                    case "LM":
                    case "RM":
                        pos = Position.Midfielder;
                        break;

                    case "F":
                    case "CF-R":
                    case "CF-L":
                    case "LF":
                    case "RF":
                        pos = Position.Forward;
                        break;

                    default:
                        pos = Position.Midfielder;
                        break;
                }

                //Get player rating
                playerRating = GetPlayerRating(player, goalsConceeded, matchId);
            }

            byte commType = (byte)commentType;
            byte evType = (byte)eventType;
            byte perpPos = (byte)Perspective.Positive;
            byte perpNeg = (byte)Perspective.Negative;
            byte playerPosition = (byte)pos;

            IList<string> positiveComments = new List<string>();
            IList<string> negativeComments = new List<string>();
            
            using (EFDbContext context = new EFDbContext())
            {
                if (pos == Position.All)
                {
                    positiveComments = context.Comment
                    .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == perpPos) && (x.PlayerRating == playerRating))
                    .Select(x => x.Text).ToList();

                    negativeComments = context.Comment
                        .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == perpNeg) && (x.PlayerRating == playerRating))
                        .Select(x => x.Text).ToList();
                }
                else
                {
                    positiveComments = context.Comment
                    .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == perpPos) && (x.Position == playerPosition) && (x.PlayerRating == playerRating))
                    .Select(x => x.Text).ToList();

                    negativeComments = context.Comment
                        .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == perpNeg) && (x.Position == playerPosition) && (x.PlayerRating == playerRating))
                        .Select(x => x.Text).ToList();
                }             
            }

            var random = new Random();
            int index = 0;
            string posComment = string.Empty;
            string negComment = string.Empty;

            Dictionary<string, string> placeholders = new Dictionary<string, string>();

            placeholders.Add("PLAYERNAME", playerName);
            placeholders.Add("TEAM", teamName);

            if (positiveComments != null)
            {
                if (positiveComments.Count > 0)
                {
                    index = random.Next(positiveComments.Count);
                    posComment = positiveComments[index];

                    //Replace text placeholders                
                    foreach (KeyValuePair<string, string> kvp in placeholders)
                    {
                        string strToReplace = "{%" + kvp.Key + "%}";

                        if (posComment.Contains(strToReplace))
                        {
                            posComment = posComment.Replace(strToReplace, kvp.Value);
                        }
                    }
                }     
            }            

            if (negativeComments != null)
            {
                if (negativeComments.Count > 0)
                {
                    index = random.Next(negativeComments.Count);
                    negComment = negativeComments[index];

                    //Replace text placeholders
                    foreach (KeyValuePair<string, string> kvp in placeholders)
                    {
                        string strToReplace = "{%" + kvp.Key + "%}";

                        if (negComment.Contains(strToReplace))
                        {
                            negComment = negComment.Replace(strToReplace, kvp.Value);
                        }
                    }
                }         
            }

            //TODO - remove when real comments added
            //posComment = string.Format("Player - EventType: {0}, CommentType: {1}, PlayerPosition: {2}, PlayerRating: {3}, PlayerName: {4}, PlayerTeam: [5}, Perspective: Positive",
            //    eventType.ToString(), commentType.ToString(), position, playerRating.ToString(), playerName, teamName);
            //negComment = string.Format("Player - EventType: {0}, CommentType: {1}, PlayerPosition: {2}, PlayerRating: {3}, PlayerName: {4}, PlayerTeam: [5}, Perspective: Negative",
            //    eventType.ToString(), commentType.ToString(), position, playerRating.ToString(), playerName, teamName);
            posComment = "Player = positive";
            negComment = "Player = negative";

            //Assign comment
            if (teamName.Trim().ToLower() == homeTeam.Name.Trim().ToLower()) //Does comment relate to home team
            {
                if (isPositiveEvent == true)
                {
                    homeTeamComment = posComment;
                    awayTeamComment = negComment;
                }
                else
                {
                    homeTeamComment = negComment;
                    awayTeamComment = posComment;
                }           
            }
            else
            {
                if (isPositiveEvent == true)
                {
                    awayTeamComment = posComment;
                    homeTeamComment = negComment;
                }
                else
                {
                    awayTeamComment = negComment;
                    homeTeamComment = posComment;
                }           
            }         
        }

        private static void GenerateTeamComment
            (
            Guid matchId,
            int homeTeamAPIId,
            int awayTeamAPIId,
            string feedComment,
            CommentType commentType, 
            EventType eventType, 
            out string homeTeamComment, 
            out string awayTeamComment
            )
        {
            homeTeamComment = string.Empty;
            awayTeamComment = string.Empty;
          
            var summary = GetSummaryByMatchId(matchId);

            int homeGoals = 0;
            int awayGoals = 0;
          
            if (summary != null)
            {
                using (EFDbContext context = new EFDbContext())
                {
                    homeGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == true)).Count();
                    awayGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == false)).Count();
                }
            }

            //Get teams
            var homeTeam = GetTeamByAPIId(homeTeamAPIId);
            var awayTeam = GetTeamByAPIId(awayTeamAPIId);

            //Get team ratings
            var matchStats = GetMatchStatsByAPIId(matchId);

            byte commType = (byte)commentType;
            byte evType = (byte)eventType;
            byte neutralPos = (byte)Perspective.Neutral;
            byte homeTeamRating = 0;
            byte awayTeamRating = 0;

            if(matchStats != null)
            {
                homeTeamRating = matchStats.HomeTeamRating;
                awayTeamRating = matchStats.AwayTeamRating;       
            }
         
            IList<string> homeComments = new List<string>();
            IList<string> awayComments = new List<string>();
          
            using (EFDbContext context = new EFDbContext())
            {
                homeComments = context.Comment
                .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == neutralPos) && (x.TeamRating == homeTeamRating))
                    .Select(x => x.Text).ToList();

                awayComments = context.Comment
                .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == neutralPos) && (x.TeamRating == awayTeamRating))
                    .Select(x => x.Text).ToList();
            }

            var random = new Random();
            int index = 0;
            string homeComment = string.Empty;
            string awayComment = string.Empty;

            Dictionary<string, string> placeholders = new Dictionary<string, string>();

            placeholders.Add("HOMETEAM", homeTeam.Name);
            placeholders.Add("AWAYTEAM", awayTeam.Name);

            if (homeComments != null)
            {
                if (homeComments.Count > 0)
                {
                    index = random.Next(homeComments.Count);
                    homeComment = homeComments[index];

                    //Replace text placeholders                
                    foreach (KeyValuePair<string, string> kvp in placeholders)
                    {
                        string strToReplace = "{%" + kvp.Key + "%}";

                        if (homeComment.Contains(strToReplace))
                        {
                            homeComment = homeComment.Replace(strToReplace, kvp.Value);
                        }
                    }
                }     
            }

            if (awayComments != null)
            {
                if (awayComments.Count > 0)
                {             
                    index = random.Next(awayComments.Count);
                    awayComment = awayComments[index];

                    //Replace text placeholders                
                    foreach (KeyValuePair<string, string> kvp in placeholders)
                    {
                        string strToReplace = "{%" + kvp.Key + "%}";

                        if (awayComment.Contains(strToReplace))
                        {
                            awayComment = awayComment.Replace(strToReplace, kvp.Value);
                        }
                    }
                }
            }

            homeTeamComment = homeComment;
            awayTeamComment = awayComment;

            //TODO - remove when real comments added, uncomment above
            //homeTeamComment = string.Format("Team - EventType: {0}, CommentType: {1}, HomeTeamRating: {2}, TeamName: [3}, Perspective: Neutral",
            //    eventType.ToString(), commentType.ToString(), homeTeamRating.ToString(), homeTeam.Name);
            //awayTeamComment = string.Format("Team - EventType: {0}, CommentType: {1}, AwayTeamRating: {2}, TeamName: [3}, Perspective: Neutral",
            //    eventType.ToString(), commentType.ToString(), awayTeamRating.ToString(), awayTeam.Name);

            homeTeamComment = "Team home team - Perspective: Neutral";
            awayTeamComment = "Team away team - Perspective: Neutral";   
        }

        private static void GenerateMatchComment
            (
            Guid matchId,
            int homeTeamAPIId,
            int awayTeamAPIId,
            string feedComment,
            CommentType commentType, 
            EventType eventType, 
            out string homeTeamComment, 
            out string awayTeamComment)
        {
            homeTeamComment = string.Empty;
            awayTeamComment = string.Empty;

            var summary = GetSummaryByMatchId(matchId);

            int homeGoals = 0;
            int awayGoals = 0;

            if (summary != null)
            {
                using (EFDbContext context = new EFDbContext())
                {
                    homeGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == true)).Count();
                    awayGoals = context.Goals.Where(x => (x.SummaryId == summary.Id) && (x.IsHomeTeam == false)).Count();
                }
            }

            //Get teams
            var homeTeam = GetTeamByAPIId(homeTeamAPIId);
            var awayTeam = GetTeamByAPIId(awayTeamAPIId);

            //Get team ratings
            var matchStats = GetMatchStatsByAPIId(matchId);

            byte commType = (byte)commentType;
            byte evType = (byte)eventType;
            byte neutralPos = (byte)Perspective.Neutral;
            byte matchRating = 0;
            byte homeTeamRating = 0;
            byte awayTeamRating = 0;

            if (matchStats != null)
            {
                homeTeamRating = matchStats.HomeTeamRating;
                awayTeamRating = matchStats.AwayTeamRating;
            }

            decimal calcRating = ((decimal)homeTeamRating + (decimal)awayTeamRating) / 2M;
            calcRating = calcRating + (((decimal)homeGoals + (decimal)awayGoals) * 0.2M);

            if (calcRating <= 0)
                matchRating = 1;
            else
                matchRating = Convert.ToByte(calcRating);

            IList<string> comments = new List<string>();
            
            using (EFDbContext context = new EFDbContext())
            {          
                comments = context.Comment
                    .Where(x => (x.CommentType == commType) && (x.EventType == evType) && (x.Perspective == neutralPos) && (x.MatchRating == matchRating))
                    .Select(x => x.Text).ToList();
            }

            var random = new Random();
            int index = 0;
            string homeComment = string.Empty;
            string awayComment = string.Empty;

            Dictionary<string, string> placeholders = new Dictionary<string, string>();

            placeholders.Add("HOMETEAM", homeTeam.Name);
            placeholders.Add("AWAYTEAM", awayTeam.Name);

            if (comments != null)
            {
                if (comments.Count > 0)
                {
                    index = random.Next(comments.Count);
                    homeComment = comments[index];

                    //Replace text placeholders                
                    foreach (KeyValuePair<string, string> kvp in placeholders)
                    {
                        string strToReplace = "{%" + kvp.Key + "%}";

                        if (homeComment.Contains(strToReplace))
                        {
                            homeComment = homeComment.Replace(strToReplace, kvp.Value);
                        }
                    }

                    index = random.Next(comments.Count);
                    awayComment = comments[index];

                    //Replace text placeholders
                    foreach (KeyValuePair<string, string> kvp in placeholders)
                    {
                        string strToReplace = "{%" + kvp.Key + "%}";

                        if (awayComment.Contains(strToReplace))
                        {
                            awayComment = awayComment.Replace(strToReplace, kvp.Value);
                        }
                    }
                }      
            }

            homeTeamComment = homeComment;
            awayTeamComment = awayComment;

            //TODO - remove when real comments added
            //homeTeamComment = string.Format("Match - EventType: {0}, CommentType: {1}, HomeTeamRating: {2}, TeamName: [3}, MatchRating: {4}, Perspective: Neutral",
            //    eventType.ToString(), commentType.ToString(), homeTeamRating.ToString(), homeTeam.Name, matchRating.ToString());
            //awayTeamComment = string.Format("Match - EventType: {0}, CommentType: {1}, AwayTeamRating: {2}, TeamName: [3}, MatchRating: {4}, Perspective: Neutral",
            //    eventType.ToString(), commentType.ToString(), awayTeamRating.ToString(), awayTeam.Name, matchRating.ToString());

            homeTeamComment = "Match home team - Perspective: Neutral";
            awayTeamComment = "Match away team - Perspective: Neutral";

            //update matchstat object with updated rating
            if (matchStats != null)
                SaveMatchStats(matchStats);
        }
        
        private static void GetPlayerAndTeamFromComment(EventType eventType, string comment, out string player, out string team, out string score)
        {
            player = string.Empty;
            team = string.Empty;
            score = string.Empty;

            string[] arr;
            string[] arr2;
            string[] arr3;

            try
            {
                switch (eventType)
                {
                    case EventType.AttemptBlocked:              
                    case EventType.AttemptMissedTooHigh:                  
                    case EventType.AttemptMissedHighAndWide:                    
                    case EventType.AttemptMissesToRightOrLeft:                  
                    case EventType.AttemptMissesJustABitTooHigh:                  
                    case EventType.AttemptMissed:                    
                    case EventType.AttemptSaved:
                    case EventType.FreeKick:
                    case EventType.YellowCard:
                    case EventType.RedCard:
                    case EventType.HitsTheBar:
                    case EventType.HitsThePost:
                    case EventType.Foul:
                    case EventType.Handball:
                        //Attempt blocked. Adam Lallana (Liverpool) left footed shot from the centre of the box is blocked. Assisted by Nathaniel Clyne.
                        //Adam Lallana (Liverpool) wins a free kick in the attacking half.
                        //Foul by Alberto Moreno (Liverpool).

                        arr = comment.Split('(');
                        player = arr[0].Trim();

                        arr2 = arr[1].Split(')');
                        team = arr2[0].Trim();

                        break;               
                    case EventType.Corner:
                        //Corner,  Crystal Palace. Conceded by Brendan Galloway.

                        arr = comment.Split('.');
                        team = arr[0].Trim();

                        break;
                    case EventType.Delay:
                    case EventType.DelayEnds:
                    case EventType.FirstHalfBegins:              
                    case EventType.LineupsAnnounced:
                    case EventType.General:
                        //Don't need to return anything

                        break;                
                    case EventType.Goal:
                        //Goal!  Everton 1, Crystal Palace 1. Romelu Lukaku (Everton) right footed shot from very close range to the centre of the goal. Assisted by Gareth Barry.
                        arr = comment.Split('.');
                        score = arr[0].Trim();

                        //TODO - add assist
                        arr2 = arr[1].Split('(');
                        player = arr2[0].Trim();

                        arr3 = arr2[1].Split(')');
                        team = arr3[0].Trim();

                        break;
                    case EventType.FirstHalfEnds:
                    case EventType.SecondHalfBegins:
                    case EventType.MatchEnds:
                        //Match ends, Newcastle United 2, Liverpool 0.

                        arr = comment.Split('.');
                        score = arr[0].Trim();

                        break;
                    case EventType.Offside:
                        //Offside, Crystal Palace. James McArthur tries a through ball, but Yannick Bolasie is caught offside.

                            arr = comment.Split('.');
                            team = arr[0].Trim();

                            arr2 = arr[1].Split(',');
                            player = arr2[1].Trim();

                            player = player.Replace("but", string.Empty);
                            player = player.Replace("is caught offside.", string.Empty).Trim();
                   
                        break;           
                    case EventType.Substitution:
                        //Substitution, Crystal Palace. Dwight Gayle replaces Connor Wickham.

                            arr = comment.Split('.');
                            team = arr[0].Trim();
                            player = arr[1].Trim();

                            int index = player.IndexOf("replaces");
                            player = player.Substring(0, index - 1).Trim();

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                SaveException(ex, string.Format("GetPlayerAndTeamFromComment, Comment: {0}", comment));
            }
        }

        public string GetIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);

            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }

            return IPAddress;
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

        #endregion
    }
}