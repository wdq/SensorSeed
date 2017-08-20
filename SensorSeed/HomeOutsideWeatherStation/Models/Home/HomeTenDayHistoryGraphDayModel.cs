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
                if (dayDatas.Where(x => x.Temperature != null).Max(x => x.Temperature180).HasValue)
                {
                    TemperatureHigh = (double) dayDatas.Where(x => x.Temperature != null).Max(x => x.Temperature180).Value;
                }
                else if (dayDatas.Where(x => x.Temperature180 != null).Max(x => x.Temperature180).HasValue)
                {
                    TemperatureHigh = (double)dayDatas.Where(x => x.Temperature180 != null).Max(x => x.Temperature180).Value;
                }
                else if (dayDatas.Where(x => x.TemperatureDHT22 != null).Max(x => x.TemperatureDHT22).HasValue)
                {
                    TemperatureHigh = (double) dayDatas.Where(x => x.TemperatureDHT22 != null).Max(x => x.TemperatureDHT22).Value;
                }
                else
                {
                    TemperatureHigh = -1;
                }

                if (dayDatas.Where(x => x.Temperature != null).Min(x => x.Temperature180).HasValue)
                {
                    TemperatureLow = (double)dayDatas.Where(x => x.Temperature != null).Min(x => x.Temperature180).Value;
                }
                else if (dayDatas.Where(x => x.Temperature180 != null).Min(x => x.Temperature180).HasValue)
                {
                    TemperatureLow = (double)dayDatas.Where(x => x.Temperature180 != null).Min(x => x.Temperature180).Value;
                }
                else if (dayDatas.Where(x => x.TemperatureDHT22 != null).Min(x => x.TemperatureDHT22).HasValue)
                {
                    TemperatureLow = (double)dayDatas.Where(x => x.TemperatureDHT22 != null).Min(x => x.TemperatureDHT22).Value;
                }
                else
                {
                    TemperatureLow = -1;
                }
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