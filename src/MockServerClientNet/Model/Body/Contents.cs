using System;
using System.IO;
using System.Net.Mime;

namespace MockServerClientNet.Model.Body
{
    public static class Contents
    {
        private const string DefaultTextContentType = MediaTypeNames.Text.Plain;
        private const string DefaultBinaryContentType = MediaTypeNames.Application.Octet;

        // the following are only supported as MediaTypeNames after .netstandard2.1
        private const string DefaultXmlContentType = "application/xml";
        private const string DefaultJsonContentType = "application/json";

        public static BodyContent EmptyText()
        {
            return Text(string.Empty);
        }

        public static BodyContent Text(string value, ContentType contentType = null)
        {
            return new BodyContent
            {
                Type = Body.Types.StringType,
                StringValue = value,
                ContentTypeValue = contentType?.ToString() ?? DefaultTextContentType
            };
        }

        public static BodyContent Binary(byte[] bytes, ContentType contentType = null)
        {
            return new BodyContent
            {
                Type = Body.Types.BinaryType,
                Base64Bytes = Convert.ToBase64String(bytes),
                ContentTypeValue = contentType?.ToString() ?? DefaultBinaryContentType
            };
        }

        public static BodyContent Binary(MemoryStream byteStream, ContentType contentType = null)
        {
            return Binary(byteStream.GetBuffer(), contentType);
        }

        public static BodyContent Binary(Stream stream, ContentType contentType = null)
        {
            using (var byteStream = new MemoryStream())
            {
                stream.CopyTo(byteStream);
                return Binary(byteStream, contentType);
            }
        }

        public static BodyContent Xml(string xml, ContentType contentType = null)
        {
            return new BodyContent
            {
                Type = Body.Types.XmlType,
                XmlValue = xml,
                ContentTypeValue = contentType?.ToString() ?? DefaultXmlContentType
            };
        }

        public static BodyContent Json(string json, ContentType contentType = null)
        {
            return new BodyContent
            {
                Type = Body.Types.JsonType,
                JsonValue = json,
                ContentTypeValue = contentType?.ToString() ?? DefaultJsonContentType
            };
        }
    }
}