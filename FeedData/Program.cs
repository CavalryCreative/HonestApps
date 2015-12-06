﻿using System;
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

          //GetCommentaries(2148519);
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
                         //Check if Update History Match details is true/false
                         var updateHistory = GetUpdateHistoryByMatchAPIId(item.APIId);
                         bool matchDetailsAdded = false;

                         if (updateHistory != null)
                         {
                             matchDetailsAdded = updateHistory.MatchDetails;
                         }

                         if (matchDetailsAdded == false)
                         {
                             Match match = new Match();

                             match.APIId = item.APIId;
                             match.Date = Convert.ToDateTime(string.Format("{0} {1}", item.MatchDate.Replace('.', '/'), item.Time));
                             match.EndDate = match.Date.Value.AddHours(2);
                             match.Active = DateTime.Now <= match.EndDate ? true : false;
                             match.IsToday = DateTime.Now.ToShortDateString() == match.Date.Value.ToShortDateString() ? true : false;
                             //change this when live
                             //match.IsLive = true;
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

                             UpdateHistory history = new UpdateHistory();

                             history.MatchAPIId = match.APIId;
                             history.MatchDetails = true;
                             history.Id = System.Guid.Empty;

                             SaveUpdateHistory(history);
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

            return liveMatches;
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

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                int playerAPIId = Convert.ToInt32(evt["id"]);

                                //Check if player exists
                                var player = GetPlayerByAPIId(playerAPIId);
                                Guid playerId = Guid.Empty;

                                //Add to Players if not already added
                                if (player == null)
                                {
                                    Player newPlayer = new Player();

                                    newPlayer.SquadNumber = Convert.ToByte(evt["number"]);
                                    newPlayer.Name = evt["name"];
                                    newPlayer.Position = evt["pos"];
                                    newPlayer.Id = System.Guid.Empty;
                                    newPlayer.APIPlayerId = playerAPIId;

                                    retMsg = SavePlayer(newPlayer, homeTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        newPlayer.Id = new Guid(Id);
                                    }

                                    player = newPlayer;
                                }
                                else
                                {
                                    playerId = player.Id;
                                }

                                //add to Lineups
                                Lineup lineup = new Lineup();

                                lineup.IsHomePlayer = true;
                                lineup.IsSub = false;
                                //lineup.Match = match;
                                lineup.Match_Id = match.Id;
                                lineup.MatchAPIId = matchId;
                                //lineup.Player = player;
                                lineup.Player_Id = player.Id;
                                //lineup.Team = homeTeam;
                                lineup.Team_Id = homeTeam.Id;

                                SavePlayerToMatchLineup(lineup);
                            }
                        }
                        #endregion

                        #region Home subs

                        jPath = "commentaries.[0].comm_match_subs.localteam.player";

                        y = token.SelectTokens(jPath);

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                int playerAPIId = Convert.ToInt32(evt["id"]);

                                //Check if player exists
                                var player = GetPlayerByAPIId(playerAPIId);
                                Guid playerId = Guid.Empty;

                                //Add to Players if not already added
                                if (player == null)
                                {
                                    Player newPlayer = new Player();

                                    newPlayer.SquadNumber = Convert.ToByte(evt["number"]);
                                    newPlayer.Name = evt["name"];
                                    newPlayer.Position = evt["pos"];
                                    newPlayer.Id = System.Guid.Empty;
                                    newPlayer.APIPlayerId = playerAPIId;

                                    retMsg = SavePlayer(newPlayer, homeTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        newPlayer.Id = new Guid(Id);
                                    }

                                    player = newPlayer;
                                }
                                else
                                {
                                    playerId = player.Id;
                                }

                                //add to Lineups
                                Lineup lineup = new Lineup();

                                lineup.IsHomePlayer = true;
                                lineup.IsSub = true;
                                //lineup.Match = match;
                                lineup.Match_Id = match.Id;
                                lineup.MatchAPIId = matchId;
                                //lineup.Player = player;
                                lineup.Player_Id = player.Id;
                                //lineup.Team = homeTeam;
                                lineup.Team_Id = homeTeam.Id;

                                SavePlayerToMatchLineup(lineup);
                            }
                        }
                        #endregion

                        #region Away Team

                        jPath = "commentaries.[0].comm_match_teams.visitorteam.player";

                        y = token.SelectTokens(jPath);

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                int playerAPIId = Convert.ToInt32(evt["id"]);

                                //Check if player exists
                                var player = GetPlayerByAPIId(playerAPIId);
                                Guid playerId = Guid.Empty;

                                //Add to Players if not already added
                                if (player == null)
                                {
                                    Player newPlayer = new Player();

                                    newPlayer.SquadNumber = Convert.ToByte(evt["number"]);
                                    newPlayer.Name = evt["name"];
                                    newPlayer.Position = evt["pos"];
                                    newPlayer.Id = System.Guid.Empty;
                                    newPlayer.APIPlayerId = playerAPIId;

                                    retMsg = SavePlayer(newPlayer, awayTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        newPlayer.Id = new Guid(Id);
                                    }

                                    player = newPlayer;
                                }
                                else
                                {
                                    playerId = player.Id;
                                }

                                //add to Lineups
                                Lineup lineup = new Lineup();

                                lineup.IsHomePlayer = false;
                                lineup.IsSub = false;
                                //lineup.Match = match;
                                lineup.Match_Id = match.Id;
                                lineup.MatchAPIId = matchId;
                                //lineup.Player = player;
                                lineup.Player_Id = player.Id;
                                //lineup.Team = homeTeam;
                                lineup.Team_Id = awayTeam.Id;

                                SavePlayerToMatchLineup(lineup);
                            }
                        }

                        #endregion

                        #region Away subs

                        jPath = "commentaries.[0].comm_match_subs.visitorteam.player";

                        y = token.SelectTokens(jPath);

                        foreach (var childToken in y.Children())
                        {
                            var jeff = childToken.Children();

                            foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                            {
                                int playerAPIId = Convert.ToInt32(evt["id"]);

                                //Check if player exists
                                var player = GetPlayerByAPIId(playerAPIId);
                                Guid playerId = Guid.Empty;

                                //Add to Players if not already added
                                if (player == null)
                                {
                                    Player newPlayer = new Player();

                                    newPlayer.SquadNumber = Convert.ToByte(evt["number"]);
                                    newPlayer.Name = evt["name"];
                                    newPlayer.Position = evt["pos"];
                                    newPlayer.Id = System.Guid.Empty;
                                    newPlayer.APIPlayerId = playerAPIId;

                                    retMsg = SavePlayer(newPlayer, awayTeam.Id);

                                    ReturnId(retMsg, out actionMessage, out Id);

                                    if ((actionMessage == Res.Resources.RecordAdded) || (actionMessage == Res.Resources.RecordUpdated))
                                    {
                                        newPlayer.Id = new Guid(Id);
                                    }

                                    player = newPlayer;
                                }
                                else
                                {
                                    playerId = player.Id;
                                }

                                //add to Lineups
                                Lineup lineup = new Lineup();

                                lineup.IsHomePlayer = false;
                                lineup.IsSub = true;
                                //lineup.Match = match;
                                lineup.Match_Id = match.Id;
                                lineup.MatchAPIId = matchId;
                                //lineup.Player = player;
                                lineup.Player_Id = player.Id;
                                //lineup.Team = homeTeam;
                                lineup.Team_Id = awayTeam.Id;

                                SavePlayerToMatchLineup(lineup);
                            }
                        }
                        #endregion

                        UpdateHistory history = new UpdateHistory();
                   
                        history.MatchAPIId = matchId;
                        history.Lineups = true;
                        history.Id = System.Guid.Empty;
                     
                        SaveUpdateHistory(history);
                    }                  

                    #endregion

                    #region Match Stats
                    //TODO - create record (one for match) - updatehistory

                    #endregion

                    #region Player Stats

                    #endregion

                    #region Events

                    jPath = "commentaries.[0].comm_commentaries.comment";

                    //Retrieve lastupdateId
                    int lastUpdateId = GetLatestEventId(match.Id);

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
                            commEvent.MatchId = match.Id;

                            SaveEvent(commEvent);
                        }
                    }
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

        #region Save
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

        private static string SavePlayer(Player updatedRecord, Guid teamId)
        {
            Guid Id = Guid.Empty;

            Entities context = new Entities();

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

        private static string SavePlayerToMatchLineup(Lineup updatedRecord)
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

        private static string SaveUpdateHistory(UpdateHistory updatedRecord)
        {
            bool isNewRecord = false;
            Guid Id;

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
        #endregion

        #region Get

        private static UpdateHistory GetUpdateHistoryByMatchAPIId(int id)
        {
            Entities context = new Entities();

            return context.UpdateHistories.Where(x => (x.MatchAPIId == id)).FirstOrDefault();
        }

        private static Player GetPlayerByAPIId(int id)
        {
            Entities context = new Entities();

            return context.Players.Where(x => (x.APIPlayerId == id)).FirstOrDefault();
        }

        private static Team GetTeamByAPIId(int id)
        {
            Entities context = new Entities();

            return context.Teams.Where(x => (x.APIId == id)).FirstOrDefault();
        }

        private static Match GetMatchByAPIId(int id)
        {
            Entities context = new Entities();

            return context.Matches.Where(x => (x.APIId == id)).FirstOrDefault();
        }

        private static int GetLatestEventId(Guid id)
        {
            Entities context = new Entities();

            int retInt = 0;

            var evt = context.Events.Where(x => (x.MatchId == id)).OrderByDescending(x => x.DateAdded).FirstOrDefault();

            if (evt != null)
                retInt = evt.APIId;

            return retInt;
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
