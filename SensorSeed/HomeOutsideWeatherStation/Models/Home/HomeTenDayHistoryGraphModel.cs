using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Models.Shared;

namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeTenDayHistoryGraphModel
    {
        public List<HomeTenDayHistoryGraphDayModel> DayInformation { get; set; }
        public List<TimeGraphModel> TemperatureGraph { get; set; }
        public List<TimeGraphModel> HumidityGraph { get; set; }
        public List<TimeGraphModel> PressureGraph { get; set; }
        public List<TimeGraphModel> RainfallGraph { get; set; }
        public List<WindGraphModel> WindSpeedGraph { get; set; }

        public HomeTenDayHistoryGraphModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-10).ToUniversalTime();
            DateTime endOfToday = DateTime.Today.AddHours(24).ToUniversalTime();
            List<HomeOutsideWeatherStationData> tenDayDatas = database.HomeOutsideWeatherStationDatas.Where(x => x.Timestamp > startOfTenDaysAgo && x.Timestamp < endOfToday).OrderByDescending(x => x.Timestamp).ToList();

            List<HomeTenDayHistoryGraphDayModel> DayInformationTemp = new List<HomeTenDayHistoryGraphDayModel>();
            DateTime endOfTodayTemp = DateTime.Today.AddHours(24).ToUniversalTime();
            for (int i = 0; i < 10; i++)
            {
                List<HomeOutsideWeatherStationData> dayDatas = tenDayDatas.Where(x => x.Timestamp > endOfTodayTemp.AddDays(-1) && x.Timestamp < endOfTodayTemp).OrderByDescending(x => x.Timestamp).ToList();
                DayInformationTemp.Add(new HomeTenDayHistoryGraphDayModel(dayDatas));
                endOfTodayTemp = endOfTodayTemp.AddDays(-1);
            }

            DayInformation = DayInformationTemp;
            //TemperatureGraph = tenDayDatas.Where(x => x.Temperature != null).Select(x => new TimeGraphModel(x.Timestamp.ToLocalTime(), (double)x.Temperature)).ToList();
            //HumidityGraph = tenDayDatas.Where(x => x.Humidity != null).Select(x => new TimeGraphModel(x.Timestamp.ToLocalTime(), (double)x.Humidity)).ToList();
            //PressureGraph = tenDayDatas.Where(x => x.Pressure != null).Select(x => new TimeGraphModel(x.Timestamp.ToLocalTime(), (double)x.Pressure)).ToList();
            //RainfallGraph = tenDayDatas.Where(x => x.Rain != null).Select(x => new TimeGraphModel(x.Timestamp.ToLocalTime(), (double)x.Rain)).ToList();
            //WindSpeedGraph = tenDayDatas.Where(x => x.WindSpeed != null && x.WindDirection != null).Select(x => new WindGraphModel(x.Timestamp.ToLocalTime(), (double)x.WindSpeed, (double)x.WindDirection)).ToList();

        }
    }
}