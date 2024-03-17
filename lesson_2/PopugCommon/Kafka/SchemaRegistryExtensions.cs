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
                var e = message.Event;
                var ver = message.Version;

                //костыль, чтобы схемы не плодить
                if (e.Contains("stream"))
                    e = e.Replace(".created", string.Empty).Replace(".updated", string.Empty).Replace(".deleted", string.Empty);

                var schemaRaw = await File.ReadAllTextAsync($"/src/PopugCommon/SchemaRegistry/{e}_{ver}.json");
                var schema = JSchema.Parse(schemaRaw);
                return JObject.FromObject(message).IsValid(schema);
            }
            catch { 
                return false;
            }
        }
    }
}