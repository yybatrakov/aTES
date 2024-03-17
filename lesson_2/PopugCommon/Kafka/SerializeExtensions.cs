using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static Newtonsoft.Json.JsonConvert;

namespace PopugCommon.Kafka
{

    public static class SerializeExtensions
    {

        public static string ToJson<T>(this T obj)
        {

            return SerializeObject(obj);
        }

        public static T FromJson<T>(this T obj, string data)
        {
            return DeserializeObject<T>(data);
        }

        public static T FromJson<T>(this string data)
        {
            return DeserializeObject<T>(data);
        }
    }

}