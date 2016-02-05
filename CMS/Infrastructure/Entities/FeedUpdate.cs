using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Hosting;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Concrete;
using Res = Resources;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
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

        private FeedUpdate(IHubConnectionContext<dynamic> clients)
        {
            HostingEnvironment.RegisterObject(this);

            Clients = clients;
            //matchTimer = new Timer(GetFixtures, null, TimeSpan.FromSeconds(1), TimeSpan.FromDays(1));
            eventsTimer = new Timer(BroadcastFeed, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(120000));         
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
            var liveMatches = GetLiveMatches();
            Object thisLock = new Object();

            if (liveMatches.Count > 0)
            {
                foreach (var match in liveMatches)
                {
                    //lock (thisLock)
                    //{
                        GetCommentaries(match.APIId);
                    //}                    
                }
            }
        }

        private static string GetCommentaries(int matchId)
        {
            string retMsg = string.Empty;

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

                    var match = GetMatchByAPIId(matchId);

                    var homeTeam = GetTeamByAPIId(match.HomeTeamAPIId);
                    var awayTeam = GetTeamByAPIId(match.AwayTeamAPIId);

                    string jPath = string.Empty;

                    //Check if Update History Lineups is true/false
                    var updateHistory = GetUpdateHistoryByMatchAPIId(matchId);
                    bool lineupsAdded = false;

                    if (updateHistory != null)
                    {
                        lineupsAdded = updateHistory.Lineups;
                    }

                    #region Lineups

                    //TODO - create Player stats record for each player
                    if (lineupsAdded == false)
                    {
                        #region Home Team

                        jPath = "commentaries.[0].comm_match_teams.localteam.player";

                        var y = token.SelectTokens(jPath);

                        string Id;
                        string actionMessage;
                        Byte squadNumberByte;
                        int playerAPIId;
                        bool result;

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            Player playerExists = new Player();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                result = Int32.TryParse(evt["id"], out playerAPIId);

                                //Check if player exists                           
                                if (result)
                                    playerExists = GetPlayerByAPIId(playerAPIId);

                                if (playerExists == null)
                                    playerExists = GetPlayerByName(evt["name"]);                              

                                //Add to Players if not already added
                                if (playerExists == null)
                                {
                                    Player homePlayer = new Player();

                                    result = Byte.TryParse(evt["number"], out squadNumberByte);

                                    if (result)
                                        homePlayer.SquadNumber = squadNumberByte;
                                    else
                                        homePlayer.SquadNumber = 0;

                                    homePlayer.Name = evt["name"];
                                    homePlayer.Position = evt["pos"];
                                    homePlayer.Id = System.Guid.Empty;
                                    homePlayer.APIPlayerId = playerAPIId;

                                    //System.Diagnostics.Debug.WriteLine(string.Format("Home: {0},{1}",
                                    //     homePlayer.Name,
                                    //      homePlayer.Position
                                    //     ));

                                    retMsg = SavePlayer(homePlayer, homeTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        homePlayer.Id = new Guid(Id);
                                    }

                                    //add to Lineups
                                    Lineup lineup = new Lineup();

                                    lineup.IsHomePlayer = true;
                                    lineup.IsSub = false;                                   
                                    lineup.MatchAPIId = matchId;
                                    lineup.PlayerId = homePlayer.Id;
                                                                     
                                    SavePlayerToMatchLineup(lineup);
                                }                       
                            }
                        }

                        System.Diagnostics.Debug.WriteLine("Home players complete");

                        #endregion

                        #region Home subs

                        jPath = "commentaries.[0].comm_match_subs.localteam.player";

                        y = token.SelectTokens(jPath);

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                result = Int32.TryParse(evt["id"], out playerAPIId);

                                //Check if player exists
                                Player playerExists = new Player();

                                if (result)
                                    playerExists = GetPlayerByAPIId(playerAPIId);

                                if (playerExists == null)
                                    playerExists = GetPlayerByName(evt["name"]);

                                //Add to Players if not already added
                                if (playerExists == null)
                                {
                                    Player homeSubPlayer = new Player();

                                    result = Byte.TryParse(evt["number"], out squadNumberByte);

                                    if (result)
                                        homeSubPlayer.SquadNumber = squadNumberByte;
                                    else
                                        homeSubPlayer.SquadNumber = 0;

                                    homeSubPlayer.Name = evt["name"];
                                    homeSubPlayer.Position = evt["pos"];
                                    homeSubPlayer.Id = System.Guid.Empty;
                                    homeSubPlayer.APIPlayerId = playerAPIId;

                                    //System.Diagnostics.Debug.WriteLine(string.Format("HomeSub: {0},{1}",
                                    //    homeSubPlayer.Name,
                                    //     homeSubPlayer.Position
                                    //    ));

                                    retMsg = SavePlayer(homeSubPlayer, homeTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        homeSubPlayer.Id = new Guid(Id);
                                    }

                                    //add to Lineups
                                    Lineup lineup = new Lineup();

                                    lineup.IsHomePlayer = true;
                                    lineup.IsSub = true;
                                    lineup.MatchAPIId = matchId;
                                    lineup.PlayerId = homeSubPlayer.Id;
                                   
                                    SavePlayerToMatchLineup(lineup);
                                }                              
                            }
                        }

                        System.Diagnostics.Debug.WriteLine("Home players subs complete");

                        #endregion

                        #region Away Team

                        jPath = "commentaries.[0].comm_match_teams.visitorteam.player";

                        y = token.SelectTokens(jPath);

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                result = Int32.TryParse(evt["id"], out playerAPIId);

                                //Check if player exists
                                Player playerExists = new Player();

                                if (result)
                                    playerExists = GetPlayerByAPIId(playerAPIId);

                                if (playerExists == null)
                                    playerExists = GetPlayerByName(evt["name"]);

                                //Add to Players if not already added
                                if (playerExists == null)
                                {
                                    Player awayPlayer = new Player();

                                    result = Byte.TryParse(evt["number"], out squadNumberByte);

                                    if (result)
                                        awayPlayer.SquadNumber = squadNumberByte;
                                    else
                                        awayPlayer.SquadNumber = 0;

                                    awayPlayer.Name = evt["name"];
                                    awayPlayer.Position = evt["pos"];
                                    awayPlayer.Id = System.Guid.Empty;
                                    awayPlayer.APIPlayerId = playerAPIId;

                                    //System.Diagnostics.Debug.WriteLine(string.Format("Away: {0},{1}",
                                    //  awayPlayer.Name,
                                    //   awayPlayer.Position
                                    //  ));

                                    retMsg = SavePlayer(awayPlayer, awayTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        awayPlayer.Id = new Guid(Id);
                                    }

                                    //add to Lineups
                                    Lineup lineup = new Lineup();

                                    lineup.IsHomePlayer = false;
                                    lineup.IsSub = false;                                  
                                    lineup.MatchAPIId = matchId;
                                    lineup.PlayerId = awayPlayer.Id;
                                   
                                    SavePlayerToMatchLineup(lineup);
                                }                               
                            }
                        }

                        System.Diagnostics.Debug.WriteLine("Away players complete");

                        #endregion

                        #region Away subs

                        jPath = "commentaries.[0].comm_match_subs.visitorteam.player";

                        y = token.SelectTokens(jPath);

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                result = Int32.TryParse(evt["id"], out playerAPIId);

                                //Check if player exists
                                Player playerExists = new Player();

                                if (result)
                                    playerExists = GetPlayerByAPIId(playerAPIId);

                                if (playerExists == null)
                                    playerExists = GetPlayerByName(evt["name"]);

                                //Add to Players if not already added
                                if (playerExists == null)
                                {
                                    Player awaySubPlayer = new Player();

                                    result = Byte.TryParse(evt["number"], out squadNumberByte);

                                    if (result)
                                        awaySubPlayer.SquadNumber = squadNumberByte;
                                    else
                                        awaySubPlayer.SquadNumber = 0;

                                    awaySubPlayer.Name = evt["name"];
                                    awaySubPlayer.Position = evt["pos"];
                                    awaySubPlayer.Id = System.Guid.Empty;
                                    awaySubPlayer.APIPlayerId = playerAPIId;

                                    //System.Diagnostics.Debug.WriteLine(string.Format("AwaySub: {0},{1}",
                                    //  awaySubPlayer.Name,
                                    //   awaySubPlayer.Position
                                    //  ));

                                    retMsg = SavePlayer(awaySubPlayer, awayTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        awaySubPlayer.Id = new Guid(Id);
                                    }

                                    //add to Lineups
                                    Lineup lineup = new Lineup();

                                    lineup.IsHomePlayer = false;
                                    lineup.IsSub = true;                                   
                                    lineup.MatchAPIId = matchId;
                                    lineup.PlayerId = awaySubPlayer.Id;
                                    
                                    SavePlayerToMatchLineup(lineup);
                                }
                            }
                        }

                        System.Diagnostics.Debug.WriteLine("Away players subs complete");

                        #endregion

                        UpdateHistory history = new UpdateHistory();

                        history.MatchAPIId = matchId;
                        history.Lineups = true;
                        history.Id = System.Guid.Empty;

                        SaveUpdateHistory(history);
                    }

                    #endregion

                    #region Match Stats

                    Stat stat = new Stat();
                    stat.MatchId = match.Id;

                    var matchStats = GetMatchStatsByAPIId(match.Id);

                    if (matchStats != null)
                        stat.Id = matchStats.Id;

                    JObject obj = JObject.Parse(s);
                    stat.HomeTeamCorners = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.corners.total").ToString());
                    stat.HomeTeamOffsides = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.offsides.total").ToString());
                    stat.HomeTeamPossessionTime = obj.SelectToken("commentaries.[0].comm_match_stats.localteam.offsides.total").ToString();
                    stat.HomeTeamSaves = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.saves.total").ToString());
                    stat.HomeTeamOnGoalShots = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.shots.ongoal").ToString());
                    stat.HomeTeamTotalShots = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.shots.total").ToString());
                    stat.HomeTeamFouls = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.fouls.total").ToString());
                    stat.HomeTeamRedCards = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.redcards.total").ToString());
                    stat.HomeTeamYellowCards = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.localteam.yellowcards.total").ToString());

                    stat.AwayTeamCorners = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.corners.total").ToString());
                    stat.AwayTeamOffsides = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.offsides.total").ToString());
                    stat.AwayTeamPossessionTime = obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.offsides.total").ToString();
                    stat.AwayTeamSaves = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.saves.total").ToString());
                    stat.AwayTeamOnGoalShots = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.shots.ongoal").ToString());
                    stat.AwayTeamTotalShots = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.shots.total").ToString());
                    stat.AwayTeamFouls = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.fouls.total").ToString());
                    stat.AwayTeamRedCards = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.redcards.total").ToString());
                    stat.AwayTeamYellowCards = Convert.ToByte(obj.SelectToken("commentaries.[0].comm_match_stats.visitorteam.yellowcards.total").ToString());

                    try
                    {
                        SaveMatchStats(stat);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                    }                            

                    System.Diagnostics.Debug.WriteLine("Match stats complete");

                    #endregion

                    #region Player Stats

                    //Home players
                    jPath = "commentaries.[0].comm_match_player_stats.localteam.player";

                    var playerStats = token.SelectTokens(jPath);

                    foreach (var statToken in playerStats.Children())
                    {
                        var fred = statToken.Children();

                        foreach (var evt in fred.Select(x => x.ToObject<Dictionary<string, string>>()))
                        {
                            PlayerStat playerStat = new PlayerStat();

                            playerStat.APIId = Convert.ToInt32(evt["id"]);

                            Player player = new Player();
                            player = GetPlayerByAPIId(playerStat.APIId);

                            playerStat.Assists = string.IsNullOrWhiteSpace(evt["assists"]) ? (byte)0 : Convert.ToByte(evt["assists"]);
                            playerStat.FoulsCommitted = string.IsNullOrWhiteSpace(evt["fouls_commited"]) ? (byte)0 : Convert.ToByte(evt["fouls_commited"]);
                            playerStat.FoulsDrawn = string.IsNullOrWhiteSpace(evt["fouls_drawn"]) ? (byte)0 : Convert.ToByte(evt["fouls_drawn"]);
                            playerStat.Goals = string.IsNullOrWhiteSpace(evt["goals"]) ? (byte)0 : Convert.ToByte(evt["goals"]);
                            playerStat.MatchId = match.Id;
                            playerStat.Offsides = string.IsNullOrWhiteSpace(evt["offsides"]) ? (byte)0 : Convert.ToByte(evt["offsides"]);
                            playerStat.PenaltiesMissed = string.IsNullOrWhiteSpace(evt["pen_miss"]) ? (byte)0 : Convert.ToByte(evt["pen_miss"]);
                            playerStat.PenaltiesScored = string.IsNullOrWhiteSpace(evt["pen_score"]) ? (byte)0 : Convert.ToByte(evt["pen_score"]);
                            playerStat.PlayerId = player.Id;
                            playerStat.PositionX = string.IsNullOrWhiteSpace(evt["posx"]) ? (byte)0 : Convert.ToByte(evt["posx"]);
                            playerStat.PositionY = string.IsNullOrWhiteSpace(evt["posy"]) ? (byte)0 : Convert.ToByte(evt["posy"]);
                            playerStat.RedCards = string.IsNullOrWhiteSpace(evt["redcards"]) ? (byte)0 : Convert.ToByte(evt["redcards"]);
                            playerStat.Saves = string.IsNullOrWhiteSpace(evt["saves"]) ? (byte)0 : Convert.ToByte(evt["saves"]);
                            playerStat.ShotsOnGoal = string.IsNullOrWhiteSpace(evt["shots_on_goal"]) ? (byte)0 : Convert.ToByte(evt["shots_on_goal"]);
                            playerStat.TotalShots = string.IsNullOrWhiteSpace(evt["shots_total"]) ? (byte)0 : Convert.ToByte(evt["shots_total"]);
                            playerStat.YellowCards = string.IsNullOrWhiteSpace(evt["yellowcards"]) ? (byte)0 : Convert.ToByte(evt["yellowcards"]);

                            try
                            {
                                SavePlayerStats(playerStat);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Home player stats complete");

                    //Away players
                    //jPath = "commentaries.[0].comm_match_player_stats.visitorteam.player";

                    //playerStats = token.SelectTokens(jPath);

                    //foreach (var statToken in playerStats.Children())
                    //{
                    //    var fred = statToken.Children();

                    //    foreach (var evt in fred.Select(x => x.ToObject<Dictionary<string, string>>()))
                    //    {
                    //        PlayerStat playerStat = new PlayerStat();

                    //        playerStat.APIId = Convert.ToInt32(evt["id"]);

                    //        Player player = new Player();
                    //        player = GetPlayerByAPIId(playerStat.APIId);

                    //        playerStat.Assists = string.IsNullOrWhiteSpace(evt["assists"]) ? (byte)0 : Convert.ToByte(evt["assists"]);
                    //        playerStat.FoulsCommitted = string.IsNullOrWhiteSpace(evt["fouls_commited"]) ? (byte)0 : Convert.ToByte(evt["fouls_commited"]);
                    //        playerStat.FoulsDrawn = string.IsNullOrWhiteSpace(evt["fouls_drawn"]) ? (byte)0 : Convert.ToByte(evt["fouls_drawn"]);
                    //        playerStat.Goals = string.IsNullOrWhiteSpace(evt["goals"]) ? (byte)0 : Convert.ToByte(evt["goals"]);
                    //        playerStat.MatchId = match.Id;
                    //        playerStat.Offsides = string.IsNullOrWhiteSpace(evt["offsides"]) ? (byte)0 : Convert.ToByte(evt["offsides"]);
                    //        playerStat.PenaltiesMissed = string.IsNullOrWhiteSpace(evt["pen_miss"]) ? (byte)0 : Convert.ToByte(evt["pen_miss"]);
                    //        playerStat.PenaltiesScored = string.IsNullOrWhiteSpace(evt["pen_score"]) ? (byte)0 : Convert.ToByte(evt["pen_score"]);
                    //        playerStat.PlayerId = player.Id;
                    //        playerStat.PositionX = string.IsNullOrWhiteSpace(evt["posx"]) ? (byte)0 : Convert.ToByte(evt["posx"]);
                    //        playerStat.PositionY = string.IsNullOrWhiteSpace(evt["posy"]) ? (byte)0 : Convert.ToByte(evt["posy"]);
                    //        playerStat.RedCards = string.IsNullOrWhiteSpace(evt["redcards"]) ? (byte)0 : Convert.ToByte(evt["redcards"]);
                    //        playerStat.Saves = string.IsNullOrWhiteSpace(evt["saves"]) ? (byte)0 : Convert.ToByte(evt["saves"]);
                    //        playerStat.ShotsOnGoal = string.IsNullOrWhiteSpace(evt["shots_on_goal"]) ? (byte)0 : Convert.ToByte(evt["shots_on_goal"]);
                    //        playerStat.TotalShots = string.IsNullOrWhiteSpace(evt["shots_total"]) ? (byte)0 : Convert.ToByte(evt["shots_total"]);
                    //        playerStat.YellowCards = string.IsNullOrWhiteSpace(evt["yellowcards"]) ? (byte)0 : Convert.ToByte(evt["yellowcards"]);

                    //        try
                    //        {
                    //            SavePlayerStats(playerStat);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                    //        }
                    //    }
                    //}

                    //System.Diagnostics.Debug.WriteLine("Away players stats complete");

                    #endregion

                    #region Events

                    jPath = "commentaries.[0].comm_commentaries.comment";

                    //Retrieve lastupdateId
                    int lastUpdateId = GetLatestEventId(match.Id);

                    var events = token.SelectTokens(jPath);

                    foreach (var childToken in events.Children())
                    {
                        var jeff = childToken.Children();

                        foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                        {
                            string id = evt["id"];
                            int eventId;

                            bool result = Int32.TryParse(id, out eventId);

                            //Check if player exists                           
                            if (!result)
                                eventId = 0;

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
                            commEvent.MatchId = match.Id;
                            commEvent.Score = "";

                            //Todo - set match rating for both teams

                            try
                            {
                                SaveEvent(commEvent);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("Inner Exception: {0}, Message: {1}", ex.InnerException, ex.Message));
                            }                                   
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Events complete");
                    #endregion
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

        private static string GetFeed()
        {
            var jsonObject = new JObject();

            jsonObject.Add("Updated", DateTime.Now);

            dynamic feed = jsonObject;
            feed.Events = new JArray() as dynamic;
           
            var todaysMatches = GetTodayMatches();

            foreach (var match in todaysMatches)
            {
                var matchDetails = GetMatchByAPIId(match.APIId);
                var latestEvents = GetLatestEvents(match.APIId);

                foreach (var evt in latestEvents)
                {
                    dynamic feedEvent = new JObject();

                    //TODO - generate home/away comment based on match rating
                    feedEvent.EventComment = evt.Comment;
                    feedEvent.Score = "1-0";//TODO - return score from event
                    feedEvent.Minute = evt.Minute;
                    feedEvent.EventAPIId = evt.APIId;
                    feedEvent.MatchAPIId = matchDetails.APIId;
                    feedEvent.HomeComment = "Test";
                    feedEvent.HomeTeamAPIId = matchDetails.HomeTeamAPIId;
                    feedEvent.AwayComment = "Test";
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
                        var recordToUpdate = context.UpdateHistory.Find(updatedRecord.Id);

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

        private static Team GetTeamByAPIId(int id)
        {
            Team team = new Team();

            using (EFDbContext context = new EFDbContext())
            {
                team = context.Teams.Where(x => (x.APIId == id)).FirstOrDefault();
            }

            return team;
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

        private static IList<Match> GetLiveMatches()
        {
            IList<Match> liveMatches = new List<Match>();

            using (EFDbContext context = new EFDbContext())
            {
                liveMatches = context.Matches.Where(x => x.IsLive == true).ToList();
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

        private static IList<Event> GetLatestEvents(int id)
        {
            IList<Event> latestEvents = new List<Event>();

            using (EFDbContext context = new EFDbContext())
            {
                latestEvents = context.Events.Where(x => (x.APIId == id)).OrderByDescending(x => x.DateAdded).Take(5).ToList();
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