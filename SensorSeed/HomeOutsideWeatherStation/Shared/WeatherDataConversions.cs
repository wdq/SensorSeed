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
            double heatIndex = 0;
            double temperatureF = 0;

            // Heat index: https://en.wikipedia.org/wiki/Heat_index
            // Doesn't seem to be very precise of a formula, looks like some sort of an infinite series
            // HI = c_1 + c_2*T + c_3*R + c_4*T*R + c_5*T^2 + c_6*R^2 + c_7*T^2*R + c_8*T*R^2 + c_9*T^2*R^2
            // T = temperature in F, R = humidity

            double c_1 = -42.379;
            double c_2 = 2.04901523;
            double c_3 = 10.14333127;
            double c_4 = -0.22475541;
            double c_5 = -6.83783 * Math.Pow(10, -3);
            double c_6 = -5.481717 * Math.Pow(10, -2);
            double c_7 = 1.22874 * Math.Pow(10, -3);
            double c_8 = 8.5282 * Math.Pow(10, -4);
            double c_9 = -1.99 * Math.Pow(10, -6);

            heatIndex = c_1 + (c_2 * temperatureF) + (c_3 * humidity) + (c_4 * temperatureF * humidity) + (c_5 * Math.Pow(temperatureF, 2)) + (c_6 * Math.Pow(humidity, 2)) + (c_7 * Math.Pow(temperatureF, 2) * humidity) + (c_8 * temperatureF * Math.Pow(humidity, 2)) + (c_9 * Math.Pow(temperatureF, 2) * Math.Pow(humidity, 2));

            heatIndex = (heatIndex * (9 / 5)) + 32; // Fahrenheit to Celsius

            return heatIndex;
        }

        public static double DewPoint(double temperature, double humidity)
        {
            double dewPoint = 0;

            // Dewpoint: https://en.wikipedia.org/wiki/Dew_point
            // todo: formula

            return dewPoint;
        }
    }
}