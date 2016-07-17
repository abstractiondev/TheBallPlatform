using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlatformCoreTests
{
    public static class TestSupport
    {
        public static string CurrPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string TestDataPath => Path.Combine(CurrPath, "TestData");

        public static string TheBallPath => Path.Combine(TestDataPath, "TheBall");

        public static string OIPPath => Path.Combine(TestDataPath, "OIP");

        public static string OnlineTrainingPath => Path.Combine(TestDataPath, "OnlineTraining");


        public static string GetTheBallFileFullPath(string theBallFileRelativePath)
        {
            return Path.Combine(TheBallPath, theBallFileRelativePath);
        }
    }
}
