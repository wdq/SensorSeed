using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;
using Innovative.SolarCalculator;



namespace HomeOutsideWeatherStation.Models.Home
{
    public class HomeTenDaySunriseSunsetDataPointModel
    {
        public string Date { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        public HomeTenDaySunriseSunsetDataPointModel(DateTime date)
        {
            Date = date.Date.ToString();

            TimeZoneInfo cst = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            SolarTimes solarTimes = new SolarTimes(date, 40.815999, -96.611661);
            DateTime sunrise = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunrise.ToUniversalTime(), cst).AddHours(-1); // todo: pretty sure the subtracting an hour is required for the daylight savings stuff
            DateTime sunset = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunset.ToUniversalTime(), cst).AddHours(-1);  // todo: I'll need to figure that out and get this to work year round

            Sunrise = sunrise.ToString();
            Sunset = sunset.ToString();

        }
    }

    public class HomeTenDaySunriseSunsetDataModel
    {
        public List<HomeTenDaySunriseSunsetDataPointModel> Data { get; set; }

        public HomeTenDaySunriseSunsetDataModel()
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            List<HomeTenDaySunriseSunsetDataPointModel> dataTemp = new List<HomeTenDaySunriseSunsetDataPointModel>();

            DateTime startOfTenDaysAgo = DateTime.Today.AddDays(-9);
            DateTime endOfToday = DateTime.Today.AddHours(24);

            for (int i = 0; i < 11; i++)
            {
                dataTemp.Add(new HomeTenDaySunriseSunsetDataPointModel(startOfTenDaysAgo));
                startOfTenDaysAgo = startOfTenDaysAgo.AddDays(1);
            }

            Data = dataTemp;
        }
    }
}