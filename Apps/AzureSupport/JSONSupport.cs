using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using AaltoGlobalImpact.OIP;
using JsonFx.Serialization;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using TheBall.Index;
using JsonReader = JsonFx.Json.JsonReader;
using JsonWriter = JsonFx.Json.JsonWriter;

namespace AzureSupport
{
    public static class TypeSupport
    {
        public static Type GetTypeByName(string fullName)
        {
            // TODO: Reflect proper loading based on fulltype, right now fetching from this
            Assembly currAsm = Assembly.GetExecutingAssembly();
            Type type = currAsm.GetType(fullName);
            return type;
        }
    }

    public static class JSONSupport
    {
        public static ExpandoObject GetJsonFromStream(TextReader input)
        {
            var reader = new JsonReader();
            dynamic jsonObject = reader.Read(input);
            return jsonObject;
        }

        public static ExpandoObject GetJsonFromStream(string input)
        {
            var reader = new JsonReader();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var strongType = reader.Read<NodeSummaryContainer>(input);
            watch.Stop();
            var elapsed1 = watch.ElapsedMilliseconds;
            watch.Restart();
            dynamic jsonObject = reader.Read(input);
            watch.Stop();
            var elapsed2 = watch.ElapsedMilliseconds;
            return jsonObject;
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
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectType);
            return serializer.ReadObject(stream);
        }

        public static T GetObjectFromStream<T>(Stream stream)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            T result = (T) serializer.ReadObject(stream);
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            return result;
        }

        public static void SerializeToJSONStream(object obj, Stream outputStream)
        {
            var writer = new JsonWriter();
            using (StreamWriter textWriter = new StreamWriter(outputStream))
            {
                writer.Write(obj, textWriter);
            }
        }

        public static string SerializeToJSONString(object obj)
        {
            var writer = new JsonWriter();
            return writer.Write(obj);
        }

        public static T GetObjectFromString<T>(string jsonString)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var reader = new JsonReader();
            var result = reader.Read<T>(jsonString);
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
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
            var serializer = new JsonSerializer();
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                using(var jsonTextReader = new JsonTextReader(streamReader))
            {
                dynamic result = serializer.Deserialize<ExpandoObject>(jsonTextReader);
                return result;
            }
                
        }
    }
}
