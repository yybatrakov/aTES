using System.IO;
using System.Text;

namespace Mxm.Kafka;

public class StringWriterWithCustomEncoding : StringWriter
{
    public StringWriterWithCustomEncoding(Encoding encoding)
    {
        Encoding = encoding ?? base.Encoding;
    }

    public override Encoding Encoding { get; }
}