using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Controllers.DataController
{
    public class DataController : Controller
    {
        [HttpGet]
        public string Get(string Event, string data, string published_at, string coreid)
        {
            string strippedData = HttpUtility.UrlDecode(data);
            var splitData = strippedData.Split(',');
            string addDataResult = AddData(UnixTimeStampToDateTime(double.Parse(splitData[0])), splitData[1], splitData[2], splitData[3], splitData[4], splitData[5], splitData[6], splitData[7], splitData[8], splitData[9], splitData[10], splitData[11], splitData[12], splitData[13], "", "", splitData[14], splitData[15]);
            return addDataResult;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        private static void UploadToWunderground(HomeOutsideWeatherStationData data)
        {
            using (var context = new SensorSeedDataContext())
            {
                decimal? humidity = null;
                decimal? temperature = null;

                string url = "https://rtupdate.wunderground.com/weatherstation/updateweatherstation.php";
                url += "?ID=KNELINCO88";
                url += "&PASSWORD=";
                url += "&dateutc=" + data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                if (data.WindDirection.HasValue)
                {
                    url += "&winddir=" + Convert.ToInt32(data.WindDirection.Value);
                }
                if (data.WindSpeed.HasValue)
                {
                    url += "&windspeedmph=" + data.WindSpeed.Value*(decimal)0.621371;
                    url += "&winddir_avg2m=" + data.WindSpeed.Value*(decimal)0.621371;
                }
                if (data.GustSpeed.HasValue)
                {
                    url += "&windgustmph=" + data.GustSpeed.Value*(decimal)0.621371;
                }
                if (context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > (DateTime.UtcNow.AddMinutes(-10))).Average(x => x.GustSpeed).HasValue)
                {
                    url += "&windgustmph_10m=" + context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > (DateTime.UtcNow.AddMinutes(-10))).Average(x => x.GustSpeed) * (decimal)0.621371;
                }
                if (data.Humidity.HasValue || data.HumidityDHT22.HasValue)
                {
                    decimal humidityAverage = 0;
                    decimal humidityPointCount = 0;
                    if (data.Humidity.HasValue)
                    {
                        humidityAverage += data.Humidity.Value;
                        humidityPointCount++;
                    }
                    if (data.HumidityDHT22.HasValue)
                    {
                        humidityAverage += data.HumidityDHT22.Value;
                        humidityPointCount++;
                    }
                    url += "&humidity=" + humidityAverage / humidityPointCount;
                    humidity = humidityAverage/humidityPointCount;
                }
                if (data.Temperature.HasValue || data.Temperature180.HasValue || data.TemperatureDHT22.HasValue)
                {
                    decimal temperatureAverage = 0;
                    decimal temperaturePointCount = 0;
                    if (data.Temperature.HasValue)
                    {
                        temperatureAverage += data.Temperature.Value;
                        temperaturePointCount++;
                    }
                    if (data.Temperature180.HasValue)
                    {
                        temperatureAverage += data.Temperature180.Value;
                        temperaturePointCount++;
                    }
                    if (data.TemperatureDHT22.HasValue)
                    {
                        temperatureAverage += data.TemperatureDHT22.Value;
                        temperaturePointCount++;
                    }
                    url += "&tempf=" + (((temperatureAverage/temperaturePointCount) * (decimal)1.8) + (decimal)32);
                    temperature = ((temperatureAverage/temperaturePointCount));
                }
                if (context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > DateTime.UtcNow.AddMinutes(-60)).Sum(x => x.Rain).HasValue)
                {
                    url += "&rainin=" + context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > DateTime.UtcNow.AddMinutes(-60)).Sum(x => x.Rain) * (decimal)0.0393701;
                }
                if (context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > DateTime.Today.ToUniversalTime()).Sum(x => x.Rain).HasValue)
                {
                    url += "&dailyrainin=" + context.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > DateTime.Today.ToUniversalTime()).Sum(x => x.Rain) * (decimal)0.0393701;
                }
                if (data.Pressure.HasValue)
                {
                    // For some reason the wunderground API changes the pressure when it is uploaded, this offsets that.
                    decimal offsetPressureValue = data.Pressure.Value - (decimal)43.69;
                    url += "&baromin=" + offsetPressureValue * (decimal)0.029529983071445;
                }
                if (temperature.HasValue && humidity.HasValue)
                {
                    url += "&dewptf=" + ((WeatherDataConversions.DewPoint((double)temperature.Value, (double)humidity.Value) * 1.8) + 32);
                }
                url += "&realtime=1&rtfreq=155";

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse()) ;
            }

        }

        public static string AddData(DateTime Timestamp, string Temperature, string Humidity, string Pressure, string Altitude, string Wind, string Gust, string Rain, string Battery, string Solar, string Direction, string Temperature180, string TemperatureDHT22, string HumidityDHT22, string Veml6070, string Lux, string PacketRSSI, string NumberOfPackets)
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
                if (Temperature == "0.00" && Humidity == "0.00")
                {
                    // These values are when the sensor isn't connected

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
                    SolarDecimal = SolarDecimal * (decimal)5; // Voltage divider factor
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

                    // Mapping the 0-360 output to the actual 0-360 based on direction
                    // Info: 
                    //          East reported from the sensor is physically north
                    //          South reported from the sensor is physically east
                    // That means the physical direction is the sensor direction -90 (or +270 in cases where the result would be negative)
                    // Todo: Update all data in database added before 8/24/17 at 7:55 PM with this math.

                    if (DirectionDegreesDecimal > 90)
                    {
                        DirectionDegreesDecimal -= 90;
                    }
                    else
                    {
                        DirectionDegreesDecimal += 270;
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

                // PacketRSSI
                try
                {
                    decimal PacketRSSIDecimal = 0;
                    PacketRSSIDecimal = Convert.ToDecimal(PacketRSSI);
                    data.PacketRSSI = PacketRSSIDecimal;
                }
                catch (Exception exception)
                {
                    //return "Error parsing packet RSSI";
                }

                // NumberOfPackets
                try
                {
                    int NumberOfPacketsInt = 0;
                    NumberOfPacketsInt = Convert.ToInt32(NumberOfPackets);
                    data.NumberOfPackets = NumberOfPacketsInt;
                }
                catch (Exception exception)
                {
                    //return "Error parsing number of packets";
                }

                database.HomeOutsideWeatherStationDatas.InsertOnSubmit(data);
                database.SubmitChanges();

                UploadToWunderground(data);

                return "ok";
            }
        }

    }
}