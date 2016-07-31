using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeHumidityPressureDataPointModel
    {
        public string Timestamp { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }

        public HomeHumidityPressureDataPointModel(HomeOutsideWeatherStationData data)
        {
            Timestamp = data.Timestamp.ToString();
            Humidity = (double)data.Humidity;
            Pressure = (double) data.Pressure;

        }
    }

    public class HomeHumidityPressureDataModel
    {
        public List<List<HomeHumidityPressureDataPointModel>> Data { get; set; }
        public double MaxPressureY { get; set; }
        public double MinPressureY { get; set; }

        public HomeHumidityPressureDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-9).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => (x.Humidity != null || x.HumidityDHT22 != null) && x.Pressure != null).OrderByDescending(x => x.Timestamp).ToList();

            foreach (var dayDatas in tenDayDatas)
            {
                List<decimal?> humiditiesTemp = new List<decimal?> { dayDatas.Humidity, dayDatas.HumidityDHT22 };
                dayDatas.Humidity = humiditiesTemp.Where(x => x.HasValue).Average();

            }

            List<List<HomeHumidityPressureDataPointModel>> dataTemp = new List<List<HomeHumidityPressureDataPointModel>>();
            List<HomeHumidityPressureDataPointModel> currentSeries = new List<HomeHumidityPressureDataPointModel>();

            for (int i = tenDayDatas.Count - 1; i > -1; i--)
            {
                var currentItem = tenDayDatas.ElementAt(i);
                if ((i + 1) < tenDayDatas.Count)
                {
                    var previousItem = tenDayDatas.ElementAt(i + 1);
                    var timeDifference = (currentItem.Timestamp - previousItem.Timestamp).TotalMinutes;
                    if (timeDifference > 30) // start a new series
                    {
                        currentSeries = currentSeries.Where((x, n) => n % 3 == 0).ToList(); // Remove every 3rd item
                        dataTemp.Add(currentSeries);
                        currentSeries = new List<HomeHumidityPressureDataPointModel>();
                        currentSeries.Add(new HomeHumidityPressureDataPointModel(currentItem));
                    }
                    else // add to current series
                    {
                        currentSeries.Add(new HomeHumidityPressureDataPointModel(currentItem));
                    }

                }
                else // add to current series
                {
                    currentSeries.Add(new HomeHumidityPressureDataPointModel(currentItem));
                }
            }

            if (currentSeries.Count > 0)
            {
                currentSeries = currentSeries.Where((x, n) => n % 3 == 0).ToList(); // Remove every 3rd item
                dataTemp.Add(currentSeries);
            }


            Data = dataTemp;

            MaxPressureY = (double)tenDayDatas.Max(x => x.Pressure);
            MinPressureY = (double) tenDayDatas.Min(x => x.Pressure);
        }
    }
}