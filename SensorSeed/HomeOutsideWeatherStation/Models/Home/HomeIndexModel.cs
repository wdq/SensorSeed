using HomeOutsideWeatherStation.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Innovative.SolarCalculator;
using SunSetRiseLib;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeIndexModel
    {
        /*
        *   Top left
        */

        // Elevation, coordinates, last updated
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double StationElevation { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double StationLatitude { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double StationLongitude { get; set; }
        public string StationLastUpdated { get; set; }

        // Condition icon/text, 
        public string CurrentCondition { get; set; }

        // Temperature, feels like
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentTemperature { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentTemperatureFeelsLike { get; set; }

        // Wind speed, direction, gusts
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentWindSpeed { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentWindDirection { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentWindGusts { get; set; }

        // Today recorded minimum and maximum temperatures, rain total
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double? TodayTemperatureMaximum { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double? TodayTemperatureMinimum { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double TodayRainTotal { get; set; }

        // Yesterday recorded minimum and maximum temperatures, rain total
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double YesterdayTemperatureMaximum { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double YesterdayTemperatureMinimum { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double YesterdayRainTotal { get; set; }

        /*
        *   Top right
        */
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentPressure { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentVisibility { get; set; }
        public string CurrentClouds { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentHeatIndex { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentDewPoint { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentHumidity { get; set; }
        public int CurrentUV { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentLux { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentRainfall { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CurrentSnowDepth { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public string MoonType { get; set; }
        public double MoonVisible { get; set; }

        /*
        *   Middle
        */

        public HomeTenDayHistoryGraphModel TenDayHistoryGraph { get; set; }


        /*
        *   Bottom
        */

        // History, note: a lot of these will be meaningless until I have over a year of data
        // TodayTemperatureMaximum
        // TodayTemperatureMinimum
        public double TemperatureAverageMaximum { get; set; }
        public double TemperatureAverageMinimum { get; set; }
        public double TemperatureMaximumRangeBottom { get; set; }
        public double TemperatureMaximumRangeTop { get; set; }
        public double TemperatureMinimumRangeBottom { get; set; }
        public double TemperatureMinimumRangeTop { get; set; }
        // TodayRainTotal
        public double RainAverage { get; set; }
        public double RainRangeBottom { get; set; } // todo: rain vs snow, I'll need to add in snow before winter, snow measurements will require additional hardware (maybe a heater for the rain gauge), can use temperature to determine if the heater needs to run, and if it is snow or rain
        public double RainRangeTop { get; set; }
        public double DewPointMaximum { get; set; }
        public double DewPointMinimum { get; set; }
        public double DewPointAverageMaximum { get; set; }
        public double DewPointAverageMinimum { get; set; }
        public double DewPointMaximumRangeBottom { get; set; }
        public double DewPointMaximumRangeTop { get; set; }
        public double DewPointMinimumRangeBottom { get; set; }
        public double DewPointMinimumRangeTop { get; set; }
        // YesterdayTemperatureMaximum
        // YesterdayTemperatureMinimum
        public double YesterdayTemperatureMaximumAverage { get; set; }
        public double YesterdayTemperatureMinimumAverage { get; set; }
        public double YesterdayTemperatureMaximumRecord { get; set; }
        public DateTime YesterdayTemperatureMaximumRecordTimestamp { get; set; } 
        public double YesterdayTemperatureMinimumRecord { get; set; }
        public DateTime YesterdayTemperatureMinimumRecordTimestamp { get; set; }

        public HomeIndexModel(DateTime endDate)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            HomeOutsideWeatherStationData currentData = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp < endDate).OrderByDescending(x => x.Timestamp).FirstOrDefault();
            StationElevation = (double)database.HomeOutsideWeatherStationDatas.Average(x => x.Altitude);
            StationLatitude = 40.815987372770905;
            StationLongitude = -96.61160876043141;
            StationLastUpdated = currentData.Timestamp.ToLocalTime().ToString();
            CurrentCondition = "Stuff"; // todo: make some sort of algorithm that takes in the data and returns a condition, or get this from an Internet source
            double currentTemperatureTemp = 0.00;
            int currentTemperatureTempCount = 0;
            if (currentData.Temperature.HasValue)
            {
                currentTemperatureTemp += (double) currentData.Temperature.Value;
                currentTemperatureTempCount++;
            }
            if (currentData.Temperature180.HasValue)
            {
                currentTemperatureTemp += (double) currentData.Temperature180.Value;
                currentTemperatureTempCount++;
            }
            if (currentData.TemperatureDHT22.HasValue)
            {
                currentTemperatureTemp += (double) currentData.TemperatureDHT22.Value;
                currentTemperatureTempCount++;
            }
            if (currentTemperatureTempCount == 0)
            {
                currentTemperatureTempCount = 1;
            }
            CurrentTemperature = currentTemperatureTemp / currentTemperatureTempCount;
            CurrentTemperatureFeelsLike = WeatherDataConversions.WindChill(CurrentTemperature, (double)currentData.WindSpeed); // todo: should this be wind chill, heat index, something else?
            CurrentWindSpeed = (double)currentData.WindSpeed;
            if (currentData.WindDirection == null)
            {
                CurrentWindDirection = -1;
            }
            else
            {
                CurrentWindDirection = (double)currentData.WindDirection;
            }
            if (currentData.GustSpeed == null)
            {
                CurrentWindGusts = -1;
            }
            else
            {
                CurrentWindGusts = (double)currentData.GustSpeed;
            }

            DateTime startOfToday = endDate.Date.ToUniversalTime();
            DateTime endOfToday = endDate.Date.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> todayData = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfToday && x.Timestamp < endOfToday).ToList();
            TodayTemperatureMaximum = (double?)todayData.Where(x => x.Temperature != null).Max(x => x.Temperature); // todo: probably have a field for the times that these happen too
            if (!TodayTemperatureMaximum.HasValue)
            {
                TodayTemperatureMaximum = (double?)todayData.Where(x => x.Temperature180 != null).Max(x => x.Temperature180);
            }
            if (!TodayTemperatureMaximum.HasValue)
            {
                TodayTemperatureMaximum = (double?)todayData.Where(x => x.TemperatureDHT22 != null).Max(x => x.TemperatureDHT22);
            }
            TodayTemperatureMinimum = (double?)todayData.Where(x => x.Temperature != null).Min(x => x.Temperature);
            if (!TodayTemperatureMinimum.HasValue)
            {
                TodayTemperatureMinimum = (double?)todayData.Where(x => x.Temperature180 != null).Min(x => x.Temperature180);
            }
            if (!TodayTemperatureMinimum.HasValue)
            {
                TodayTemperatureMinimum = (double?)todayData.Where(x => x.TemperatureDHT22 != null).Min(x => x.TemperatureDHT22);
            }
            TodayRainTotal = (double)todayData.Select(x => x.Rain).Sum();

            DateTime startOfYesterday = startOfToday.AddDays(-1).ToUniversalTime();
            DateTime endOfYesterday = startOfToday.ToUniversalTime();
            List<HomeOutsideWeatherStationData> yesterdayData = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfYesterday && x.Timestamp < endOfYesterday).ToList();
            if (yesterdayData.Count > 0)
            {
                if (yesterdayData.Max(x => x.Temperature).HasValue)
                {
                    YesterdayTemperatureMaximum = (double)yesterdayData.Max(x => x.Temperature).Value;
                }
                else if (yesterdayData.Max(x => x.Temperature180).HasValue)
                {
                    YesterdayTemperatureMaximum = (double)yesterdayData.Max(x => x.Temperature180).Value;
                }
                else if (yesterdayData.Max(x => x.TemperatureDHT22).HasValue)
                {
                    YesterdayTemperatureMaximum = (double) yesterdayData.Max(x => x.TemperatureDHT22).Value;
                }
                else
                {
                    YesterdayTemperatureMaximum = -1;
                }

                if (yesterdayData.Min(x => x.Temperature).HasValue)
                {
                    YesterdayTemperatureMinimum = (double)yesterdayData.Min(x => x.Temperature).Value;
                }
                else if (yesterdayData.Min(x => x.Temperature180).HasValue)
                {
                    YesterdayTemperatureMinimum = (double)yesterdayData.Min(x => x.Temperature180).Value;
                }
                else if (yesterdayData.Min(x => x.TemperatureDHT22).HasValue)
                {
                    YesterdayTemperatureMinimum = (double)yesterdayData.Min(x => x.TemperatureDHT22).Value;
                }
                else
                {
                    YesterdayTemperatureMaximum = -1;
                }
                YesterdayRainTotal = (double) yesterdayData.Select(x => x.Rain).Sum();
            }
            else
            {
                YesterdayTemperatureMaximum = -1;
                YesterdayTemperatureMinimum = -1;
                YesterdayRainTotal = -1;
            }


            CurrentPressure = (double)currentData.Pressure;
            CurrentVisibility = 0; // todo: either find out how to calculate this, or what kind of sensor I need, or pull it from the Internet
            CurrentClouds = ""; // todo: either find out how to calculate this, or what kind of sensor I need, or pull it from the Internet
            

            if (currentData.Humidity.HasValue)
            {
                CurrentHumidity = (double) currentData.Humidity.Value;
            }
            else if (currentData.HumidityDHT22.HasValue)
            {
                CurrentHumidity = (double) currentData.HumidityDHT22.Value;
            }
            else
            {
                CurrentHumidity = -1;
            }

            CurrentHeatIndex = WeatherDataConversions.HeatIndex(CurrentTemperature, (double)CurrentHumidity);
            CurrentDewPoint = WeatherDataConversions.DewPoint(CurrentTemperature, (double)CurrentHumidity);
            CurrentUV = -1;
            CurrentLux = -1;
            CurrentRainfall = TodayRainTotal; // todo: this may need to be something else
            CurrentSnowDepth = 0; // todo: snow

            double JD = 0;
            int zone = -6;
            double latitude = 40.815987372770905;
            double longitude = -96.61160876043141;
            bool dst = false;

            JD = Util.calcJD(endDate.Date);
            double sunRise = Util.calcSunRiseUTC(JD, latitude, longitude);
            double sunSet = Util.calcSunSetUTC(JD, latitude, longitude);
            DateTime sunrise = Util.getDateTime(sunRise, zone, endDate.Date, dst).Value;
            DateTime sunset = Util.getDateTime(sunSet, zone, endDate.Date, dst).Value;

            Sunrise = sunrise;
            Sunset = sunset;
            MoonType = ""; // todo: moon stuff
            MoonVisible = 0;

            TenDayHistoryGraph = new HomeTenDayHistoryGraphModel(endDate); 

        }

    }
}