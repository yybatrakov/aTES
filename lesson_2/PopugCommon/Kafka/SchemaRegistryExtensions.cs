using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using PopugCommon.KafkaMessages;
using System.IO;
using System.Threading.Tasks;

namespace PopugCommon.Kafka
{
    internal static class SchemaRegistryExtensions
    {
        public async static Task<bool> Validate<T>(this T message) where T : PopugMessage
        {
            //message.ToJson();
            //вынести в кэш
            var schemaRaw = await File.ReadAllTextAsync($"/src/PopugCommon/Scheme/{message.Event}_{message.Version}.json");
            var schema = JSchema.Parse(schemaRaw);
            return JObject.FromObject(message).IsValid(schema);
            
        }
    }
}