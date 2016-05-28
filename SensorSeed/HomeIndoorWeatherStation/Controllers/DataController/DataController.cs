using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeIndoorWeatherStation.Controllers.DataController
{
    public class DataController : Controller
    {
        [HttpPost]
        public ActionResult AddData(string StationIP, string Temperature, string Humidity)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            string[] WeatherStationBasementRoomAddress = { "10.0.10.4", "/dht2-temperature", "/dht2-humidity", "basement" };
            string[] WeatherStationLivingRoomAddress = { "10.0.10.5", "/dht2-temperature", "/dht2-humidity", "living room" };
            string[] WeatherStationServerRoomAddress = { "10.0.10.6", "/dht-temperature", "/dht-humidity", "server room" };
            string[] WeatherStationWilliamsRoomAddress = { "10.0.10.7", "/dht-temperature", "/dht-humidity", "William's room" };


            if (StationIP == WeatherStationBasementRoomAddress[0])
            {
                HomeBasementRoomWeatherStationData data = new HomeBasementRoomWeatherStationData();
                data.Id = Guid.NewGuid();
                data.Timestamp = DateTime.UtcNow;
                // Temperature
                try
                {
                    decimal TemperatureDecimal = 0;
                    TemperatureDecimal = Convert.ToDecimal(Temperature);
                    data.Temperature = TemperatureDecimal;
                }
                catch (Exception exception) { }

                // Humidity
                try
                {
                    decimal HumidityDecimal = 0;
                    HumidityDecimal = Convert.ToDecimal(Humidity);
                    data.Humidity = HumidityDecimal;
                }
                catch (Exception exception) { }

                database.HomeBasementRoomWeatherStationDatas.InsertOnSubmit(data);
                database.SubmitChanges();
            }
            else if (StationIP == WeatherStationLivingRoomAddress[0])
            {
                HomeLivingRoomWeatherStationData data = new HomeLivingRoomWeatherStationData();
                data.Id = Guid.NewGuid();
                data.Timestamp = DateTime.UtcNow;
                // Temperature
                try
                {
                    decimal TemperatureDecimal = 0;
                    TemperatureDecimal = Convert.ToDecimal(Temperature);
                    data.Temperature = TemperatureDecimal;
                }
                catch (Exception exception) { }

                // Humidity
                try
                {
                    decimal HumidityDecimal = 0;
                    HumidityDecimal = Convert.ToDecimal(Humidity);
                    data.Humidity = HumidityDecimal;
                }
                catch (Exception exception) { }

                database.HomeLivingRoomWeatherStationDatas.InsertOnSubmit(data);
                database.SubmitChanges();
            }
            else if (StationIP == WeatherStationServerRoomAddress[0])
            {
                HomeServerRoomWeatherStationData data = new HomeServerRoomWeatherStationData();
                data.Id = Guid.NewGuid();
                data.Timestamp = DateTime.UtcNow;
                // Temperature
                try
                {
                    decimal TemperatureDecimal = 0;
                    TemperatureDecimal = Convert.ToDecimal(Temperature);
                    data.Temperature = TemperatureDecimal;
                }
                catch (Exception exception) { }

                // Humidity
                try
                {
                    decimal HumidityDecimal = 0;
                    HumidityDecimal = Convert.ToDecimal(Humidity);
                    data.Humidity = HumidityDecimal;
                }
                catch (Exception exception) { }

                database.HomeServerRoomWeatherStationDatas.InsertOnSubmit(data);
                database.SubmitChanges();
            }
            else if (StationIP == WeatherStationWilliamsRoomAddress[0])
            {
                HomeWilliamRoomWeatherStationData data = new HomeWilliamRoomWeatherStationData();
                data.Id = Guid.NewGuid();
                data.Timestamp = DateTime.UtcNow;
                // Temperature
                try
                {
                    decimal TemperatureDecimal = 0;
                    TemperatureDecimal = Convert.ToDecimal(Temperature);
                    data.Temperature = TemperatureDecimal;
                }
                catch (Exception exception) { }

                // Humidity
                try
                {
                    decimal HumidityDecimal = 0;
                    HumidityDecimal = Convert.ToDecimal(Humidity);
                    data.Humidity = HumidityDecimal;
                }
                catch (Exception exception) { }

                database.HomeWilliamRoomWeatherStationDatas.InsertOnSubmit(data);
                database.SubmitChanges();
            }


            JsonResult jsonResult = new JsonResult();
            jsonResult.Data = "ok";

            return jsonResult;
        }
    }
}