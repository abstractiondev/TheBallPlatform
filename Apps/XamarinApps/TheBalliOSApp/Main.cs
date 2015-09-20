using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Foundation;
using TheBall.Support.DeviceClient;
using UIKit;

namespace TheBalliOSApp
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            int i = 0;
            /*
            ClientExecute.ExecuteWithSettings(settings =>
            {
                var testConn = settings.Connections.FirstOrDefault(conn => conn.HostName == "test.theball.me");
                if (testConn == null)
                {
                    ClientExecute.CreateConnection("test.theball.me", "kalle.launiala@gmail.com", "testConn");
                }
            }, exception =>
            {
                Debug.WriteLine("Conn error: " + exception.ToString());
            });*/
            UIApplication.Main(args, null, "AppDelegate");

        }
    }
}