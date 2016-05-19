using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Models.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeTenDayHistoryGraphModel
    {
        public DateTime Date { get; set; }
        public double TemperatureHigh { get; set; }
        public double TemperatureLow { get; set; }
        public string Condition { get; set; }
        public double Rain { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public TimeGraphModel TemperatureGraph { get; set; }
        public TimeGraphModel PressureGraph { get; set; }
        public TimeGraphModel RainfallGraph { get; set; }
        public WindGraphModel WindSpeedGraph { get; set; }
    }
}