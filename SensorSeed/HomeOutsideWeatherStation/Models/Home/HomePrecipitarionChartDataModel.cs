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
        public decimal CurrentRain { get; set; }
        public decimal TotalRain { get; set; }

        public HomePrecipitationChartDataPointModel(HomeOutsideWeatherStationData data, decimal totalRain)
        {
            Timestamp = data.Timestamp.ToString();
            CurrentRain = (decimal)data.Rain;
            TotalRain = totalRain;
        }
    }

    public class HomePrecipitationChartDataModel
    {
        public List<HomePrecipitationChartDataPointModel> Data { get; set; }

        public HomePrecipitationChartDataModel(DateTime endDate)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = endDate.Date.AddDays(-9).ToUniversalTime().AddHours(-18);
            DateTime endOfToday = endDate.Date.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Rain != null).OrderByDescending(x => x.Timestamp).ToList();
            decimal totalRain = tenDayDatas.Sum(x => (decimal)x.Rain);
            tenDayDatas = tenDayDatas.Where(x => x.Rain > 0).ToList();

            List<HomePrecipitationChartDataPointModel> dataTemp = new List<HomePrecipitationChartDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                totalRain -= (decimal)data.Rain;
                dataTemp.Add(new HomePrecipitationChartDataPointModel(data, totalRain));
            }

            Data = dataTemp;
        }
    }
}