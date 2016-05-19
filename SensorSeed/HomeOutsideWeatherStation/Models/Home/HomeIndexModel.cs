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


    }
}