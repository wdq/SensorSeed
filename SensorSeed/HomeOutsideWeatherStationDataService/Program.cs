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
                string url = @"http://10.0.14.1/";
                string[] sensorData = new string[16];

                bool tryAgain = true;
                while (tryAgain)
                {
                    Console.Write(currentTime.ToString());
                    Console.Write(":    Getting sensor data...");
                    try
                    {
                        WebClient client = new WebClient();
                        client.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                        html = client.DownloadString(url);

                        int index = html.IndexOf("\n");
                        html = html.Substring(index + "\n".Length);
                        string[] lines = html.Split(new string[] { "\n" }, StringSplitOptions.None);
                        for (int i = 0; i < 15; i++)
                        {
                            string lineData = lines[i].Substring(lines[i].IndexOf(":") + 2).Trim();
                            sensorData[i] = lineData;
                        }

                        string addDataResult = AddData(sensorData[0], sensorData[1], sensorData[2], sensorData[3], sensorData[4], sensorData[5], sensorData[6], sensorData[7], sensorData[8], sensorData[9], sensorData[10], sensorData[11], sensorData[12], sensorData[13], sensorData[14]);
                        Console.WriteLine(addDataResult);
                        tryAgain = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);

                    }
                }

                Thread.Sleep(TimeSpan.FromMinutes(2.5));
            }
        }

        public static string AddData(string Temperature, string Humidity, string Pressure, string Altitude, string Wind, string Gust, string Rain, string Battery, string Solar, string Direction, string Temperature180, string Veml6070, string Lux, string TemperatureDHT22, string HumidityDHT22)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            HomeOutsideWeatherStationData data = new HomeOutsideWeatherStationData();

            data.Id = Guid.NewGuid();
            data.Timestamp = DateTime.UtcNow;

            // Temperature
            try
            {
                decimal TemperatureDecimal = 0;
                TemperatureDecimal = Convert.ToDecimal(Temperature);
                data.Temperature = TemperatureDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing temperature";
            }

            // Humidity
            try
            {
                decimal HumidityDecimal = 0;
                HumidityDecimal = Convert.ToDecimal(Humidity);
                data.Humidity = HumidityDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing humidity";
            }

            // Pressure
            try
            {
                decimal PressureDecimal = 0;
                PressureDecimal = Convert.ToDecimal(Pressure);
                data.Pressure = PressureDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing pressure";
            }

            // Altitude
            try
            {
                decimal AltitudeDecimal = 0;
                AltitudeDecimal = Convert.ToDecimal(Altitude);
                data.Altitude = AltitudeDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing altitude";
            }


            // WindSpeed
            try
            {
                decimal WindSpeedDecimal = 0;
                WindSpeedDecimal = Convert.ToDecimal(Wind);
                data.WindSpeed = WindSpeedDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing wind speed";
            }

            // GustSpeed
            try
            {
                decimal GustSpeedDecimal = 0;
                GustSpeedDecimal = Convert.ToDecimal(Gust);
                data.GustSpeed = GustSpeedDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing gust speed";
            }

            // Rain
            try
            {
                decimal RainDecimal = 0;
                RainDecimal = Convert.ToDecimal(Rain);
                data.Rain = RainDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing rain";
            }

            // Battery
            try
            {
                decimal BatteryDecimal = 0;
                BatteryDecimal = Convert.ToDecimal(Battery);
                BatteryDecimal = BatteryDecimal/(decimal) 1024; // Max ADC value
                BatteryDecimal = BatteryDecimal*(decimal) 3.3; // High voltage
                BatteryDecimal = BatteryDecimal*(decimal) 2; // Voltage divider factor
                data.Battery = BatteryDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing battery";
            }

            // Solar
            try
            {
                decimal SolarDecimal = 0;
                SolarDecimal = Convert.ToDecimal(Solar);
                SolarDecimal = SolarDecimal/(decimal) 1024; // Max ADC value
                SolarDecimal = SolarDecimal*(decimal) 3.3; // High voltage
                SolarDecimal = SolarDecimal*(decimal) 3; // Voltage divider factor
                data.Solar = SolarDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing solar";
            }

            // Wind Direction
            try
            {
                decimal DirectionDegreesDecimal = 0;
                decimal DirectionDecimal = 0;
                DirectionDecimal = Convert.ToDecimal(Direction);
                decimal r1 = 10000;
                decimal r2 = 0; // Unknown
                decimal vin = (decimal)3.3;
                decimal vout = ((DirectionDecimal / (decimal)1024) * (decimal)3.3);
                r2 = r1 * (1 / ((vin / vout) - 1));
                if (((decimal)300 <= r2) && (r2 < (decimal)790))
                {
                    DirectionDegreesDecimal = (decimal)112.5;
                }
                else if (((decimal)790 <= r2) && (r2 < (decimal)946))
                {
                    DirectionDegreesDecimal = (decimal)67.5;
                }
                else if (((decimal)946 <= r2) && (r2 < (decimal)1205))
                {
                    DirectionDegreesDecimal = (decimal)90;
                }
                else if (((decimal)1205 <= r2) && (r2 < (decimal)1805))
                {
                    DirectionDegreesDecimal = (decimal)157.5;
                }
                else if (((decimal)1805 <= r2) && (r2 < (decimal)2670))
                {
                    DirectionDegreesDecimal = (decimal)135;
                }
                else if (((decimal)2670 <= r2) && (r2 < (decimal)3520))
                {
                    DirectionDegreesDecimal = (decimal)202.5;
                }
                else if (((decimal)3520 <= r2) && (r2 < (decimal)5235))
                {
                    DirectionDegreesDecimal = (decimal)180;
                }
                else if (((decimal)5235 <= r2) && (r2 < (decimal)7385))
                {
                    DirectionDegreesDecimal = (decimal)22.5;
                }
                else if (((decimal)7385 <= r2) && (r2 < (decimal)11160))
                {
                    DirectionDegreesDecimal = (decimal)45;
                }
                else if (((decimal)11160 <= r2) && (r2 < (decimal)15060))
                {
                    DirectionDegreesDecimal = (decimal)247.5;
                }
                else if (((decimal)15060 <= r2) && (r2 < (decimal)18940))
                {
                    DirectionDegreesDecimal = (decimal)225;
                }
                else if (((decimal)18940 <= r2) && (r2 < (decimal)27440))
                {
                    DirectionDegreesDecimal = (decimal)337.5;
                }
                else if (((decimal)27440 <= r2) && (r2 < (decimal)37560))
                {
                    DirectionDegreesDecimal = (decimal)0;
                }
                else if (((decimal)37560 <= r2) && (r2 < (decimal)53510))
                {
                    DirectionDegreesDecimal = (decimal)292.5;
                }
                else if (((decimal)53510 <= r2) && (r2 < (decimal)92450))
                {
                    DirectionDegreesDecimal = (decimal)315;
                }
                else if (((decimal)92450 <= r2) && (r2 < (decimal)160000))
                {
                    DirectionDegreesDecimal = (decimal)270;
                }

                data.WindDirection = DirectionDegreesDecimal;

            }
            catch (Exception exception)
            {
                return "Error parsing wind direction";
            }

            // Temperature180
            try
            {
                decimal TemperatureDecimal = 0;
                TemperatureDecimal = Convert.ToDecimal(Temperature180);
                data.Temperature180 = TemperatureDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing temperature180";
            }

            // Veml6070
            try
            {
                int Veml6070Int = 0;
                Veml6070Int = Convert.ToInt32(Veml6070);
                data.Veml6070 = Veml6070Int;
            }
            catch (Exception exception)
            {
                return "Error parsing veml6070";
            }

            // Lux
            try
            {
                decimal LuxDecimal = 0;
                LuxDecimal = Convert.ToDecimal(Lux);
                data.Lux = LuxDecimal;
            }
            catch (Exception exception)
            {
                return "Error parsing lux";
            }

            // TemperatureDHT22
            try
            {
                decimal TemperatureDecimalDHT22 = 0;
                TemperatureDecimalDHT22 = Convert.ToDecimal(TemperatureDHT22);
                data.TemperatureDHT22 = TemperatureDecimalDHT22;
            }
            catch (Exception exception)
            {
                return "Error parsing temperaturedht22";
            }

            // HumidityDHT22
            try
            {
                decimal HumidityDecimalDHT22 = 0;
                HumidityDecimalDHT22 = Convert.ToDecimal(HumidityDHT22);
                data.HumidityDHT22 = HumidityDecimalDHT22;
            }
            catch (Exception exception)
            {
                return "Error parsing humidity dht22";
            }


            database.HomeOutsideWeatherStationDatas.InsertOnSubmit(data);
            database.SubmitChanges();

            return "ok";
        }

    }
}
