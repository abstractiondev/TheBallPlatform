using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public static T GetObjectFromStream<T>(Stream stream)
        {
            T result = (T)GetObjectFromStream(stream, typeof(T));
            return result;
        }

        public static object GetObjectFromStream(Stream stream, Type objectType)
        {
            var serializer = new JsonSerializer();
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                var result = serializer.Deserialize(streamReader, objectType);
                return result;
            }
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectType);
            //return serializer.ReadObject(stream);
        }

        public static void SerializeToJSONStream(object obj, Stream stream)
        {
            var serializer = new JsonSerializer();
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                serializer.Serialize(streamWriter, obj);
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
    }
}
