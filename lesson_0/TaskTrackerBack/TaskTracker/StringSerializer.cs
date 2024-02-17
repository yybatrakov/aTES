﻿using System.Text;
using Confluent.Kafka;

namespace Mxm.Kafka;

public class StringSerializer : ISerializer<string>
{
    public byte[] Serialize(string data, SerializationContext context)
    {
        return data == null ? null : Encoding.UTF8.GetBytes(data);
    }
}