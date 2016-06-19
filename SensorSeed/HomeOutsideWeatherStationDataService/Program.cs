using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Quartz;
using Quartz.Impl;
using Quartz.Job;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Net.Cache;

namespace HomeOutsideWeatherStationDataService
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                DateTime currentTime = DateTime.Now;


                string html = string.Empty;
                string url = @"http://10.0.14.34/";
                var postData = new NameValueCollection();

                bool tryAgain = true;
                while (tryAgain)
                {
                    Console.Write(currentTime.ToString());
                    Console.Write(":    Getting sensor data...");
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                        request.Timeout = 20000;

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                            int index = html.IndexOf("\n");
                            html = html.Substring(index + "\n".Length);
                            string[] lines = html.Split(new string[] { "\n" }, StringSplitOptions.None);
                            for (int i = 0; i < 13; i++)
                            {
                                string lineData = lines[i].Substring(lines[i].IndexOf(":") + 2).Trim();
                                if (i == 0)
                                {
                                    postData["Temperature"] = lineData;
                                }
                                else if (i == 1)
                                {
                                    postData["Humidity"] = lineData;
                                }
                                else if (i == 2)
                                {
                                    postData["Pressure"] = lineData;
                                }
                                else if (i == 3)
                                {
                                    postData["Altitude"] = lineData;
                                }
                                else if (i == 4)
                                {
                                    postData["Wind"] = lineData;
                                }
                                else if (i == 5)
                                {
                                    postData["Gust"] = lineData;
                                }
                                else if (i == 6)
                                {
                                    postData["Rain"] = lineData;
                                }
                                else if (i == 7)
                                {
                                    postData["Battery"] = lineData;
                                }
                                else if (i == 8)
                                {
                                    postData["Solar"] = lineData;
                                }
                                else if (i == 9)
                                {
                                    postData["Direction"] = lineData;
                                }
                                else if (i == 10)
                                {
                                    postData["Temperature180"] = lineData;
                                }
                                else if (i == 11)
                                {
                                    postData["Veml6070"] = lineData;
                                }
                                else if (i == 12)
                                {
                                    postData["Lux"] = lineData;
                                }
                                else if (i == 13)
                                {
                                    postData["TemperatureDHT22"] = lineData;
                                }
                                else if (i == 14)
                                {
                                    postData["HumidityDHT22"] = lineData;
                                }
                            }
                            using (var client = new WebClient())
                            {

                                var postResponse = client.UploadValues("http://localhost:8081/HomeOutsideWeatherStation/Data/AddData", postData);

                                var postResponseString = Encoding.Default.GetString(postResponse);
                                Console.WriteLine(postResponseString);
                                tryAgain = false;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error");

                    }
                }

                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
