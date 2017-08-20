using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeOutsideWeatherStation.Shared;
using Innovative.SolarCalculator;
using SunSetRiseLib;


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

            double JD = 0;
            int zone = -6;
            double latitude = 40.815987372770905;
            double longitude = -96.61160876043141;
            bool dst = false;

            JD = Util.calcJD(date.Date);
            double sunRise = Util.calcSunRiseUTC(JD, latitude, longitude);
            double sunSet = Util.calcSunSetUTC(JD, latitude, longitude);
            DateTime? sunrise = Util.getDateTime(sunRise, zone, date.Date, dst);
            DateTime? sunset = Util.getDateTime(sunSet, zone, date.Date, dst);

            Sunrise = sunrise.ToString();
            Sunset = sunset.ToString();

        }
    }

    public class HomeTenDaySunriseSunsetDataModel
    {
        public List<HomeTenDaySunriseSunsetDataPointModel> Data { get; set; }

        public HomeTenDaySunriseSunsetDataModel(DateTime endDate)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            List<HomeTenDaySunriseSunsetDataPointModel> dataTemp = new List<HomeTenDaySunriseSunsetDataPointModel>();

            DateTime startOfTenDaysAgo = endDate.Date.AddDays(-9).AddHours(-18);
            DateTime endOfToday = endDate.Date.AddHours(24);

            for (int i = 0; i < 11; i++)
            {
                dataTemp.Add(new HomeTenDaySunriseSunsetDataPointModel(startOfTenDaysAgo));
                startOfTenDaysAgo = startOfTenDaysAgo.AddDays(1);
            }

            Data = dataTemp;
        }
    }
}