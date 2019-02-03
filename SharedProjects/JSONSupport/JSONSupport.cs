using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TheBall.CORE.Storage
{
    public static class JSONSupport
    {
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
            T result = (T)GetObjectFromStream(stream, typeof(T));
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            return result;
        }


        public static string SerializeToJSONString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
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
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                dynamic result = serializer.Deserialize<ExpandoObject>(jsonTextReader);
                return result;
            }

        }

        public static byte[] SerializeToJSONData(object obj)
        {
            using(var memoryStream = new MemoryStream())
            {
                SerializeToJSONStream(obj, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static void SerializeToJSONStream(object obj, Stream outputStream)
        {
            var serializer = new JsonSerializer();
            using (var streamWriter = new StreamWriter(outputStream, Encoding.UTF8))
            {
                serializer.Serialize(streamWriter, obj);
            }
        }

    }
}
