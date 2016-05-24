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
        public List<HomePowerChartDataPointModel> Data { get; set; }

        public HomePowerChartDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-9).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Battery != null && x.Solar != null).OrderByDescending(x => x.Timestamp).ToList();


            List<HomePowerChartDataPointModel> dataTemp = new List<HomePowerChartDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                dataTemp.Add(new HomePowerChartDataPointModel(data));
            }

            Data = dataTemp;
        }
    }
}