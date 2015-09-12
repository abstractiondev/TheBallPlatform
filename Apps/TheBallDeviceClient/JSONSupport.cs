using System.IO;
using System.Text;
//using Newtonsoft.Json;
using JsonReader = JsonFx.Json.JsonReader;
using JsonWriter = JsonFx.Json.JsonWriter;

namespace TheBall.Support.DeviceClient
{
    public static class JSONSupport
    {
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
            var reader = new JsonReader();
            return reader.Read<T>(jsonString);
        }

        public static T GetObjectFromStream<T>(Stream stream)
        {
            using (TextReader textReader = new StreamReader(stream, Encoding.UTF8))
            using(var reader = new Newtonsoft.Json.JsonTextReader(textReader))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                var result = serializer.Deserialize<T>(reader);
                return result;
                //var result = reader.
            }
            //return JsonConvert.DeserializeObject<T>(stream);
#if broken
           var reader = new JsonReader();
            using (TextReader textReader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.Read<T>(textReader);
            }
#endif
        }
    }
}
