using System.IO;
using System.Net.Http;
using MockServerClientNet.Extensions;
using Xunit;

namespace MockServerClientNet.Tests
{
    public class ExpectationFileTest : MockServerClientTest
    {
        [Fact]
        public void WhenExpectationsAreLoadedFromFile_ShouldRespondFromTheConfiguredRoutes()
        {
            // arrange
            var filePath = Path.Combine("ExpectationFiles", "TestExpectations.json");
            MockServerClient.LoadExpectationsFromFile(filePath);

            // act
            SendRequest(BuildGetRequest("/entity1?id=1"), out var responseBody1, out _);
            SendRequest(BuildGetRequest("/entity2"), out var responseBody2, out _);
            SendRequest(BuildRequest(HttpMethod.Post, "/entity3", "request3"), out var responseBody3, out _);
            SendRequest(BuildRequest(HttpMethod.Post, "/entity4", "request4"), out var responseBody4, out _);

            // assert
            Assert.Equal("response1", responseBody1);
            Assert.Equal("response2", responseBody2);
            Assert.Equal("response3", responseBody3);
            Assert.Equal("response4", responseBody4);
        }
    }
}