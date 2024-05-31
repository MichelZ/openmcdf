using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenMcdf;
using System.IO;

namespace OpenMcdf.PerfTest
{
    class Program
    {
        static int MAX_STORAGE_COUNT = 200;
        static int MAX_STREAM_COUNT = 50;
        static String fileName = "PerfLoad.cfs";

        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();
            File.Delete(fileName);
            if (!File.Exists(fileName))
            {
                //CreateFile(true);
            }

            Console.WriteLine($"Create with validation took {stopWatch.Elapsed}");
            stopWatch.Restart();
            File.Delete(fileName);
            if (!File.Exists(fileName))
            {
               CreateFile(false);
            }

            Console.WriteLine($"Create without validation took {stopWatch.Elapsed}");

            CompoundFile cf = new CompoundFile(fileName);
            stopWatch.Restart();
            CFStream s = cf.RootStorage.GetStream("Test1");
            Console.WriteLine($"Read took {stopWatch.Elapsed}");
            Console.Read();
        }

        private static void CreateFile(bool useValidation)
        {
            var configuration = CFSConfiguration.Default;
            if (!useValidation)
            {
                configuration = configuration | CFSConfiguration.NoValidationException;
            }

            CompoundFile cf = new CompoundFile(CFSVersion.Ver_3, configuration);

            for (int j = 0; j < MAX_STREAM_COUNT; j++)
            {
                cf.RootStorage.AddStream("Test" + j.ToString()).SetData(Helpers.GetBuffer(300));
            }

            for (int i = 0; i < MAX_STORAGE_COUNT; i++)
            {
                var storage = cf.RootStorage.AddStorage("Storage" + i.ToString());

                for (int j = 0; j < MAX_STREAM_COUNT; j++)
                {
                    storage.AddStream("Test" + j.ToString()).SetData(Helpers.GetBuffer(300));
                }
            }

            cf.Save(fileName);
            cf.Close();
        }
    }
}
