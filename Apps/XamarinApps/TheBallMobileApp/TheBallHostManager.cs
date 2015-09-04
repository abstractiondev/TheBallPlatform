using System;
using System.Linq;
using TheBall.Support.DeviceClient;

namespace TheBallMobileApp
{
    internal class TheBallHostManager
    {
        public class HostConnectionData
        {
            public string hostName { get; set; }
            public string accountEmail { get; set; }
            public string groupID { get; set; }

            internal bool isFullAccountSync
            {
                get { return !String.IsNullOrEmpty(accountEmail); }
            }
        }

        public static WebUIOperation RegisterConnectionOperation = (url, data) =>
        {
            if (!url.EndsWith("RegisterConnection"))
                return null;
            try
            {
                var reader = new JsonFx.Json.JsonReader();
                var obj = reader.Read<HostConnectionData>(data);
                string hostName = obj.hostName;
                string groupID = obj.groupID;
                string emailAddress = obj.accountEmail;
                if (String.IsNullOrWhiteSpace(hostName) || (String.IsNullOrWhiteSpace(groupID) && String.IsNullOrWhiteSpace(emailAddress)))
                    throw new Exception("HostName or GroupID missing");
                string connectionName = String.Format("{0}_{1}_{2}", hostName, emailAddress, groupID);
                ClientExecute.ExecuteWithSettings(userSettings =>
                {
                    var existingConnection = userSettings.Connections.Any(conn => conn.Name == connectionName);
                    if (!existingConnection)
                    {
                        string connectionTargetToken = emailAddress ?? groupID;
                        ClientExecute.CreateConnection(hostName, groupID, connectionName);
                    }
                }, TBJS2OP.ReportException);
                TBJS2OP.ReportSuccess("Connection registered succesfully");
            }
            catch (Exception exception)
            {
                TBJS2OP.ReportException(exception);
            }
            return TBJS2OP.TRUE_RESULT;
        };

    }
}