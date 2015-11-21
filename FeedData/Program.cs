using System;
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
    class Program
    {
        static void Main(string[] args)
        {
            //GetFixtures();
            GetCommentaries();
        }

        private static string GetFixtures()
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
                string uri = "http://football-api.com/api/?Action=commentaries&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&match_id=2146622";  // <-- this returns formatted json

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    var arr = JsonConvert.DeserializeObject<JObject>(s);
                    int i = 1;
                    //string cat;
                    //string film;
                    //string instavid;
                    //string bluray;
                    //string dvd;
                    //string imghtml;

                    foreach (KeyValuePair<string,JToken> obj in arr)
                    {
                        //cat = (string)obj["category"];
                        //film = (string)obj["film"];
                        //instavid = (string)obj["instavid"];
                        //bluray = (string)obj["bluray"];
                        //dvd = (string)obj["dvd"];
                        //imghtml = (string)obj["imghtml"];
                        //MessageBox.Show(string.Format("Object {0} in JSON array: cat == {1}, " +
                        //  "film == {2}, instavid == {3}, bluray == {4}, dvd == {5}, imghtml == {6}",
                        //     i, cat, film, instavid, bluray, dvd, imghtml));
                     
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

        private static string GetCommentaries()
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
                string uri = "http://football-api.com/api/?Action=commentaries&APIKey=5d003dc1-7e24-ad8d-a2c3610dd99b&match_id=2146622";  // <-- this returns formatted json

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";  // <-- GET is the default method/verb, but it's here for clarity
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                if ((webResponse.StatusCode == HttpStatusCode.OK)) //&& (webResponse.ContentLength > 0))
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    string s = reader.ReadToEnd();

                    var arr = JsonConvert.DeserializeObject<JObject>(s);

                    foreach (KeyValuePair<string, JToken> obj in arr)
                    {
                        if (obj.Value.ToString() == "commentaries")
                        {

                        }
                        //cat = (string)obj["category"];
                        //film = (string)obj["film"];
                        //instavid = (string)obj["instavid"];
                        //bluray = (string)obj["bluray"];
                        //dvd = (string)obj["dvd"];
                        //imghtml = (string)obj["imghtml"];
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
