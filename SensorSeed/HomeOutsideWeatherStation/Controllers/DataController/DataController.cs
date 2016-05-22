using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeOutsideWeatherStation.Controllers.DataController
{
    public class DataController : Controller
    {
        [HttpPost]
        public ActionResult AddData(string Temperature, string Humidity, string Pressure, string Altitude, string Wind, string Gust, string Rain, string Battery, string Solar, string Direction, string Temperature180)
        {
            SensorSeedDataContext database = new SensorSeedDataContext();

            HomeOutsideWeatherStationData data = new HomeOutsideWeatherStationData();

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

            // Pressure
            try
            {
                decimal PressureDecimal = 0;
                PressureDecimal = Convert.ToDecimal(Pressure);
                data.Pressure = PressureDecimal;
            }
            catch (Exception exception) { }

            // Altitude
            try
            {
                decimal AltitudeDecimal = 0;
                AltitudeDecimal = Convert.ToDecimal(Altitude);
                data.Altitude = AltitudeDecimal;
            }
            catch (Exception exception) { }


            // WindSpeed
            try
            {
                decimal WindSpeedDecimal = 0;
                WindSpeedDecimal = Convert.ToDecimal(Wind);
                data.WindSpeed = WindSpeedDecimal;
            }
            catch (Exception exception) { }

            // GustSpeed
            try
            {
                decimal GustSpeedDecimal = 0;
                GustSpeedDecimal = Convert.ToDecimal(Gust);
                data.GustSpeed = GustSpeedDecimal;
            }
            catch (Exception exception) { }

            // Rain
            try
            {
                decimal RainDecimal = 0;
                RainDecimal = Convert.ToDecimal(Rain);
                data.Rain = RainDecimal;
            }
            catch (Exception exception) { }

            // Battery
            try
            {
                decimal BatteryDecimal = 0;
                BatteryDecimal = Convert.ToDecimal(Battery);
                BatteryDecimal = BatteryDecimal / (decimal)1024; // Max ADC value
                BatteryDecimal = BatteryDecimal * (decimal)3.3; // High voltage
                BatteryDecimal = BatteryDecimal * (decimal)2; // Voltage divider factor
                data.Battery = BatteryDecimal;
            }
            catch (Exception exception) { }

            // Solar
            try
            {
                decimal SolarDecimal = 0;
                SolarDecimal = Convert.ToDecimal(Solar);
                SolarDecimal = SolarDecimal / (decimal)1024; // Max ADC value
                SolarDecimal = SolarDecimal * (decimal)3.3; // High voltage
                SolarDecimal = SolarDecimal * (decimal)3; // Voltage divider factor
                data.Solar = SolarDecimal;
            }
            catch (Exception exception) { }

            // Wind Direction
            try
            {
                decimal DirectionDegreesDecimal = 0;
                decimal DirectionDecimal = 0;
                DirectionDecimal = Convert.ToDecimal(Direction);
                decimal r1 = 10000;
                decimal r2 = 0; // Unknown
                decimal vin = (decimal)3.3;
                decimal vout = ((DirectionDecimal / (decimal)1024) * (decimal)3.3);
                r2 = r1 * (1 / ((vin / vout) - 1));
                if (((decimal)300 <= r2) && (r2 < (decimal)790))
                {
                    DirectionDegreesDecimal = (decimal)112.5;
                }
                else if (((decimal)790 <= r2) && (r2 < (decimal)946))
                {
                    DirectionDegreesDecimal = (decimal)67.5;
                }
                else if (((decimal)946 <= r2) && (r2 < (decimal)1205))
                {
                    DirectionDegreesDecimal = (decimal)90;
                }
                else if (((decimal)1205 <= r2) && (r2 < (decimal)1805))
                {
                    DirectionDegreesDecimal = (decimal)157.5;
                }
                else if (((decimal)1805 <= r2) && (r2 < (decimal)2670))
                {
                    DirectionDegreesDecimal = (decimal)135;
                }
                else if (((decimal)2670 <= r2) && (r2 < (decimal)3520))
                {
                    DirectionDegreesDecimal = (decimal)202.5;
                }
                else if (((decimal)3520 <= r2) && (r2 < (decimal)5235))
                {
                    DirectionDegreesDecimal = (decimal)180;
                }
                else if (((decimal)5235 <= r2) && (r2 < (decimal)7385))
                {
                    DirectionDegreesDecimal = (decimal)22.5;
                }
                else if (((decimal)7385 <= r2) && (r2 < (decimal)11160))
                {
                    DirectionDegreesDecimal = (decimal)45;
                }
                else if (((decimal)11160 <= r2) && (r2 < (decimal)15060))
                {
                    DirectionDegreesDecimal = (decimal)247.5;
                }
                else if (((decimal)15060 <= r2) && (r2 < (decimal)18940))
                {
                    DirectionDegreesDecimal = (decimal)225;
                }
                else if (((decimal)18940 <= r2) && (r2 < (decimal)27440))
                {
                    DirectionDegreesDecimal = (decimal)337.5;
                }
                else if (((decimal)27440 <= r2) && (r2 < (decimal)37560))
                {
                    DirectionDegreesDecimal = (decimal)0;
                }
                else if (((decimal)37560 <= r2) && (r2 < (decimal)53510))
                {
                    DirectionDegreesDecimal = (decimal)292.5;
                }
                else if (((decimal)53510 <= r2) && (r2 < (decimal)92450))
                {
                    DirectionDegreesDecimal = (decimal)315;
                }
                else if (((decimal)92450 <= r2) && (r2 < (decimal)160000))
                {
                    DirectionDegreesDecimal = (decimal)270;
                }

                data.WindDirection = DirectionDegreesDecimal;
            }
            catch (Exception exception)
            {
                int a = 1;
            }

            // Temperature180
            try
            {
                decimal TemperatureDecimal = 0;
                TemperatureDecimal = Convert.ToDecimal(Temperature180);
                data.Temperature180 = TemperatureDecimal;
            }
            catch (Exception exception) { }



            database.HomeOutsideWeatherStationDatas.InsertOnSubmit(data);
            database.SubmitChanges();

            JsonResult jsonResult = new JsonResult();
            jsonResult.Data = "ok";

            return jsonResult;
        }
    }
}