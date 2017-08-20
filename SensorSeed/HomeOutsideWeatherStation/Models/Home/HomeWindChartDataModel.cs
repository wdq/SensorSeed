using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeWindChartDataPointModel
    {
        public string Timestamp { get; set; }
        public double WindSpeed { get; set; }
        public double GustSpeed { get; set; }
        public double WindDirection { get; set; }

        public HomeWindChartDataPointModel(HomeOutsideWeatherStationData data)
        {
            Timestamp = data.Timestamp.ToString();
            WindSpeed = (double)data.WindSpeed;
            GustSpeed = (double)data.GustSpeed;
            WindDirection = (double) data.WindDirection;
        }
    }

    public class HomeWindChartDataModel
    {
        public List<HomeWindChartDataPointModel> Data { get; set; }

        public HomeWindChartDataModel(DateTime endDate)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = endDate.Date.AddDays(-9).ToUniversalTime().AddHours(-18);
            DateTime endOfToday = endDate.Date.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.WindSpeed != null && x.GustSpeed != null && x.WindDirection != null).OrderByDescending(x => x.Timestamp).ToList();
            tenDayDatas = tenDayDatas.Where((x, n) => n % 3 == 0).ToList(); // Remove every 3rd item

            List<HomeWindChartDataPointModel> dataTemp = new List<HomeWindChartDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                dataTemp.Add(new HomeWindChartDataPointModel(data));
            }

            Data = dataTemp;
        }
    }
}