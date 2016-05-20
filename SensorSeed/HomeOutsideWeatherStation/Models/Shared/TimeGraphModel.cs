using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOutsideWeatherStation.Models.Shared
{
    public class TimeGraphModel
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }

        public TimeGraphModel(DateTime timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }
}
