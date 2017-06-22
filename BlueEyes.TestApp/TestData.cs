using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlueEyes.TestApp.Models;
using CsvHelper;

namespace BlueEyes.TestApp
{
    class TestData
    {
        public IList<ResponseTime> ResponseTimeDetail { get; private set; }
        public IList<ResponseTime> ResponseTimeHourly { get; private set; }
        public IList<InterfaceTraffic> LargeInterfaceTrafficDetail { get; private set; }
        public IList<InterfaceTraffic> LargeInterfaceTrafficHourly { get; private set; }
        public IList<InterfaceTraffic> SmallInterfaceTrafficDetail { get; private set; }
        public IList<InterfaceTraffic> SmallInterfaceTrafficHourly { get; private set; }
        public IList<CpuLoad> CpuLoadDetail { get; private set; }
        public IList<CpuLoad> CpuLoadHourly { get; private set; }

        private static readonly Lazy<TestData> _instance = new Lazy<TestData>(() => new TestData());
        public static TestData Instance => _instance.Value;
        
        private TestData()
        {
            LoadAllTestData();
        }

        private void LoadAllTestData()
        {
            var dataDir = @".\Data\";
            ResponseTimeDetail = LoadTestData<ResponseTime>(Path.Combine(dataDir, "ResponseTime_Detail.csv"));
            ResponseTimeHourly = LoadTestData<ResponseTime>(Path.Combine(dataDir, "ResponseTime_Hourly.csv"));
            LargeInterfaceTrafficDetail = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "LargeInterfaceTraffic_Detail.csv"));
            LargeInterfaceTrafficHourly = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "LargeInterfaceTraffic_Hourly.csv"));
            SmallInterfaceTrafficDetail = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "SmallInterfaceTraffic_Detail.csv"));
            SmallInterfaceTrafficHourly = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "SmallInterfaceTraffic_Hourly.csv"));
            CpuLoadDetail = LoadTestData<CpuLoad>(Path.Combine(dataDir, "CPULoad_Detail.csv"));
            CpuLoadHourly = LoadTestData<CpuLoad>(Path.Combine(dataDir, "CPULoad_Hourly.csv"));
        }



        private IList<T> LoadTestData<T>(string file)
        {
            using (var textReader = File.OpenText(file))
            {
                var csv = new CsvReader(textReader);

                var records = csv.GetRecords<T>();
                return records.ToList();
            }
        }

    }
}
