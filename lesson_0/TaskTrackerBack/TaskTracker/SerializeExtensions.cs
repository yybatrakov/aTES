using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static Newtonsoft.Json.JsonConvert;

namespace Mxm.Kafka;

public static class SerializeExtensions
{
    public static readonly XmlSerializerNamespaces EmptyXmlSerializerNamespaces = new();

    static SerializeExtensions()
    {
        EmptyXmlSerializerNamespaces.Add("", "");
    }

    public static string XmlSerializeToString<T>(this T obj)
    {
        return obj.XmlSerializeToString(null, null, false);
    }

    public static string XmlSerializeToString<T>(this T obj, Encoding encoding)
    {
        return obj.XmlSerializeToString(encoding, null, false);
    }

    public static string XmlSerializeToString<T>(
        this T obj,
        Encoding encoding,
        XmlSerializerNamespaces xmlSerializerNamespaces)
    {
        return obj.XmlSerializeToString(encoding, xmlSerializerNamespaces, false);
    }

    public static string XmlSerializeToString<T>(
        this T obj,
        Encoding encoding,
        XmlSerializerNamespaces ns,
        bool omitXmlDeclaration)
    {
        return obj.XmlSerializeToString(encoding, ns, omitXmlDeclaration, true, true);
    }

    public static string XmlSerializeToString<T>(
        this T obj,
        Encoding encoding,
        XmlSerializerNamespaces ns,
        bool omitXmlDeclaration,
        bool attributesOnNewLine,
        bool indentElements)
    {
        return obj.XmlSerializeToString(encoding, ns, omitXmlDeclaration, attributesOnNewLine, indentElements, null);
    }

    public static string XmlSerializeToString<T>(
        this T obj,
        Encoding encoding,
        XmlSerializerNamespaces ns,
        bool omitXmlDeclaration,
        bool attributesOnNewLine,
        bool indentElements,
        string newLineChars)
    {
        using var withCustomEncoding = new StringWriterWithCustomEncoding(encoding);
        
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = omitXmlDeclaration,
            NewLineOnAttributes = attributesOnNewLine,
            Indent = indentElements
        };
        if (newLineChars != null)
        {
            settings.NewLineChars = newLineChars;
            settings.NewLineHandling = NewLineHandling.Replace;
        }

        using var xmlWriter = XmlWriter.Create(withCustomEncoding, settings);
        new XmlSerializer(obj.GetType()).Serialize(xmlWriter, obj, ns);
        return withCustomEncoding.ToString();
    }

    public static string XmlSerializeToStringDefaultNoFormat<T>(this T obj)
    {
        return obj.XmlSerializeToString(null, EmptyXmlSerializerNamespaces, false, false, false);
    }

    public static T XmlDeserializeFromString<T>(string objectData)
    {
        return (T)XmlDeserializeFromString(objectData, typeof(T));
    }

    public static object XmlDeserializeFromString(string objectData, Type type)
    {
        using var textReader = new StringReader(objectData);
        return new XmlSerializer(type).Deserialize(textReader);
    }

    public static T XmlDeserializeFromStringNoNamespace<T>(string objectData)
    {
        return (T)XmlDeserializeFromStringNoNamespace(objectData, typeof(T));
    }

    public static object XmlDeserializeFromStringNoNamespace(string objectData, Type type)
    {
        using TextReader input = new StringReader(objectData);
        using var xmlTextReader = new XmlTextReader(input);

        xmlTextReader.Namespaces = false;
        
        return new XmlSerializer(type).Deserialize(xmlTextReader);
    }

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