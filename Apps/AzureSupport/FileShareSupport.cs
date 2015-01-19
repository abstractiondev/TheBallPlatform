﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.AutoScaling.Model;
using TheBall;

namespace AzureSupport
{
    public static class FileShareSupport
    {
        //private const string CoreSharedFolderName = "tbcore";
        public static void MountCoreShare()
        {
            string shareAndUserName = InstanceConfiguration.CoreFileShareAccountName;
            string shareKeyName = InstanceConfiguration.CoreFileShareAccountKey;
            string sharePathWithFolder = InstanceConfiguration.CoreShareWithFolderName;
            if (String.IsNullOrEmpty(shareAndUserName) || String.IsNullOrEmpty(shareKeyName))
                throw new InvalidDataException("Missing required configuration data");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            //startInfo.Arguments = String.Format("/C net use X: \\\\{0}.file.core.windows.net\\tbcore /u:{0} {1}",
            startInfo.Arguments = String.Format("/C net use {0} /u:{1} {2}",
                sharePathWithFolder,
                shareAndUserName, shareKeyName);
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
