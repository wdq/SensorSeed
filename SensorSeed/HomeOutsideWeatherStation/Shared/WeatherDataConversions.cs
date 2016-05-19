using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeOutsideWeatherStation.Shared
{
    public static class WeatherDataConversions
    {
        public static double WindChill(double temperature, double windSpeed)
        {
            double windChill = 0;

            // North American and United Kingdom wind chill index: https://en.wikipedia.org/wiki/Wind_chill
            // T_wc = 13.12 + 0.621*T_a - 11.37*V^0.16 + 0.3965*T_a*V^0.16
            // T_a is air temperature in C
            // V is the wind speed in km/h

            windChill = 13.12 + (0.621 * temperature) - (11.37 * Math.Pow(windSpeed, 0.16)) + (0.3965 * temperature * Math.Pow(windSpeed, 0.16));

            return windChill;
        }

        public static double HeatIndex(double temperature, double humidity)
        {
            // Heat index: https://en.wikipedia.org/wiki/Heat_index
            // Doesn't seem to be very precise of a formula, looks like some sort of an infinite series
            // Code taken from here: https://gist.github.com/Injac/c8399ca89efe8bdc213f
            // DHT11 Sensor driver for Windows 10 IoT Core - C#

            return -8.784695 +
                    1.61139411 * temperature +
                    2.338549 * humidity +
                    -0.14611605 * temperature * humidity +
                    -0.01230809 * Math.Pow(temperature, 2) +
                    -0.01642482 * Math.Pow(humidity, 2) +
                    0.00221173 * Math.Pow(temperature, 2) * humidity +
                    0.00072546 * temperature * Math.Pow(humidity, 2) +
                    -0.00000358 * Math.Pow(temperature, 2) * Math.Pow(humidity, 2);

        }

        public static double DewPoint(double temperature, double humidity)
        {
            //double dewPoint = 0;

            // Dewpoint: https://en.wikipedia.org/wiki/Dew_point
            // Code taken from here: http://stackoverflow.com/a/27289801

            return (temperature - (14.55 + 0.114 * temperature) * (1 - (0.01 * humidity)) - Math.Pow(((2.5 + 0.007 * temperature) * (1 - (0.01 * humidity))), 3) - (15.9 + 0.117 * temperature) * Math.Pow((1 - (0.01 * humidity)), 14));
        }
    }
}