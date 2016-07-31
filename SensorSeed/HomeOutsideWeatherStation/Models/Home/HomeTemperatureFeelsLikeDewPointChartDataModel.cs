using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeTemperatureFeelsLikeDewPointChartDataPointModel
    {
        public string Timestamp { get; set; }
        public double Temperature { get; set; }
        public double TemperatureFeelsLike { get; set; }
        public double DewPoint { get; set; }

        public HomeTemperatureFeelsLikeDewPointChartDataPointModel(HomeOutsideWeatherStationData data)
        {
            Timestamp = data.Timestamp.ToString();
            Temperature = (double)data.Temperature;
            TemperatureFeelsLike = WeatherDataConversions.WindChill(Temperature, (double)data.WindSpeed);
            DewPoint = WeatherDataConversions.DewPoint(Temperature, (double)data.Humidity);

        }
    }

    public class HomeTemperatureFeelsLikeDewPointChartDataModel
    {
        public List<List<HomeTemperatureFeelsLikeDewPointChartDataPointModel>> Data { get; set; }
        public double MaxY { get; set; }
        public double MinY { get; set; }

        public HomeTemperatureFeelsLikeDewPointChartDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-9).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).Where(x => (x.Temperature != null || x.Temperature180 != null || x.TemperatureDHT22 != null) && (x.Humidity != null || x.HumidityDHT22 != null) && x.WindSpeed != null).OrderByDescending(x => x.Timestamp).ToList();
            foreach (var dayDatas in tenDayDatas)
            {
                List<decimal?> temperaturesTemp = new List<decimal?> { dayDatas.Temperature, dayDatas.Temperature180, dayDatas.TemperatureDHT22 };
                List<decimal?> humiditiesTemp = new List<decimal?> { dayDatas.Humidity, dayDatas.HumidityDHT22 };
                dayDatas.Temperature = temperaturesTemp.Where(x => x.HasValue).Average();
                dayDatas.Humidity = humiditiesTemp.Where(x => x.HasValue).Average();

            }

            List<List<HomeTemperatureFeelsLikeDewPointChartDataPointModel>> dataTemp = new List<List<HomeTemperatureFeelsLikeDewPointChartDataPointModel>>();
            List<HomeTemperatureFeelsLikeDewPointChartDataPointModel> currentSeries = new List<HomeTemperatureFeelsLikeDewPointChartDataPointModel>();

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
                        currentSeries = new List<HomeTemperatureFeelsLikeDewPointChartDataPointModel>();
                        currentSeries.Add(new HomeTemperatureFeelsLikeDewPointChartDataPointModel(currentItem));
                    }
                    else // add to current series
                    {
                        currentSeries.Add(new HomeTemperatureFeelsLikeDewPointChartDataPointModel(currentItem));
                    }

                }
                else // add to current series
                {
                    currentSeries.Add(new HomeTemperatureFeelsLikeDewPointChartDataPointModel(currentItem));
                }
            }

            if (currentSeries.Count > 0)
            {
                currentSeries = currentSeries.Where((x, n) => n % 3 == 0).ToList(); // Remove every 3rd item
                dataTemp.Add(currentSeries);
            }
            Data = dataTemp;

            var maxTemp = (double)tenDayDatas.Max(x => x.Temperature);
            var maxTempFeelsLike = tenDayDatas.Max(x => WeatherDataConversions.WindChill((double)x.Temperature, (double)x.WindSpeed));
            var maxDewPoint = tenDayDatas.Max(x => WeatherDataConversions.DewPoint((double)x.Temperature, (double)x.Humidity));
            var minTemp = (double)tenDayDatas.Min(x => x.Temperature);
            var minTempFeelsLike = tenDayDatas.Min(x => WeatherDataConversions.WindChill((double)x.Temperature, (double)x.WindSpeed));
            var minDewPoint = tenDayDatas.Min(x => WeatherDataConversions.DewPoint((double)x.Temperature, (double)x.Humidity));

            MaxY = new List<double> {maxTemp, maxTempFeelsLike, maxDewPoint}.Max();
            MinY = new List<double> { minTemp, minTempFeelsLike, minDewPoint }.Min();
        }
    }
}