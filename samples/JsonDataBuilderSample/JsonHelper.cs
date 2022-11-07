using Newtonsoft.Json;

namespace JsonDataBuilderSample
{
    public class JsonHelper
    {
        public JsonHelper()
        {

        }

        // file with GeoJSON
        public static void SerializeToFile(string path, object? value, JsonSerializerSettings? settings = null)
        {
            using StreamWriter file = File.CreateText(path);

            var serializer = NetTopologySuite.IO.GeoJsonSerializer.CreateDefault(settings);

            serializer.CheckAdditionalContent = true;

            serializer.Serialize(file, value);
        }
    }
}
