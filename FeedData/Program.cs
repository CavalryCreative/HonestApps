﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMS.Infrastructure.Entities;
using CMS.Infrastructure.Concrete;

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
            GetFixtures(DateTime.Now, DateTime.Now.AddDays(7));
            //GetCommentaries(2146623);
        }

        private static string GetFixtures(DateTime startDate, DateTime endDate)
        {
            //const int POINT_OF_SATISFIED_CURIOSITY = 7;
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

                    JToken token = JObject.Parse(s);

                    string jPath = "matches";
                    //int lastUpdateId = 8961117;

                    var matches = token.SelectTokens(jPath);

                    foreach (var childToken in matches.Children())
                    {
                        //var jeff = childToken.ToObject<Dictionary<string, string>();

                        //foreach (var evt in jeff.Select(x => x.ToObject<Dictionary<string, string>>()))
                        //{
                            //string id = evt["id"];
                            //int eventId = Convert.ToInt32(id);

                            //if (eventId <= lastUpdateId)
                            //{
                            //    break;
                            //}

                            //Event commEvent = new Event();

                        //string important = childToken["match_localteam_id"];
                            //string isgoal = evt["isgoal"];
                            //string minute = evt["minute"];

                            //if (minute.Contains('\''))
                            //{
                            //    minute = minute.Remove(minute.Length - 1, 1);
                            //}

                            //string comment = evt["comment"];

                            //commEvent.Important = important == "True" ? true : false;
                            //commEvent.Goal = isgoal == "True" ? true : false;

                            //if (!string.IsNullOrWhiteSpace(minute))
                            //    commEvent.Minute = Convert.ToByte(minute);

                            //commEvent.Comment = comment;
                            //commEvent.APIId = eventId;
                            ////commEvent.MatchId = matchId;

                            //EFEvent efEvent = new EFEvent();
                            //efEvent.Save(commEvent);
                        //}
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

                            EFEvent efEvent = new EFEvent();
                            efEvent.Save(commEvent);
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
    }
}
