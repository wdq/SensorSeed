using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeTemperatureFeelsLikeDewPointChartDataPointModel
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double TemperatureFeelsLike { get; set; }
        public double DewPoint { get; set; }

        public HomeTemperatureFeelsLikeDewPointChartDataPointModel(HomeOutsideWeatherStationData data)
        {
            Timestamp = data.Timestamp;
            Temperature = (double)data.Temperature;
            TemperatureFeelsLike = WeatherDataConversions.WindChill((double)data.Temperature, (double)data.WindSpeed);
            DewPoint = WeatherDataConversions.DewPoint((double)data.Temperature, (double)data.Humidity);

        }
    }

    public class HomeTemperatureFeelsLikeDewPointChartDataModel
    {
        public List<HomeTemperatureFeelsLikeDewPointChartDataPointModel> Data { get; set; }

        public HomeTemperatureFeelsLikeDewPointChartDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-10).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Temperature != null && x.Humidity != null && x.WindSpeed != null).OrderByDescending(x => x.Timestamp).ToList();


            List<HomeTemperatureFeelsLikeDewPointChartDataPointModel> dataTemp = new List<HomeTemperatureFeelsLikeDewPointChartDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                dataTemp.Add(new HomeTemperatureFeelsLikeDewPointChartDataPointModel(data));
            }

            Data = dataTemp;
        }
    }
}