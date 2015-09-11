using System;
using System.IO;
using System.Linq;
using Java.Net;
using TheBall.Support.DeviceClient;
using Xamarin;

namespace TheBallMobileApp
{
    internal class TheBallHostManager
    {
        public class HostConnectionData
        {
            public string host { get; set; }
            public string email { get; set; }
            public string groupID { get; set; }

            internal bool isFullAccountSync
            {
                get { return !String.IsNullOrEmpty(email); }
            }
        }

        public static Tuple<string, string, Stream> CustomDataRetriever(string datakey)
        {
            switch (datakey)
            {
                case "ConnectionHosts.json":
                {
                    byte[] data;
                    using (var stream = new MemoryStream())
                    {
                        JSONSupport.SerializeToJSONStream(new
                        {
                            email = "test.email@from.json",
                            hosts = new[]
                            {
                                new {displayName = "home.theball.me", value = "home.theball.me"},
                                new {displayName = "test.theball.me", value = "test.theball.me"},
                                new {displayName = "members.onlinetaekwondo.net", value = "members.onlinetaekwondo.net"},
                                new {displayName = "beta.diosphere.org", value = "beta.diosphere.org"},
                            }
                        }, stream);
                        data = stream.ToArray();
                    }
                    var dataStream = new MemoryStream(data);
                    return new Tuple<string, string, Stream>("application/json", "utf-8", dataStream);
                }
                case "Connections.json":
                {
                    object dataObject = null;
                    ClientExecute.ExecuteWithSettings(settings =>
                    {
                        var connections = settings.Connections.Select(conn => new
                        {
                            id = conn.Device.EstablishedTrustID,
                            host = conn.HostName,
                            email = conn.Device.AccountEmail
                        }).ToArray();
                        dataObject = new
                        {
                            connections
                        };
                    }, ReportException);
                    byte[] data;
                    using (var stream = new MemoryStream())
                    {
                        JSONSupport.SerializeToJSONStream(dataObject, stream);
                        data = stream.ToArray();
                    }
                    var dataStream = new MemoryStream(data);
                    return new Tuple<string, string, Stream>("application/json", "utf-8", dataStream);
                }
            }
            return null;
        }

        private static void ReportException(Exception obj)
        {
            if (Insights.IsInitialized)
                Insights.Report(obj);
        }

        public static WebUIOperation DeleteConnectionOperation = (url, data) =>
        {
            if (!url.EndsWith("DeleteConnection"))
                return null;
            try
            {
                var reader = new JsonFx.Json.JsonReader();
                dynamic dynobj = reader.Read(data);
                string connectionID = (string) dynobj["connectionID"];
                ClientExecute.ExecuteWithSettings(userSettings =>
                {
                    var connToDelete =
                        userSettings.Connections.FirstOrDefault(conn => conn.Device.EstablishedTrustID == connectionID);
                    if (connToDelete != null)
                    {
                        ClientExecute.DeleteConnection(connToDelete.Name, true);
                    }
                }, TBJS2OP.ReportException);
            }
            catch (Exception exception)
            {
                TBJS2OP.ReportException(exception);
            }

            return TBJS2OP.TRUE_RESULT;
        };

        public static WebUIOperation CreateConnectionOperation = (url, data) =>
        {
            if (!url.EndsWith("CreateConnection"))
                return null;
            try
            {
                var reader = new JsonFx.Json.JsonReader();
                var obj = reader.Read<HostConnectionData>(data);
                string hostName = obj.host;
                string groupID = obj.groupID;
                string emailAddress = obj.email;
                if (String.IsNullOrWhiteSpace(hostName) || (String.IsNullOrWhiteSpace(groupID) && String.IsNullOrWhiteSpace(emailAddress)))
                    throw new Exception("HostName or GroupID missing");
                string connectionName = String.Format("{0}_{1}_{2}", hostName, emailAddress, groupID);
                ClientExecute.ExecuteWithSettings(userSettings =>
                {
                    var existingConnection = userSettings.Connections.Any(conn => conn.Name == connectionName);
                    if (!existingConnection)
                    {
                        string connectionTargetToken = emailAddress ?? groupID;
                        ClientExecute.CreateConnection(hostName, connectionTargetToken, connectionName);
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

        public static void SetDeviceClientHooks()
        {
            ClientExecute.LocalContentItemRetriever = location =>
            {
                var result = VirtualFS.Current.GetContentRelativeFromRoot(location);
                return result;
            };
            ClientExecute.LocalTargetRemover = targetLocation =>
            {
                VirtualFS.Current.RemoveLocalContent(targetLocation);
            };
            ClientExecute.LocalTargetStreamRetriever = targetLocationItem =>
            {
                return VirtualFS.Current.GetLocalTargetStreamForWrite(targetLocationItem);
            };
            ClientExecute.LocalTargetContentWriteFinalizer = targetLocationItem =>
            {
                VirtualFS.Current.UpdateMetadataAfterWrite(targetLocationItem);
            };
        }

    }
}