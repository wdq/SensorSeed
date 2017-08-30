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
using System.Text.RegularExpressions;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace HomeOutsideWeatherStationDataService
{
    class Program
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static void UploadHistoryToWunderground()
        {
            using (var context = new SensorSeedDataContext())
            {
                var rows = context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp <= DateTime.Parse("2016-04-18").Date.AddHours(24)).OrderByDescending(x => x.Timestamp);
                foreach (var row in rows)
                {
                    UploadToWunderground(row);
                }
            }
        }

        private static void UploadToWunderground(HomeOutsideWeatherStationData data)
        {
            using (var context = new SensorSeedDataContext())
            {
                double? humidity = null;
                double? temperature = null;

                string url = "https://weatherstation.wunderground.com/weatherstation/updateweatherstation.php";
                url += "?ID=KNELINCO88";
                url += "&PASSWORD=";
                url += "&dateutc=" + data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                if (data.WindDirection.HasValue)
                {
                    url += "&winddir=" + Convert.ToInt32(data.WindDirection.Value);
                }
                if (data.WindSpeed.HasValue)
                {
                    url += "&windspeedmph=" + Convert.ToDouble(data.WindSpeed.Value) * 0.621371;
                    url += "&winddir_avg2m=" + Convert.ToDouble(data.WindSpeed.Value) * 0.621371;
                }
                if (data.GustSpeed.HasValue)
                {
                    url += "&windgustmph=" + Convert.ToDouble(data.GustSpeed.Value) * 0.621371;
                }
                if (context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > (data.Timestamp.AddMinutes(-10)) && x.Timestamp <= data.Timestamp).Average(x => x.GustSpeed).HasValue)
                {
                    url += "&windgustmph_10m=" + Convert.ToDouble(context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > (data.Timestamp.AddMinutes(-10)) && x.Timestamp <= data.Timestamp).Average(x => x.GustSpeed)) * 0.621371;
                }
                if (data.Humidity.HasValue || data.HumidityDHT22.HasValue)
                {
                    double humidityAverage = 0;
                    int humidityPointCount = 0;
                    if (data.Humidity.HasValue)
                    {
                        humidityAverage += Convert.ToDouble(data.Humidity.Value);
                        humidityPointCount++;
                    }
                    if (data.HumidityDHT22.HasValue)
                    {
                        humidityAverage += Convert.ToDouble(data.HumidityDHT22.Value);
                        humidityPointCount++;
                    }
                    url += "&humidity=" + humidityAverage / humidityPointCount;
                    humidity = humidityAverage / humidityPointCount;
                }
                if (data.Temperature.HasValue || data.Temperature180.HasValue || data.TemperatureDHT22.HasValue)
                {
                    double temperatureAverage = 0;
                    double temperaturePointCount = 0;
                    if (data.Temperature.HasValue)
                    {
                        temperatureAverage += Convert.ToDouble(data.Temperature.Value);
                        temperaturePointCount++;
                    }
                    if (data.Temperature180.HasValue)
                    {
                        temperatureAverage += Convert.ToDouble(data.Temperature180.Value);
                        temperaturePointCount++;
                    }
                    if (data.TemperatureDHT22.HasValue)
                    {
                        temperatureAverage += Convert.ToDouble(data.TemperatureDHT22.Value);
                        temperaturePointCount++;
                    }
                    url += "&tempf=" + (((temperatureAverage / temperaturePointCount) * 1.8) + 32);
                    temperature = ((temperatureAverage / temperaturePointCount));
                }
                if (context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > data.Timestamp.AddMinutes(-60) && x.Timestamp <= data.Timestamp).Sum(x => x.Rain).HasValue)
                {
                    url += "&rainin=" + Convert.ToDouble(context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > data.Timestamp.AddMinutes(-60) && x.Timestamp <= data.Timestamp).Sum(x => x.Rain)) * 0.0393701;
                }
                if (context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > data.Timestamp.Date && x.Timestamp <= data.Timestamp).Sum(x => x.Rain).HasValue)
                {
                    url += "&dailyrainin=" + Convert.ToDouble(context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > data.Timestamp.Date && x.Timestamp <= data.Timestamp).Sum(x => x.Rain)) * 0.0393701;
                }
                if (data.Pressure.HasValue)
                {
                    url += "&baromin=" + Convert.ToDouble(data.Pressure.Value) * 0.029529983071445; // might be wrong conversion factor, todo:
                }
                if (temperature.HasValue && humidity.HasValue)
                {
                    url += "&dewptf=" + ((DewPoint(temperature.Value, humidity.Value) * 1.8) + 32);
                }

                Console.WriteLine(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) ;
                Thread.Sleep(250);
            }

        }

        public static double DewPoint(double temperature, double humidity)
        {
            //double dewPoint = 0;

            // Dewpoint: https://en.wikipedia.org/wiki/Dew_point
            // Code taken from here: http://stackoverflow.com/a/27289801

            return (temperature - (14.55 + 0.114 * temperature) * (1 - (0.01 * humidity)) - Math.Pow(((2.5 + 0.007 * temperature) * (1 - (0.01 * humidity))), 3) - (15.9 + 0.117 * temperature) * Math.Pow((1 - (0.01 * humidity)), 14));
        }


        static void ImportOldCSV()
        {
            var csvLines = File.ReadLines(@"C:\Projects\mongoweather.csv");
            int lineCount = 0;
            foreach (var line in csvLines)
            {
                if (lineCount > 0)
                {
                    var fields = line.Split(',');
                    var guid = Guid.NewGuid();
                    double timestamp = Convert.ToDouble(fields[1]);
                    Decimal temperature = Convert.ToDecimal(fields[2]);
                    Decimal humidity = Convert.ToDecimal(fields[3]);

                    HomeOutsideWeatherStationData row = new HomeOutsideWeatherStationData();;
                    row.Id = guid;
                    row.Timestamp = UnixTimeStampToDateTime(timestamp/1000);
                    row.TemperatureDHT22 = temperature;
                    row.HumidityDHT22 = humidity;

                    using (var context = new SensorSeedDataContext())
                    {
                        context.HomeOutsideWeatherStationDatas.InsertOnSubmit(row);
                        context.SubmitChanges();
                    }

                    string url = "https://weatherstation.wunderground.com/weatherstation/updateweatherstation.php";
                    url += "?ID=KNELINCO88";
                    url += "&PASSWORD=";
                    url += "&dateutc=" + row.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    if (row.TemperatureDHT22.HasValue)
                    {
                        url += "&tempf=" + (row.TemperatureDHT22.Value*(decimal) 1.8) + (decimal) 32;
                    }
                    if (row.HumidityDHT22.HasValue)
                    {
                        url += "&humidity=" + row.HumidityDHT22.Value;
                    }
                    if (row.TemperatureDHT22.HasValue && row.HumidityDHT22.HasValue)
                    {
                        url += "&dewptf=" + ((DewPoint(Convert.ToDouble(row.TemperatureDHT22.Value), Convert.ToDouble(row.HumidityDHT22.Value)) * 1.8) + 32);
                    }

                    Console.WriteLine(url);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) ;
                    Thread.Sleep(250);

                }
                lineCount++;
            }
        }

        static void Main(string[] args)
        {
            //UploadHistoryToWunderground();
            ImportOldCSV();
        }

        public static string AddData(DateTime Timestamp, string Temperature, string Humidity, string Pressure, string Altitude, string Wind, string Gust, string Rain, string Battery, string Solar, string Direction, string Temperature180, string TemperatureDHT22, string HumidityDHT22, string Veml6070, string Lux)
        {
            Temperature = String.Join("", Temperature.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Humidity = String.Join("", Humidity.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Pressure = String.Join("", Pressure.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Altitude = String.Join("", Altitude.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Wind = String.Join("", Wind.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Wind = Wind.TrimEnd('.');
            Gust = String.Join("", Gust.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Gust = Gust.Replace("." + Wind + ".", "");
            Rain = String.Join("", Rain.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Rain = Rain.TrimEnd('.');
            Battery = String.Join("", Battery.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Battery = Battery.TrimEnd('.');
            Solar = String.Join("", Solar.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Solar = Solar.TrimEnd('.');
            Direction = String.Join("", Direction.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            Direction = Direction.TrimEnd('.');
            Temperature180 = String.Join("", Temperature180.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            TemperatureDHT22 = String.Join("", TemperatureDHT22.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));
            HumidityDHT22 = String.Join("", HumidityDHT22.Where(x => Char.IsDigit(x) || x == '.' || x == '-'));

            SensorSeedDataContext database = new SensorSeedDataContext();

            HomeOutsideWeatherStationData data = new HomeOutsideWeatherStationData();

            data.Id = Guid.NewGuid();
            data.Timestamp = Timestamp;

            var possibleExistingRecords = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp == Timestamp);

            if (possibleExistingRecords.Any())
            {
                return "Record exists";
            }
            else
            {
                // Temperature
                try
                {
                    decimal TemperatureDecimal = 0;
                    TemperatureDecimal = Convert.ToDecimal(Temperature);
                    data.Temperature = TemperatureDecimal;
                }
                catch (Exception exception)
                {
                    // return "Error parsing temperature";
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
                    //return "Error parsing humidity";
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
                    //return "Error parsing pressure";
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
                    //return "Error parsing altitude";
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
                    //return "Error parsing wind speed";
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
                    //return "Error parsing gust speed";
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
                    //return "Error parsing rain";
                }

                // Battery
                try
                {
                    decimal BatteryDecimal = 0;
                    BatteryDecimal = Convert.ToDecimal(Battery);
                    BatteryDecimal = BatteryDecimal / (decimal)1024; // Max ADC value
                    BatteryDecimal = BatteryDecimal * (decimal)5; // High voltage
                    BatteryDecimal = BatteryDecimal * (decimal)3; // Voltage divider factor
                    data.Battery = BatteryDecimal;
                }
                catch (Exception exception)
                {
                    //return "Error parsing battery";
                }

                // Solar
                try
                {
                    decimal SolarDecimal = 0;
                    SolarDecimal = Convert.ToDecimal(Solar);
                    SolarDecimal = SolarDecimal / (decimal)1024; // Max ADC value
                    SolarDecimal = SolarDecimal * (decimal)5; // High voltage
                    SolarDecimal = SolarDecimal * (decimal)3; // Voltage divider factor
                    data.Solar = SolarDecimal;
                }
                catch (Exception exception)
                {
                    //return "Error parsing solar";
                }

                // Wind Direction
                try
                {
                    decimal DirectionDegreesDecimal = 0;
                    decimal DirectionDecimal = 0;
                    DirectionDecimal = Convert.ToDecimal(Direction);
                    decimal r1 = 10000;
                    decimal r2 = 0; // Unknown
                    decimal vin = (decimal)5;
                    decimal vout = ((DirectionDecimal / (decimal)1024) * (decimal)5);
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
                    //return "Error parsing wind direction";
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
                    //return "Error parsing temperature180";
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
                    //return "Error parsing veml6070";
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
                    //return "Error parsing lux";
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
                    //return "Error parsing temperaturedht22";
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
                    //return "Error parsing humidity dht22";
                }


                database.HomeOutsideWeatherStationDatas.InsertOnSubmit(data);
                database.SubmitChanges();

                return "ok";
            }
        }

    }
}
