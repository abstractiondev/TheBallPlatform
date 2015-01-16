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
        public static string CurrPath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        public static string TestDataPath
        {
            get { return Path.Combine(CurrPath, "TestData"); }
        }

        public static string TheBallPath
        {
            get { return Path.Combine(TestDataPath, "TheBall"); }
        }

        public static string GetTheBallFileFullPath(string theBallFileRelativePath)
        {
            return Path.Combine(TheBallPath, theBallFileRelativePath);
        }
    }
}
