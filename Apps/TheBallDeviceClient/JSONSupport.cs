using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

//using Newtonsoft.Json;

namespace TheBall.Support.DeviceClient
{
    public static class JSONSupport
    {
        public static void SerializeToJSONStream(object obj, Stream outputStream)
        {
            using (StreamWriter textWriter = new StreamWriter(outputStream))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, obj);
            }
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
