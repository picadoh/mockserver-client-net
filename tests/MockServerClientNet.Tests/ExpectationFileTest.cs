using System.IO;
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
            SendRequest(BuildGetRequest("/hello?id=1"), out var responseBody1, out _);
            SendRequest(BuildGetRequest("/hello2"), out var responseBody2, out _);

            // assert
            Assert.NotNull(responseBody1);
            Assert.NotNull(responseBody2);
        }
    }
}