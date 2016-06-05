using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomePrecipitationChartDataPointModel
    {
        public string Timestamp { get; set; }
        public double CurrentRain { get; set; }
        public double TotalRain { get; set; }

        public HomePrecipitationChartDataPointModel(HomeOutsideWeatherStationData data, double totalRain)
        {
            Timestamp = data.Timestamp.ToString();
            CurrentRain = (double)data.Rain;
            TotalRain = totalRain;
        }
    }

    public class HomePrecipitationChartDataModel
    {
        public List<HomePrecipitationChartDataPointModel> Data { get; set; }

        public HomePrecipitationChartDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-9).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Rain != null).OrderByDescending(x => x.Timestamp).ToList();
            double totalRain = tenDayDatas.Sum(x => (double)x.Rain);

            List<HomePrecipitationChartDataPointModel> dataTemp = new List<HomePrecipitationChartDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                totalRain -= (double)data.Rain;
                dataTemp.Add(new HomePrecipitationChartDataPointModel(data, totalRain));
            }

            Data = dataTemp;
        }
    }
}