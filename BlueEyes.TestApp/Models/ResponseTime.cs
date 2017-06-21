using System;

namespace BlueEyes.TestApp.Models
{
    class ResponseTime
    {
        public int NodeId { get; set; }
        public DateTime DateTime { get; set; }
        public double AvgResponseTime { get; set; }
    }
}