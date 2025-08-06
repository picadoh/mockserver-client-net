using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class RetrieveRecordedEventsTest(MockServerFixture fixture) : MockServerClientTest(fixture: fixture)
    {
        [Fact]
        public async Task ShouldVerifyRetrieveRecordedEventsWorksWithoutContentType()
        {
            var expectedRequest = Request().WithMethod(HttpMethod.Post);

            // set expectation
            await MockServerClient
                .When(expectedRequest)
                .RespondAsync(Response().WithStatusCode(200).WithBody("{}"));

            // act
            var content = new StringContent("""{ "foo": "bar" }""");

            await SendRequestAsync(BuildRequest(HttpMethod.Post, "/test", content));

            var response = await MockServerClient.RetrieveRecordedRequestsAsync(expectedRequest);
            
            // assert
            Assert.NotEmpty(response);
            Assert.Equal("""{ "foo": "bar" }""", response[0].Body.StringValue);
        }
        
        [Fact]
        public async Task ShouldVerifyRetrieveRecordedEventsWorksWithJsonContentType()
        {
            // arrange
            var expectedRequest = Request().WithMethod(HttpMethod.Post);
            
            await MockServerClient
                .When(expectedRequest)
                .RespondAsync(Response().WithStatusCode(200).WithBody("{}"));

            // act
            var content = new StringContent("""{ "foo": "bar" }""");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await SendRequestAsync(BuildRequest(HttpMethod.Post, "/test", content));

            var response = await MockServerClient.RetrieveRecordedRequestsAsync(expectedRequest);
            
            // assert
            Assert.NotEmpty(response);
            Assert.Equal("""{"foo":"bar"}""", response[0].Body.JsonValue);
        }

        [Fact]
        public async Task ShouldVerifyRetrieveRecordedEventsWorksWithXmlContentType()
        {
            // arrange
            var expectedRequest = Request().WithMethod(HttpMethod.Patch);

            await MockServerClient
                .When(expectedRequest)
                .RespondAsync(Response().WithStatusCode(200).WithBody("{}"));

            // act
            var content = new StringContent("<foo>bar</foo>", Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            await SendRequestAsync(BuildRequest(HttpMethod.Patch, "/test", content));
            
            var response = await MockServerClient.RetrieveRecordedRequestsAsync(expectedRequest);

            // assert
            Assert.NotEmpty(response);
            Assert.Equal("<foo>bar</foo>", response[0].Body.XmlValue);
        }

        [Fact]
        public async Task ShouldVerifyRetrieveRecordedEventsWorksWithOctetStreamContentType()
        {
            // arrange
            var expectedRequest = Request().WithMethod(HttpMethod.Put);

            await MockServerClient
                .When(expectedRequest)
                .RespondAsync(Response().WithStatusCode(200).WithBody("{}"));

            // act
            var binaryData = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello" in bytes
            var content = new ByteArrayContent(binaryData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            await SendRequestAsync(BuildRequest(HttpMethod.Put, "/test", content));
            
            var response = await MockServerClient.RetrieveRecordedRequestsAsync(expectedRequest);

            // assert
            Assert.NotEmpty(response);
            Assert.Equal(Convert.ToBase64String(binaryData), response[0].Body.Base64Bytes);
        }
    }
}
