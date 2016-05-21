using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeHumidityPressureChartDataPointModel
    {
        public DateTime Timestamp { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }

        public HomeHumidityPressureChartDataPointModel(HomeOutsideWeatherStationData data)
        {
            Timestamp = data.Timestamp;
            Humidity = (double) data.Humidity;
            Pressure = (double) data.Pressure;
        }
    }

    class HomeHumidityPressureChartDataModel
    {
        public List<HomeHumidityPressureChartDataPointModel> Data { get; set; }

        public HomeHumidityPressureChartDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-10).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => x.Humidity != null && x.Pressure != null).OrderByDescending(x => x.Timestamp).ToList();


            List<HomeHumidityPressureChartDataPointModel> dataTemp = new List<HomeHumidityPressureChartDataPointModel>();
            foreach (var data in tenDayDatas)
            {
                dataTemp.Add(new HomeHumidityPressureChartDataPointModel(data));
            }

            Data = dataTemp;
        }
    }
}
