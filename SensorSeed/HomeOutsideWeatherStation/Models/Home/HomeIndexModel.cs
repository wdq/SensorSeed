using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeIndexModel
    {
        /*
        *   Top left
        */

        // Elevation, coordinates, last updated
        public string StationElevation { get; set; }
        public string StationLatitude { get; set; }
        public string StationLongitude { get; set; }
        public string StationLastUpdated { get; set; }

        // Condition icon/text, 
        
        // Temperature, feels like
        public string CurrentTemperature { get; set; }
        public string CurrentTemperatureFeelsLike { get; set; }

        // Wind speed, direction, gusts
        public string CurrentWindSpeed { get; set; }
        public string CurrentWindDirection { get; set; }
        public string CurrentWindGusts { get; set; }

        // Today recorded minimum and maximum temperatures, rain total
        public string TodayTemperatureMaximum { get; set; }
        public string TodayTemperatureMinimum { get; set; }
        public string TodayRainTotal { get; set; }

        // Yesterday recorded minimum and maximum temperatures, rain total
        public string YesterdayTemperatureMaximum { get; set; }
        public string YesterdayTemperatureMinimum { get; set; }
        public string YesterdayRainTotal { get; set; }

        /*
        *   Top right
        */

        public string CurrentPressure { get; set; }
        public string CurrentVisibility { get; set; }
        public string CurrentClouds { get; set; }
        public string CurrentHeatIndex { get; set; }
        public string CurrentDewPoint { get; set; }
        public string CurrentHumidity { get; set; }
        public string CurrentRainfall { get; set; }
        public string CurrentSnowDepth { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string MoonType { get; set; }
        public string MoonVisible { get; set; }
    }
}