using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using MockServerClientNet.Model;
using MockServerClientNet.Model.Body;
using Xunit;
using static MockServerClientNet.Model.Body.Contents;
using static MockServerClientNet.Model.HttpResponse;
using static MockServerClientNet.Model.HttpRequest;

namespace MockServerClientNet.Tests
{
    public class BodyContentTest(MockServerFixture fixture) : MockServerClientTest(fixture: fixture)
    {
        [Fact]
        public async Task ShouldRespondExactString()
        {
            await SetupResponseBodyExpectation(Text("response"));

            var response = await SendRequestAsync(BuildGetRequest("/"));

            Assert.Equal("response", await response.Content.ReadAsStringAsync());
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldRespondBinary()
        {
            await SetupResponseBodyExpectation(Binary(new byte[] {1, 2, 3}));

            var response = await SendRequestAsync(BuildGetRequest("/"));

            Assert.Equal(new byte[] {1, 2, 3}, await response.Content.ReadAsByteArrayAsync());
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldRespondBinaryFromMemoryStream()
        {
            using (var byteStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(byteStream))
                {
                    await writer.WriteLineAsync("This is a test line");
                }

                await SetupResponseBodyExpectation(Binary(byteStream));
            }

            var response = await SendRequestAsync(BuildGetRequest("/"));

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                Assert.Equal("This is a test line", await reader.ReadLineAsync());
            }

            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldRespondBinaryFromFile()
        {
            using (var fileStream = File.OpenRead(Path.Combine("TestFiles", "SampleText.txt")))
            {
                await SetupResponseBodyExpectation(Binary(fileStream));
            }

            var response = await SendRequestAsync(BuildGetRequest("/"));

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                Assert.Equal("This is a text file", (await reader.ReadLineAsync()).Trim('\0'));
            }

            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldRespondWithSpecifiedContentType()
        {
            await SetupResponseBodyExpectation(Binary(new byte[] {1, 2, 3}, new ContentType("application/pdf")));

            var response = await SendRequestAsync(BuildGetRequest("/"));

            Assert.Equal(new byte[] {1, 2, 3}, await response.Content.ReadAsByteArrayAsync());
            Assert.Equal("application/pdf", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldRespondXml()
        {
            const string xml = @"<department>
                                    <employee>
                                        <name>John Doe</name>
                                        <age>45</age>
                                    </employee>
                                    <employee>
                                        <name>Jane Doe</name>
                                        <age>38</age>
                                    </employee>
                                </department>";

            await SetupResponseBodyExpectation(Xml(xml));

            var response = await SendRequestAsync(BuildGetRequest("/"));

            Assert.Equal(xml, await response.Content.ReadAsStringAsync());
            Assert.Equal("application/xml", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldRespondJson()
        {
            const string json = @"{ ""department"": {
                                        ""employee"": [
                                            { ""name"": ""John Doe"", ""age"": 45 },
                                            { ""name"": ""Jane Doe"", ""age"": 38 }
                                        ]
                                    }
                                }";

            await SetupResponseBodyExpectation(Json(json));

            var response = await SendRequestAsync(BuildGetRequest("/"));

            Assert.Equal(json, await response.Content.ReadAsStringAsync());
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task SetupResponseBodyExpectation(BodyContent bodyContent)
        {
            await MockServerClient
                .When(Request()
                        .WithMethod(HttpMethod.Get),
                    Times.Unlimited())
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody(bodyContent)
                    .WithDelay(TimeSpan.Zero)
                );
        }
    }
}