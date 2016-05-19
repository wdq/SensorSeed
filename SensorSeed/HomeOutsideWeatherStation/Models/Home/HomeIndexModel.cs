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
        public double StationElevation { get; set; }
        public double StationLatitude { get; set; }
        public double StationLongitude { get; set; }
        public DateTime StationLastUpdated { get; set; }

        // Condition icon/text, 
        public string CurrentCondition { get; set; }
        
        // Temperature, feels like
        public double CurrentTemperature { get; set; }
        public double CurrentTemperatureFeelsLike { get; set; }

        // Wind speed, direction, gusts
        public double CurrentWindSpeed { get; set; }
        public double CurrentWindDirection { get; set; }
        public double CurrentWindGusts { get; set; }

        // Today recorded minimum and maximum temperatures, rain total
        public double TodayTemperatureMaximum { get; set; }
        public double TodayTemperatureMinimum { get; set; }
        public double TodayRainTotal { get; set; }

        // Yesterday recorded minimum and maximum temperatures, rain total
        public double YesterdayTemperatureMaximum { get; set; }
        public double YesterdayTemperatureMinimum { get; set; }
        public double YesterdayRainTotal { get; set; }

        /*
        *   Top right
        */

        public double CurrentPressure { get; set; }
        public double CurrentVisibility { get; set; }
        public string CurrentClouds { get; set; }
        public double CurrentHeatIndex { get; set; }
        public double CurrentDewPoint { get; set; }
        public double CurrentHumidity { get; set; }
        public double CurrentRainfall { get; set; }
        public double CurrentSnowDepth { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public string MoonType { get; set; }
        public double MoonVisible { get; set; }

        /*
        *   Middle
        */

        public List<HomeTenDayHistoryGraphModel> TenDayHistoryGraph { get; set; }


        /*
        *   Bottom
        */


    }
}