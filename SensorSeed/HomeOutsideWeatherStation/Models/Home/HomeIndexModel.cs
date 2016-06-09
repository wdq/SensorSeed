using HomeOutsideWeatherStation.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Innovative.SolarCalculator;

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
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double StationLastUpdated { get; set; }

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

        public HomeIndexModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            HomeOutsideWeatherStationData currentData = database.HomeOutsideWeatherStationDatas.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            StationElevation = (double)database.HomeOutsideWeatherStationDatas.Average(x => x.Altitude);
            StationLatitude = 40.815987372770905;
            StationLongitude = -96.61160876043141;
            StationLastUpdated = (DateTime.Now - currentData.Timestamp.ToLocalTime()).TotalSeconds;
            CurrentCondition = "Stuff"; // todo: make some sort of algorithm that takes in the data and returns a condition, or get this from an Internet source
            CurrentTemperature = (((double)currentData.Temperature + (double)currentData.Temperature180) / 2);
            CurrentTemperatureFeelsLike = WeatherDataConversions.WindChill(CurrentTemperature, (double)currentData.WindSpeed); // todo: should this be wind chill, heat index, something else?
            CurrentWindSpeed = (double)currentData.WindSpeed;
            CurrentWindDirection = (double)currentData.WindDirection;
            CurrentWindGusts = (double)currentData.GustSpeed;

            DateTime startOfToday = DateTime.Today.ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> todayData = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfToday && x.Timestamp < endOfToday).ToList();
            TodayTemperatureMaximum = (double?)todayData.Where(x => x.Temperature != null).Max(x => x.Temperature); // todo: probably have a field for the times that these happen too
            TodayTemperatureMinimum = (double?)todayData.Where(x => x.Temperature != null).Min(x => x.Temperature);
            TodayRainTotal = (double)todayData.Select(x => x.Rain).Sum();

            DateTime startOfYesterday = DateTime.Today.AddDays(-1).ToUniversalTime();
            DateTime endOfYesterday = DateTime.Today.ToUniversalTime();
            List<HomeOutsideWeatherStationData> yesterdayData = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfYesterday && x.Timestamp < endOfYesterday).ToList();
            YesterdayTemperatureMaximum = (double)yesterdayData.Max(x => x.Temperature);
            YesterdayTemperatureMinimum = (double)yesterdayData.Min(x => x.Temperature);
            YesterdayRainTotal = (double)yesterdayData.Select(x => x.Rain).Sum();

            CurrentPressure = (double)currentData.Pressure;
            CurrentVisibility = 0; // todo: either find out how to calculate this, or what kind of sensor I need, or pull it from the Internet
            CurrentClouds = ""; // todo: either find out how to calculate this, or what kind of sensor I need, or pull it from the Internet
            CurrentHeatIndex = WeatherDataConversions.HeatIndex(CurrentTemperature, (double)currentData.Humidity);
            CurrentDewPoint = WeatherDataConversions.DewPoint(CurrentTemperature, (double)currentData.Humidity);
            CurrentHumidity = (double)currentData.Humidity;
            CurrentUV = (int)currentData.Veml6070;
            CurrentLux = (double) currentData.Lux;
            CurrentRainfall = TodayRainTotal; // todo: this may need to be something else
            CurrentSnowDepth = 0; // todo: snow
            TimeZoneInfo cst = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            SolarTimes solarTimes = new SolarTimes(DateTime.Today, 40.815999, -96.611661);
            DateTime sunrise = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunrise.ToUniversalTime(), cst);
            DateTime sunset = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunset.ToUniversalTime(), cst); 
            Sunrise = sunrise;
            Sunset = sunset;
            MoonType = ""; // todo: moon stuff
            MoonVisible = 0;

            TenDayHistoryGraph = new HomeTenDayHistoryGraphModel(); 

        }

    }
}