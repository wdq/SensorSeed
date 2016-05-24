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
        public List<HomeHumidityPressureDataPointModel> Data { get; set; }

        public HomeHumidityPressureDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-9).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Humidity != null && x.Pressure != null).OrderByDescending(x => x.Timestamp).ToList();


            List<HomeHumidityPressureDataPointModel> dataTemp = new List<HomeHumidityPressureDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                dataTemp.Add(new HomeHumidityPressureDataPointModel(data));
            }

            Data = dataTemp;
        }
    }
}