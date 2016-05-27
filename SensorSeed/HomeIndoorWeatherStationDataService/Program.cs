using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeIndoorWeatherStationDataService
{
    class Program
    {

        public static string[] WeatherStationBasementRoomAddress = { "10.0.10.4", "/dht2-temperature", "/dht2-humidity", "basement" };
        public static string[] WeatherStationLivingRoomAddress = { "10.0.10.5", "/dht2-temperature", "/dht2-humidity", "living room"};
        public static string[] WeatherStationServerRoomAddress = { "10.0.10.6", "/dht-temperature", "/dht-humidity", "server room"};
        public static string[] WeatherStationWilliamsRoomAddress = { "10.0.10.7", "/dht-temperature", "/dht-humidity", "William's room" };


        static double GetTemperature(string temperatureUrl)
        {
            string html = string.Empty;
            try
            {
                HttpWebRequest temperatureRequest = (HttpWebRequest)WebRequest.Create(temperatureUrl);
                temperatureRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                temperatureRequest.Timeout = 20000;

                using (HttpWebResponse temperatureResponse = (HttpWebResponse)temperatureRequest.GetResponse())

                using (Stream temperatureStream = temperatureResponse.GetResponseStream())
                using (StreamReader temperatureReader = new StreamReader(temperatureStream))
                {
                    html = temperatureReader.ReadToEnd();
                    if (html == "" || html == "nan")
                    {
                        return -10000;
                    }
                    return Convert.ToDouble(html);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                return -10000;
            }
        }

        static double GetHumidity(string humidityUrl)
        {
            string html = string.Empty;
            try
            {
                HttpWebRequest humidityRequest = (HttpWebRequest)WebRequest.Create(humidityUrl);
                humidityRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                humidityRequest.Timeout = 20000;

                using (HttpWebResponse humidityResponse = (HttpWebResponse)humidityRequest.GetResponse())

                using (Stream humidityStream = humidityResponse.GetResponseStream())
                using (StreamReader humidityReader = new StreamReader(humidityStream))
                {
                    html = humidityReader.ReadToEnd();
                    if (html == "" || html == "nan")
                    {
                        return -10000;
                    }
                    return Convert.ToDouble(html);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                return -10000;
            }
        }

        static void GetStationData(string[] WeatherStationAddress)
        {

            string temperatureUrl = @"http://" + WeatherStationAddress[0] + WeatherStationAddress[1];
            string humidityUrl = @"http://" + WeatherStationAddress[0] + WeatherStationAddress[2];

            bool tryAgain = true;
            while (tryAgain)
            {
                DateTime currentTime = DateTime.Now;

                Console.Write(currentTime.ToString());
                Console.Write(":    Getting " + WeatherStationAddress[3] + " sensor data...");

                var postData = new NameValueCollection();

                double temperature1 = GetTemperature(temperatureUrl);
                Thread.Sleep(3000); // the sensor needs time between readings
                double temperature2 = GetTemperature(temperatureUrl);
                Thread.Sleep(3000);
                double temperature3 =  GetTemperature(temperatureUrl);
                Thread.Sleep(3000);
                string temperature = "";
                if (temperature1 != -10000 && temperature2 != -10000 && temperature3 != -10000)
                {
                    temperature = ((temperature1 + temperature2 + temperature3)/3.0).ToString();
                }

                double humidity1 = GetTemperature(humidityUrl);
                Thread.Sleep(3000);
                double humidity2 = GetTemperature(humidityUrl);
                Thread.Sleep(3000);
                double humidity3 = GetTemperature(humidityUrl);
                Thread.Sleep(3000);
                string humidity = "";
                if (humidity1 != -10000 && humidity2 != -10000 && humidity3 != -10000)
                {
                    humidity = ((humidity1 + humidity2 + humidity3) / 3.0).ToString();
                }

                postData["StationIP"] = WeatherStationAddress[0];
                postData["Temperature"] = temperature;
                postData["Humidity"] = humidity;

                using (var client = new WebClient())
                {
                    var postResponse = client.UploadValues("http://localhost/SensorSeed/HomeIndoorWeatherStation/Data/AddData", postData);
                    var postResponseString = Encoding.Default.GetString(postResponse);
                    Console.WriteLine(postResponseString);
                    tryAgain = false;
                }
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                GetStationData(WeatherStationBasementRoomAddress);
                GetStationData(WeatherStationLivingRoomAddress);
                GetStationData(WeatherStationServerRoomAddress);
                GetStationData(WeatherStationWilliamsRoomAddress);

                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
