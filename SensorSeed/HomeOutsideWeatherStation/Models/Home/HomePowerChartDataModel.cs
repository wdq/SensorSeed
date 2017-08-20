using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomePowerChartDataPointModel
    {
        public string Timestamp { get; set; }
        public double Battery { get; set; }
        public double Solar { get; set; }

        public HomePowerChartDataPointModel(HomeOutsideWeatherStationData data)
        {
            Timestamp = data.Timestamp.ToString();
            Battery = (double)data.Battery;
            Solar = (double)data.Solar; ;

        }
    }

    public class HomePowerChartDataModel
    {
        public List<List<HomePowerChartDataPointModel>> Data { get; set; }
        public double MaxBatteryY { get; set; }
        public double MinBatteryY { get; set; }
        public double MaxSolarY { get; set; }
        public double MinSolarY { get; set; }

        public HomePowerChartDataModel(DateTime endDate)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = endDate.Date.AddDays(-9).ToUniversalTime().AddHours(-18);
            DateTime endOfToday = endDate.Date.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Battery != null && x.Solar != null).OrderByDescending(x => x.Timestamp).ToList();

            List<List<HomePowerChartDataPointModel>> dataTemp = new List<List<HomePowerChartDataPointModel>>();
            List<HomePowerChartDataPointModel> currentSeries = new List<HomePowerChartDataPointModel>();

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
                        currentSeries = new List<HomePowerChartDataPointModel>();
                        currentSeries.Add(new HomePowerChartDataPointModel(currentItem));
                    }
                    else // add to current series
                    {
                        currentSeries.Add(new HomePowerChartDataPointModel(currentItem));
                    }

                }
                else // add to current series
                {
                    currentSeries.Add(new HomePowerChartDataPointModel(currentItem));
                }
            }

            if (currentSeries.Count > 0)
            {
                currentSeries = currentSeries.Where((x, n) => n % 3 == 0).ToList(); // Remove every 3rd item
                dataTemp.Add(currentSeries);
            }

            Data = dataTemp;
            MaxBatteryY = (double) tenDayDatas.Max(x => x.Battery);
            MinBatteryY = (double) tenDayDatas.Min(x => x.Battery);
            MaxSolarY = (double) tenDayDatas.Max(x => x.Solar);
            MinSolarY = (double) tenDayDatas.Min(x => x.Solar);
        }
    }
}