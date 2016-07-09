using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Models.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeTenDayHistoryGraphDayModel
    {
        public DateTime Date { get; set; }
        public double? TemperatureHigh { get; set; }
        public double? TemperatureLow { get; set; }
        public string Condition { get; set; }
        public double? Rain { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

        public HomeTenDayHistoryGraphDayModel(DateTime date, double? temperatureHigh, double? temperatureLow, string condition, double? rain, DateTime sunrise, DateTime sunset)
        {
            Date = date;
            TemperatureHigh = temperatureHigh;
            TemperatureLow = temperatureLow;
            Condition = condition;
            Rain = rain;
            Sunrise = sunrise;
            Sunset = sunset;
        }

        public HomeTenDayHistoryGraphDayModel(List<HomeOutsideWeatherStationData> dayDatas)
        {
            if (dayDatas.Count > 0)
            {
                Date = dayDatas.FirstOrDefault().Timestamp.ToLocalTime();
                TemperatureHigh = (double?) dayDatas.Where(x => x.Temperature != null).Max(x => x.Temperature);
                TemperatureLow = (double?) dayDatas.Where(x => x.Temperature != null).Min(x => x.Temperature);
                Condition = ""; // todo:
                Rain = (double?) dayDatas.Where(x => x.Rain != null).Sum(x => x.Rain);
                Sunrise = DateTime.Today; // todo:
                Sunset = DateTime.Today; // todo:            

            }
            else
            {
                Date = DateTime.MinValue;
                TemperatureHigh = -1;
                TemperatureLow = -1;
                Condition = ""; // todo:
                Rain = -1;
                Sunrise = DateTime.Today; // todo:
                Sunset = DateTime.Today; // todo:            
            }
            }
    }
}