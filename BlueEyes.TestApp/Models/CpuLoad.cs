using System;

namespace BlueEyes.TestApp.Models
{
    class CpuLoad
    {
        public int NodeId { get; set; }
        public DateTime DateTime { get; set; }
        public double AvgLoad { get; set; }
        public double AvgMemoryUsed { get; set; }
        public double AvgPercentMemoryUsed { get; set; }
    }
}