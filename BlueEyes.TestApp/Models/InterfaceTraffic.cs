using System;

namespace BlueEyes.TestApp.Models
{
    class InterfaceTraffic
    {
        public int NodeId { get; set; }
        public int InterfaceId { get; set; }
        public DateTime DateTime { get; set; }
        public double InAveragebps { get; set; }
        public double OutAveragebps { get; set; }
        public double InTotalBytes { get; set; }
        public double OutTotalBytes { get; set; }
    }
}