using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using Nito.AsyncEx;

namespace TheBallDevWorker
{

    class Program
    {
        private static int ExitCode = 0;

        const string ComponentName = "TheBallDevWorker";

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }



        static int Main(string[] args)
        {
            try
            {
                //Debugger.Launch();
                bool isTestMode = false;
                string dedicatedToOwner = null;
                string applicationConfigFullPath = null;
                string updateAccessInfoFile = null;
                bool autoUpdate = false;
                string clientHandle = null;
                var optionSet = new OptionSet()
                {
                };
                var options = optionSet.Parse(args);
                bool hasExtraOptions = options.Count > 0;
                AsyncContext.Run(() => MainAsync());
            }
            catch (Exception exception)
            {
                var errorFile = Path.Combine(AssemblyDirectory, "ConsoleErrorLog.txt");
                File.WriteAllText(errorFile, exception.ToString());
                Console.WriteLine("Top Exception Handler: ");
                Console.WriteLine(exception.ToString());
                return -2;
            }
            return ExitCode;
        }

        static async Task MainAsync()
        {
            
        }

    }
}
