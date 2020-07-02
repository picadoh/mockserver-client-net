using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet.Model;
using MockServerClientNet.Model.Body;
using Xunit;
using static MockServerClientNet.Model.Body.Matchers;
using static MockServerClientNet.Model.HttpResponse;
using static MockServerClientNet.Model.HttpRequest;

namespace MockServerClientNet.Tests
{
    public class BodyMatcherTest : MockServerClientTest
    {
        [Fact]
        public async Task ShouldMatchExactString()
        {
            await SetupRequestBodyExpectation(MatchingExactString("some"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "some")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "some_string")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchSubString()
        {
            await SetupRequestBodyExpectation(MatchingSubString("some"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "some_string")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "other_string")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchNotExactString()
        {
            await SetupRequestBodyExpectation(Not(MatchingExactString("some")));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "other")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "some")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchBinary()
        {
            await SetupRequestBodyExpectation(MatchingBinary(new byte[] {1, 2, 3}));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", new byte[] {1, 2, 3})
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", new byte[] {1, 2})
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchBinaryFromMemoryStream()
        {
            byte[] bytes;

            using (var byteStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(byteStream))
                {
                    await writer.WriteLineAsync("This is a test line");
                }

                bytes = byteStream.GetBuffer();

                await SetupRequestBodyExpectation(MatchingBinary(byteStream));
            }

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", bytes)
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", new byte[] {1, 2, 3})
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchBinaryFromFile()
        {
            byte[] bytes;

            using (var fileStream = File.OpenRead(Path.Combine("TestFiles", "SampleText.txt")))
            {
                using (var byteStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(byteStream);
                    bytes = byteStream.GetBuffer();
                }

                fileStream.Position = 0;

                await SetupRequestBodyExpectation(MatchingBinary(fileStream));
            }

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", bytes)
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", new byte[] {1, 2, 3})
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchStrictXml()
        {
            await SetupRequestBodyExpectation(MatchingXml(
                @"<department>
                    <employee>
                        <name>John Doe</name>
                        <age>45</age>
                    </employee>
                    <employee>
                        <name>Jane Doe</name>
                        <age>38</age>
                    </employee>
                </department>"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"<department>
                                <employee>
                                    <name>John Doe</name>
                                    <age>45</age>
                                </employee>
                                <employee>
                                    <name>Jane Doe</name>
                                    <age>38</age>
                                </employee>
                            </department>")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"<department>
                                <employee>
                                    <name>John Doe</name>
                                    <age>40</age>
                                </employee>
                            </department>")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchPartialJson()
        {
            await SetupRequestBodyExpectation(MatchingJson(
                @"{ ""department"": {
                        ""employee"": [
                            { ""name"": ""John Doe"", ""age"": 45 }
                        ]
                    }
                }"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 45, ""address"": ""World"" },
                                        { ""name"": ""Jane Doe"", ""age"": 38, ""address"": ""World"" }
                                    ]
                                }
                            }")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 40 },
                                        { ""name"": ""Jane Doe"", ""age"": 30 }
                                    ]
                                }
                            }")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchStrictJson()
        {
            await SetupRequestBodyExpectation(MatchingStrictJson(
                @"{ ""department"": {
                        ""employee"": [
                            { ""name"": ""John Doe"", ""age"": 45 },
                            { ""name"": ""Jane Doe"", ""age"": 38 }
                        ]
                    }
                }"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 45 },
                                        { ""name"": ""Jane Doe"", ""age"": 38 }
                                    ]
                                }
                            }")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 45, ""address"": ""World"" },
                                        { ""name"": ""Jane Doe"", ""age"": 38, ""address"": ""World"" }
                                    ]
                                }
                            }")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchXPath()
        {
            await SetupRequestBodyExpectation(MatchingXPath("/department/employee[age>40]/name"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"<department>
                                <employee>
                                    <name>John Doe</name>
                                    <age>45</age>
                                </employee>
                                <employee>
                                    <name>Jane Doe</name>
                                    <age>38</age>
                                </employee>
                            </department>")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"<department>
                                <employee>
                                    <name>John Doe</name>
                                    <age>40</age>
                                </employee>
                                <employee>
                                    <name>Jane Doe</name>
                                    <age>38</age>
                                </employee>
                            </department>")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchJsonPath()
        {
            await SetupRequestBodyExpectation(MatchingJsonPath("$.department.employee[?(@.age > 40)]"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 45 },
                                        { ""name"": ""Jane Doe"", ""age"": 38 }
                                    ]
                                }
                            }")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 40 },
                                        { ""name"": ""Jane Doe"", ""age"": 38 }
                                    ]
                                }
                            }")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchXmlSchema()
        {
            await SetupRequestBodyExpectation(MatchingXmlSchema(
                @"<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
                    <xs:element name=""department"">
                        <xs:complexType>
                            <xs:sequence>
                                <xs:element name=""employee"" maxOccurs=""unbounded"" minOccurs=""0"">
                                    <xs:complexType>
                                        <xs:sequence>
                                            <xs:element type=""xs:string"" name=""name""/>
                                            <xs:element type=""xs:byte"" name=""age""/>
                                        </xs:sequence>
                                    </xs:complexType>
                                </xs:element>
                            </xs:sequence>
                        </xs:complexType>
                    </xs:element>
                </xs:schema>"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"<department>
                                <employee>
                                    <name>John Doe</name>
                                    <age>45</age>
                                </employee>
                                <employee>
                                    <name>Jane Doe</name>
                                    <age>38</age>
                                </employee>
                            </department>")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"<department>
                                <employee>
                                    <name>John Doe</name>
                                </employee>
                                <employee>
                                    <name>Jane Doe</name>
                                </employee>
                            </department>")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchJsonSchema()
        {
            await SetupRequestBodyExpectation(MatchingJsonSchema(
                @"{ ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                        ""type"": ""object"",
                        ""properties"": {
                            ""department"": {
                                ""type"": ""object"",
                                ""properties"": {
                                    ""employee"": {
                                        ""type"": ""array"",
                                        ""items"": [
                                            {
                                                ""type"": ""object"",
                                                ""properties"": {
                                                    ""name"": { ""type"": ""string"" },
                                                    ""age"": { ""type"": ""integer"" }
                                                },
                                                ""required"": [ ""name"", ""age"" ]
                                            }
                                        ]
                                    }
                                },
                                ""required"": [ ""employee"" ]
                            }
                        },
                        ""required"": [ ""department"" ]
                    }"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"", ""age"": 45 },
                                        { ""name"": ""Jane Doe"", ""age"": 38 }
                                    ]
                                }
                            }")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/",
                    @"{ ""department"": {
                                    ""employee"": [
                                        { ""name"": ""John Doe"" },
                                        { ""name"": ""Jane Doe"" }
                                    ]
                                }
                            }")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldMatchRegex()
        {
            await SetupRequestBodyExpectation(MatchingRegex("some_.*_value"));

            var response = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "some_longer_value")
            );

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var failResponse = await SendRequestAsync(
                BuildRequest(HttpMethod.Post, "/", "some_value")
            );

            Assert.Equal(HttpStatusCode.NotFound, failResponse.StatusCode);
        }

        [Fact]
        public void ShouldNegateBodyMatcher()
        {
            var negated = Not(MatchingExactString("some"));

            Assert.True(negated.IsNot);

            var negatedTwice = Not(Not(MatchingExactString("some")));

            Assert.False(negatedTwice.IsNot);
        }

        private async Task SetupRequestBodyExpectation(BodyMatcher requestBody)
        {
            await MockServerClient
                .When(Request()
                        .WithMethod(HttpMethod.Post)
                        .WithBody(requestBody),
                    Times.Unlimited())
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("response")
                    .WithDelay(TimeSpan.Zero)
                );
        }
    }
}