using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOutsideWeatherStation.Models.Shared
{
    public class WindGraphModel
    {
        public DateTime Timestamp { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
    }
}
