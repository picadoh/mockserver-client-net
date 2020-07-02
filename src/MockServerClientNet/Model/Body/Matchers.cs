using System;
using System.IO;

namespace MockServerClientNet.Model.Body
{
    public static class Matchers
    {
        private const string StrictJsonMatchType = "STRICT";

        public static BodyMatcher MatchingEmptyString()
        {
            return MatchingExactString(string.Empty);
        }

        public static BodyMatcher MatchingExactString(string value)
        {
            return new BodyMatcher
            {
                Type = Body.Types.StringType,
                StringValue = value
            };
        }

        public static BodyMatcher MatchingSubString(string value)
        {
            return new BodyMatcher
            {
                Type = Body.Types.StringType,
                StringValue = value,
                IsSubString = true
            };
        }

        public static BodyMatcher MatchingBinary(byte[] bytes)
        {
            return new BodyMatcher
            {
                Type = Body.Types.BinaryType,
                Base64Bytes = Convert.ToBase64String(bytes)
            };
        }

        public static BodyMatcher MatchingBinary(MemoryStream byteStream)
        {
            return MatchingBinary(byteStream.GetBuffer());
        }

        public static BodyMatcher MatchingBinary(Stream fileStream)
        {
            using (var byteStream = new MemoryStream())
            {
                fileStream.CopyTo(byteStream);
                return MatchingBinary(byteStream);
            }
        }

        public static BodyMatcher MatchingXml(string xml)
        {
            return new BodyMatcher
            {
                Type = Body.Types.XmlType,
                XmlValue = xml
            };
        }

        public static BodyMatcher MatchingJson(string json)
        {
            return new BodyMatcher
            {
                Type = Body.Types.JsonType,
                JsonValue = json
            };
        }

        public static BodyMatcher MatchingStrictJson(string json)
        {
            return new BodyMatcher
            {
                Type = Body.Types.JsonType,
                JsonValue = json,
                JsonMatchType = StrictJsonMatchType
            };
        }

        public static BodyMatcher MatchingXPath(string xpath)
        {
            return new BodyMatcher
            {
                Type = Body.Types.XPathType,
                XPathValue = xpath
            };
        }

        public static BodyMatcher MatchingJsonPath(string jsonPath)
        {
            return new BodyMatcher
            {
                Type = Body.Types.JsonPathType,
                JsonPathValue = jsonPath
            };
        }

        public static BodyMatcher MatchingXmlSchema(string xmlSchema)
        {
            return new BodyMatcher
            {
                Type = Body.Types.XmlSchemaType,
                XmlSchemaValue = xmlSchema
            };
        }

        public static BodyMatcher MatchingJsonSchema(string jsonSchema)
        {
            return new BodyMatcher
            {
                Type = Body.Types.JsonSchemaType,
                JsonSchemaValue = jsonSchema
            };
        }

        public static BodyMatcher MatchingRegex(string regex)
        {
            return new BodyMatcher
            {
                Type = Body.Types.RegexType,
                RegexValue = regex
            };
        }

        public static BodyMatcher Not(BodyMatcher other)
        {
            var negate = other.Clone();
            negate.IsNot = !negate.IsNot;
            return negate;
        }
    }
}