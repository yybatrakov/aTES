using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using PopugCommon.KafkaMessages;
using System.IO;
using System.Threading.Tasks;

namespace PopugCommon.Kafka
{
    public static class SchemaRegistryExtensions
    {
        public async static Task<bool> Validate<T>(this T message) where T : PopugMessage
        {
            try
            {
                var schemaRaw = await File.ReadAllTextAsync($"/src/PopugCommon/SchemaRegistry/{message.Event}_{message.Version}.json");
                var schema = JSchema.Parse(schemaRaw);
                return JObject.FromObject(message).IsValid(schema);
            }
            catch { 
                return false;
            }
        }
    }
}