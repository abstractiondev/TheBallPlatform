using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using TheBall.Index;

namespace TheBall.CORE.Storage
{
    public static class JSONSupport
    {
        public static ExpandoObject GetJsonFromStream(Stream stream)
        {
            return GetExpandoObject(stream);
        }

        public static ExpandoObject GetJsonFromStream(string input)
        {
            return GetExpandoObject(input);
        }

        public static T GetObjectFromData<T>(byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                return GetObjectFromStream<T>(memStream);
            }
        }

        public static object GetObjectFromData(byte[] data, Type objectType)
        {
            using (var memStream = new MemoryStream(data))
            {
                return GetObjectFromStream(memStream, objectType);
            }
        }

        public static object GetObjectFromStream(Stream stream, Type objectType)
        {
            var serializer = new JsonSerializer();
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var result = serializer.Deserialize(jsonTextReader, objectType);
                return result;
            }
        }

        public static T GetObjectFromStream<T>(Stream stream)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            T result = (T) GetObjectFromStream(stream, typeof (T));
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            return result;
        }

        public static void SerializeToJSONStream(object obj, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, obj);
                jsonWriter.Flush();
            }
        }

        public static byte[] SerializeToJSONData(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                SerializeToJSONStream(obj, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static string SerializeToJSONString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T GetObjectFromString<T>(string jsonString)
        {
            var result = JsonConvert.DeserializeObject<T>(jsonString);
            return result;
        }

        public static ExpandoObject GetExpandoObject(string jsonString)
        {
            var converter = new ExpandoObjectConverter();
            var result = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, converter);
            return result;
        }


        public static ExpandoObject GetExpandoObject(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<ExpandoObject>(jsonReader);
            }
        }
    }
}
