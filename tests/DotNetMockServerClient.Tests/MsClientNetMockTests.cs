// -----------------------------------------------------------------------
// <copyright file="MsClientNetMockTests.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.DataContracts.Serializer;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using HttpRequest = DotNetMockServerClient.DataContracts.HttpRequest;

    /// <summary>
    /// The implementation of MockServerClientNetTests tests.
    /// </summary>
    public class MsClientNetMockTests : IDisposable
    {
        private readonly Mock<IMockServerClient> mockMockServerClient;

        private readonly string mockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "localhost";

        private readonly int mockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080", CultureInfo.InvariantCulture);

        private MockServerClient mockServerClient;

        private HttpResponseMessage httpResponseMessage;

        private Mock<HttpMessageHandler> mockHttpMessageHandler;

        private HttpClient httpClient;

        private bool canDispose;

        private bool canReset;

        /// <summary>
        /// Initialises a new instance of the <see cref="MsClientNetMockTests"/> class.
        /// </summary>
        public MsClientNetMockTests()
        {
            this.canDispose = true;
            this.canReset = true;

            var result1 = new List<Expectation>();
            this.mockMockServerClient = new Mock<IMockServerClient>();
            this.mockMockServerClient.Setup(x => x.RetrieveActiveExpectationsAsync(It.IsAny<HttpRequest>())).ReturnsAsync(result1.ToArray()).Verifiable();
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_ActualSendExpectationAsync()
        {
            // arrange
            var sendRequest = new Expectation();

            // Always get a 'Cannot access a disposed object in SendRequest' just for this test.
            this.canDispose = false;

            this.CreateClient(string.Empty, HttpStatusCode.Created);

            // act
            await this.mockServerClient.SendExpectationAsync(sendRequest).ConfigureAwait(false);

            // assert
            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test RetrieveActiveExpectationsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_MockRetrieveActiveExpectationsAsync()
        {
            this.CreateClient("Unused");

            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockMockServerClient.Object.RetrieveActiveExpectationsAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test RetrieveActiveExpectationsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_MockSendRequestAsync_Fails()
        {
            // arrange
            // Reset fails if CreateClient is HttpStatusCode.BadRequest
            this.canReset = false;

            this.CreateClient(string.Empty, HttpStatusCode.BadRequest);

            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            try
            {
                await this.mockServerClient.RetrieveLogMessagesAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // assert
                Assert.NotNull(ex);
            }
        }

        /// <summary>
        /// Test RetrieveActiveExpectationsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_RetrieveActiveExpectationsAsync()
        {
            // arrange
            var fp = Path.Combine("ExpectationFiles", "TestExpectations.json");
            var fb = File.ReadAllBytes(fp);
            var fs = Encoding.UTF8.GetString(fb, 0, fb.Length);

            this.CreateClient(fs);

            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockServerClient.RetrieveActiveExpectationsAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_RetrieveLogMessagesAsync()
        {
            // arrange
            var expectedResult = "[{'id':1,'value':'1'}]";

            this.CreateClient(expectedResult);

            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockServerClient.RetrieveLogMessagesAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Test RetrieveRecordedRequestsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_RetrieveRecordedRequestsAsync()
        {
            // arrange
            JsonSerializer<HttpRequest> s = new JsonSerializer<HttpRequest>();

            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");
            HttpRequest testRequest = HttpRequest.Request().WithMethod("TEST");

            var jsonRequest = "[" + s.Serialize(testRequest) + "]";

            this.CreateClient(jsonRequest);

            // act
            var result = await this.mockServerClient.RetrieveRecordedRequestsAsync(httpRequest).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            Assert.Equal("TEST", result[0].Method);
        }

        /// <summary>
        /// Test ClearAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_ClearAsync()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.ClearAsync(httpRequest).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test ClearAsync.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_Clear()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.Clear(httpRequest);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test VerifyAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_VerifyAsync()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result1 = await this.mockServerClient.VerifyAsync(httpRequest).ConfigureAwait(false);

            var result2 = await this.mockServerClient.VerifyAsync(httpRequest, VerificationTimes.Once()).ConfigureAwait(false);

            // assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test VerifyZeroInteractions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_VerifyZeroInteractionsAsync()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.VerifyZeroInteractionsAsync().ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test VerifyZeroInteractions.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_VerifyZeroInteractions()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.VerifyZeroInteractions();

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_IsRunningAsync_Okay()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.IsRunningAsync(1).ConfigureAwait(false);

            // assert
            Assert.True(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test ShouldCheck_IsRunningAsync_Fails.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_IsRunningAsync_Fails()
        {
            // arrange
            this.CreateClient(string.Empty, HttpStatusCode.NotFound);

            // act
            var result = await this.mockServerClient.IsRunningAsync(2).ConfigureAwait(false);

            // assert
            Assert.False(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_IsRunning()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.IsRunning(1);

            // assert
            Assert.True(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test StopAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_StopAsync()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.ClearAsync(httpRequest).ConfigureAwait(false);

            this.CreateClient(string.Empty, HttpStatusCode.Accepted);

            await this.mockServerClient.StopAsync(true).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test Stop.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_Stop()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.Clear(httpRequest);

            this.CreateClient(string.Empty, HttpStatusCode.Accepted);

            this.mockServerClient.Stop(true);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldSerialise_And_Deserialise_ExpectationsAsync()
        {
            // ARRANGE
            var expectation = new Expectation(new HttpRequest(), new DataContracts.Times(1, false), null);
            expectation.HttpRequest.WithBody(new BodyCheck()
            {
                BodyString = "<hello>world</hello>",
                ContentType = "something",
                Negate = false,
                Regex = "funny",
                SubString = true,
                Type = "may",
                XPath = "happen",
            });
            expectation.HttpRequest.WithCookie(new DotNetMockServerClient.DataContracts.Cookie("yummy", "cookies"));
            expectation.HttpRequest.WithHeaders("header", "Locklear", "thought", "was", "hot");
            expectation.HttpRequest.WithKeepAlive(true);
            expectation.HttpRequest.WithMethod("POST-IT");
            expectation.HttpRequest.WithPath("long and winding");
            expectation.HttpRequest.WithQueryStringParameters(new Parameter("no", "parameters", "please"));
            expectation.HttpRequest.WithSecure(false);

            var serializer = new JsonSerializer<Expectation>();
            using (MemoryStream memStream = new MemoryStream(100))
            {
                await serializer.SerializeAsync(expectation, memStream).ConfigureAwait(false);

                // ACT
                memStream.Position = 0;
                var deseralizedPayload = await serializer.DeserializeObjectAsync(memStream).ConfigureAwait(false);

                // ASSERT
                Assert.NotNull(deseralizedPayload);
                Assert.Equal(expectation.HttpRequest.HeadersList.Count, deseralizedPayload.HttpRequest.HeadersList.Count);
            }

            using (MemoryStream memStream = new MemoryStream(100))
            {
                // ARRANGE
                var serialisedExpectation = "{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).* http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xPath\":null,\"regex\":null,\"type\":\"STRING\",\"bodyString\":\"\",\"subString\":true,\"negate\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"isSecure\":null,\"isKeepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"count\":1,\"isUnlimited\":false},\"timeToLive\":null}";

                UTF8Encoding uniEncoding = new UTF8Encoding();
                byte[] firstString = uniEncoding.GetBytes(serialisedExpectation);
                memStream.Write(firstString, 0, firstString.Length);

                // ACT
                memStream.Position = 0;
                Expectation newExpectation = await serializer.DeserializeObjectAsync(memStream).ConfigureAwait(false);

                // ASSERT
                Assert.NotNull(newExpectation);
                Assert.Single(newExpectation.HttpRequest.HeadersList);
            }

            using (MemoryStream memStream = new MemoryStream(100))
            {
                // ARRANGE
                var serialisedExpectation = "[{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).*http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"parameters\":{},\"cookies\":{},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xpath\":null,\"regex\":null,\"type\":\"STRING\",\"string\":\"\",\"subString\":true,\"not\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"secure\":null,\"keepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><s:Envelope xmlns:s=\\\"http://schemas.xmlsoap.org/soap/envelope/\\\" xmlns:a=\\\"http://www.w3.org/2005/08/addressing\\\"><s:Header><a:Action s:mustUnderstand=\\\"1\\\">http://webservices.amadeus.com/SATRQT_13_2_1A</a:Action><h:Session xmlns:h=\\\"http://xml.amadeus.com/2010/06/Session_v3\\\" TransactionStatusCode=\\\"InSeries\\\"><h:SessionId>01J2JLAJ0O</h:SessionId><h:SequenceNumber>3</h:SequenceNumber><h:SecurityToken>OULV8XFKFZ2X30QWH4MI0NT8C</h:SecurityToken></h:Session><a:MessageID>urn:uuid:52a1120a-c90a-4124-9140-0bde7aee9705</a:MessageID><a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo><a:To s:mustUnderstand=\\\"1\\\">https://nodea1.test.webservices.amadeus.com/1ASIWAGMBA</a:To></s:Header><s:Body><Air_MultiAvailability xmlns=\\\"http://xml.amadeus.com/SATRQT_13_2_1A\\\"><messageActionDetails><functionDetails><businessFunction>1</businessFunction><actionCode>55</actionCode></functionDetails></messageActionDetails></Air_MultiAvailability></s:Body></s:Envelope>\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"remainingTimes\":1,\"unlimited\":false},\"timeToLive\":null},{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).*http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"parameters\":{},\"cookies\":{},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xpath\":null,\"regex\":null,\"type\":\"STRING\",\"string\":\"\",\"subString\":true,\"not\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"secure\":null,\"keepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><s:Envelope xmlns:s=\\\"http://schemas.xmlsoap.org/soap/envelope/\\\" xmlns:a=\\\"http://www.w3.org/2005/08/addressing\\\"><s:Header><a:Action s:mustUnderstand=\\\"1\\\">http://webservices.amadeus.com/SATRQT_13_2_1A</a:Action><h:Session xmlns:h=\\\"http://xml.amadeus.com/2010/06/Session_v3\\\" TransactionStatusCode=\\\"InSeries\\\"><h:SessionId>01J2JLAJ0O</h:SessionId><h:SequenceNumber>2</h:SequenceNumber><h:SecurityToken>OULV8XFKFZ2X30QWH4MI0NT8C</h:SecurityToken></h:Session><a:MessageID>urn:uuid:cc912ea6-8967-4fb6-b35c-6f22f22e0451</a:MessageID><a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo><a:To s:mustUnderstand=\\\"1\\\">https://nodea1.test.webservices.amadeus.com/1ASIWAGMBA</a:To></s:Header><s:Body><Air_MultiAvailability xmlns=\\\"http://xml.amadeus.com/SATRQT_13_2_1A\\\"><messageActionDetails><functionDetails><businessFunction>1</businessFunction><actionCode>55</actionCode></functionDetails></messageActionDetails></Air_MultiAvailability></s:Body></s:Envelope>\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"remainingTimes\":1,\"unlimited\":false},\"timeToLive\":null},{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).*http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"parameters\":{},\"cookies\":{},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xpath\":null,\"regex\":null,\"type\":\"STRING\",\"string\":\"\",\"subString\":true,\"not\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"secure\":null,\"keepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><s:Envelope xmlns:s=\\\"http://schemas.xmlsoap.org/soap/envelope/\\\" xmlns:a=\\\"http://www.w3.org/2005/08/addressing\\\"><s:Header><a:Action s:mustUnderstand=\\\"1\\\">http://webservices.amadeus.com/SATRQT_13_2_1A</a:Action><h:AMA_SecurityHostedUser xmlns:h=\\\"http://xml.amadeus.com/2010/06/Security_v1\\\"><h:UserID POS_Type=\\\"1\\\" PseudoCityCode=\\\"NYCBA08AB\\\" AgentDutyCode=\\\"SU\\\" RequestorType=\\\"U\\\"><RequestorID xmlns=\\\"http://xml.amadeus.com/2010/06/Types_v1\\\"><CompanyName xmlns=\\\"http://www.iata.org/IATA/2007/00/IATA2010.1\\\">BA</CompanyName></RequestorID></h:UserID></h:AMA_SecurityHostedUser><h:Session xmlns:h=\\\"http://xml.amadeus.com/2010/06/Session_v3\\\" TransactionStatusCode=\\\"Start\\\" /><a:MessageID>urn:uuid:1f3ddfa7-20ca-49e4-b4ef-e7e182345d7e</a:MessageID><a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo><Security xmlns=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\\\"><wsse:UsernameToken xmlns:wsu=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\\\" wsu:Id=\\\"SecurityToken-01f263b7-c84f-4cea-90a0-cd9211a3ec39\\\" xmlns:wsse=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\\\"><wsse:Username>WSBALIM</wsse:Username><wsse:Password Type=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest\\\">szW/V7sCd+7p5acja9WJfEDkBME=</wsse:Password><wsse:Nonce EncodingType=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\\\">SXPz4FuaDCbiCplSqEWX/A==</wsse:Nonce><wsu:Created>2020-02-24T09:55:29Z</wsu:Created></wsse:UsernameToken></Security><a:To s:mustUnderstand=\\\"1\\\">https://nodea1.test.webservices.amadeus.com/1ASIWAGMBA</a:To></s:Header><s:Body><Air_MultiAvailability xmlns=\\\"http://xml.amadeus.com/SATRQT_13_2_1A\\\"><messageActionDetails><functionDetails><businessFunction>1</businessFunction><actionCode>44</actionCode></functionDetails></messageActionDetails><requestSection><availabilityProductInfo><availabilityDetails><departureDate>130420</departureDate></availabilityDetails><departureLocationInfo><cityAirport>LHR</cityAirport></departureLocationInfo><arrivalLocationInfo><cityAirport>JFK</cityAirport></arrivalLocationInfo></availabilityProductInfo><numberOfSeatsInfo><numberOfPassengers>1</numberOfPassengers></numberOfSeatsInfo><airlineOrFlightOption><flightIdentification><airlineCode>BA</airlineCode></flightIdentification></airlineOrFlightOption><availabilityOptions><productTypeDetails><typeOfRequest>TN</typeOfRequest></productTypeDetails><optionInfo><type>FLO</type><arguments>ON</arguments></optionInfo><optionInfo><type>FLO</type><arguments>OD</arguments></optionInfo></availabilityOptions></requestSection></Air_MultiAvailability></s:Body></s:Envelope>\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"remainingTimes\":1,\"unlimited\":false},\"timeToLive\":null}]";
                UTF8Encoding uniEncoding = new UTF8Encoding();
                byte[] firstString = uniEncoding.GetBytes(serialisedExpectation);
                memStream.Write(firstString, 0, firstString.Length);

                // ACT
                memStream.Position = 0;
                List<Expectation> expectations = await serializer.DeserializeListAsync(memStream).ConfigureAwait(false);

                // ASSERT
                Assert.NotNull(expectations);
                Assert.Equal(3, expectations.Count);
            }
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldSerialise_And_Deserialise_Expectations()
        {
            // ARRANGE
            var expectation = new Expectation(new HttpRequest(), new DataContracts.Times(1, false), null);
            expectation.HttpRequest.WithBody(new BodyCheck()
            {
                BodyString = "<hello>world</hello>",
                ContentType = "something",
                Negate = false,
                Regex = "funny",
                SubString = true,
                Type = "may",
                XPath = "happen",
            });
            expectation.HttpRequest.WithCookie(new DotNetMockServerClient.DataContracts.Cookie("yummy", "cookies"));
            expectation.HttpRequest.WithHeaders("header", "Locklear", "thought", "was", "hot");
            expectation.HttpRequest.WithKeepAlive(true);
            expectation.HttpRequest.WithMethod("POST-IT");
            expectation.HttpRequest.WithPath("long and winding");
            expectation.HttpRequest.WithQueryStringParameters(new Parameter("no", "parameters", "please"));
            expectation.HttpRequest.WithSecure(false);
            expectation.HttpResponse = new HttpResponse();
            expectation.HttpResponse.WithHeaders("heath", "ledger", "died", "in", "2007");

            var serializer = new JsonSerializer<Expectation>();
            var serialisedExpectation = serializer.Serialize(expectation);

            // ACT
            Expectation newExpectation = serializer.DeserializeObject(serialisedExpectation);

            // ASSERT
            Assert.NotNull(newExpectation);
            Assert.Equal(expectation.HttpRequest.HeadersList.Count, newExpectation.HttpRequest.HeadersList.Count);
            Assert.Equal(expectation.HttpRequest.CookiesList.Count, newExpectation.HttpRequest.CookiesList.Count);
            Assert.Equal(expectation.HttpRequest.ParametersList.Count, newExpectation.HttpRequest.ParametersList.Count);
            Assert.Equal(expectation.HttpResponse.HeadersList.Count, newExpectation.HttpResponse.HeadersList.Count);

            // ARRANGE
            serialisedExpectation = "{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).* http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xPath\":null,\"regex\":null,\"type\":\"STRING\",\"bodyString\":\"\",\"subString\":true,\"negate\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"isSecure\":null,\"isKeepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"count\":1,\"isUnlimited\":false},\"timeToLive\":null}";

            // ACT
            newExpectation = serializer.DeserializeObject(serialisedExpectation);

            // ASSERT
            Assert.NotNull(newExpectation);
            Assert.Single(newExpectation.HttpRequest.HeadersList);

            // ARRANGE
            serialisedExpectation = "[{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).*http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"parameters\":{},\"cookies\":{},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xpath\":null,\"regex\":null,\"type\":\"STRING\",\"string\":\"\",\"subString\":true,\"not\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"secure\":null,\"keepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><s:Envelope xmlns:s=\\\"http://schemas.xmlsoap.org/soap/envelope/\\\" xmlns:a=\\\"http://www.w3.org/2005/08/addressing\\\"><s:Header><a:Action s:mustUnderstand=\\\"1\\\">http://webservices.amadeus.com/SATRQT_13_2_1A</a:Action><h:Session xmlns:h=\\\"http://xml.amadeus.com/2010/06/Session_v3\\\" TransactionStatusCode=\\\"InSeries\\\"><h:SessionId>01J2JLAJ0O</h:SessionId><h:SequenceNumber>3</h:SequenceNumber><h:SecurityToken>OULV8XFKFZ2X30QWH4MI0NT8C</h:SecurityToken></h:Session><a:MessageID>urn:uuid:52a1120a-c90a-4124-9140-0bde7aee9705</a:MessageID><a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo><a:To s:mustUnderstand=\\\"1\\\">https://nodea1.test.webservices.amadeus.com/1ASIWAGMBA</a:To></s:Header><s:Body><Air_MultiAvailability xmlns=\\\"http://xml.amadeus.com/SATRQT_13_2_1A\\\"><messageActionDetails><functionDetails><businessFunction>1</businessFunction><actionCode>55</actionCode></functionDetails></messageActionDetails></Air_MultiAvailability></s:Body></s:Envelope>\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"remainingTimes\":1,\"unlimited\":false},\"timeToLive\":null},{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).*http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"parameters\":{},\"cookies\":{},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xpath\":null,\"regex\":null,\"type\":\"STRING\",\"string\":\"\",\"subString\":true,\"not\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"secure\":null,\"keepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><s:Envelope xmlns:s=\\\"http://schemas.xmlsoap.org/soap/envelope/\\\" xmlns:a=\\\"http://www.w3.org/2005/08/addressing\\\"><s:Header><a:Action s:mustUnderstand=\\\"1\\\">http://webservices.amadeus.com/SATRQT_13_2_1A</a:Action><h:Session xmlns:h=\\\"http://xml.amadeus.com/2010/06/Session_v3\\\" TransactionStatusCode=\\\"InSeries\\\"><h:SessionId>01J2JLAJ0O</h:SessionId><h:SequenceNumber>2</h:SequenceNumber><h:SecurityToken>OULV8XFKFZ2X30QWH4MI0NT8C</h:SecurityToken></h:Session><a:MessageID>urn:uuid:cc912ea6-8967-4fb6-b35c-6f22f22e0451</a:MessageID><a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo><a:To s:mustUnderstand=\\\"1\\\">https://nodea1.test.webservices.amadeus.com/1ASIWAGMBA</a:To></s:Header><s:Body><Air_MultiAvailability xmlns=\\\"http://xml.amadeus.com/SATRQT_13_2_1A\\\"><messageActionDetails><functionDetails><businessFunction>1</businessFunction><actionCode>55</actionCode></functionDetails></messageActionDetails></Air_MultiAvailability></s:Body></s:Envelope>\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"remainingTimes\":1,\"unlimited\":false},\"timeToLive\":null},{\"httpRequest\":{\"headers\":{\"SOAPAction\":[\"(?i).*http://webservices.amadeus.com/SATRQT_13_2_1A.*\"]},\"parameters\":{},\"cookies\":{},\"method\":\"POST\",\"path\":\"/1ASIWNXGAML\",\"body\":{\"xpath\":null,\"regex\":null,\"type\":\"STRING\",\"string\":\"\",\"subString\":true,\"not\":false,\"contentType\":\"text/plain; charset=utf-16\"},\"secure\":null,\"keepAlive\":null},\"httpResponse\":{\"statusCode\":200,\"body\":\"<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><s:Envelope xmlns:s=\\\"http://schemas.xmlsoap.org/soap/envelope/\\\" xmlns:a=\\\"http://www.w3.org/2005/08/addressing\\\"><s:Header><a:Action s:mustUnderstand=\\\"1\\\">http://webservices.amadeus.com/SATRQT_13_2_1A</a:Action><h:AMA_SecurityHostedUser xmlns:h=\\\"http://xml.amadeus.com/2010/06/Security_v1\\\"><h:UserID POS_Type=\\\"1\\\" PseudoCityCode=\\\"NYCBA08AB\\\" AgentDutyCode=\\\"SU\\\" RequestorType=\\\"U\\\"><RequestorID xmlns=\\\"http://xml.amadeus.com/2010/06/Types_v1\\\"><CompanyName xmlns=\\\"http://www.iata.org/IATA/2007/00/IATA2010.1\\\">BA</CompanyName></RequestorID></h:UserID></h:AMA_SecurityHostedUser><h:Session xmlns:h=\\\"http://xml.amadeus.com/2010/06/Session_v3\\\" TransactionStatusCode=\\\"Start\\\" /><a:MessageID>urn:uuid:1f3ddfa7-20ca-49e4-b4ef-e7e182345d7e</a:MessageID><a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo><Security xmlns=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\\\"><wsse:UsernameToken xmlns:wsu=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\\\" wsu:Id=\\\"SecurityToken-01f263b7-c84f-4cea-90a0-cd9211a3ec39\\\" xmlns:wsse=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\\\"><wsse:Username>WSBALIM</wsse:Username><wsse:Password Type=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest\\\">szW/V7sCd+7p5acja9WJfEDkBME=</wsse:Password><wsse:Nonce EncodingType=\\\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\\\">SXPz4FuaDCbiCplSqEWX/A==</wsse:Nonce><wsu:Created>2020-02-24T09:55:29Z</wsu:Created></wsse:UsernameToken></Security><a:To s:mustUnderstand=\\\"1\\\">https://nodea1.test.webservices.amadeus.com/1ASIWAGMBA</a:To></s:Header><s:Body><Air_MultiAvailability xmlns=\\\"http://xml.amadeus.com/SATRQT_13_2_1A\\\"><messageActionDetails><functionDetails><businessFunction>1</businessFunction><actionCode>44</actionCode></functionDetails></messageActionDetails><requestSection><availabilityProductInfo><availabilityDetails><departureDate>130420</departureDate></availabilityDetails><departureLocationInfo><cityAirport>LHR</cityAirport></departureLocationInfo><arrivalLocationInfo><cityAirport>JFK</cityAirport></arrivalLocationInfo></availabilityProductInfo><numberOfSeatsInfo><numberOfPassengers>1</numberOfPassengers></numberOfSeatsInfo><airlineOrFlightOption><flightIdentification><airlineCode>BA</airlineCode></flightIdentification></airlineOrFlightOption><availabilityOptions><productTypeDetails><typeOfRequest>TN</typeOfRequest></productTypeDetails><optionInfo><type>FLO</type><arguments>ON</arguments></optionInfo><optionInfo><type>FLO</type><arguments>OD</arguments></optionInfo></availabilityOptions></requestSection></Air_MultiAvailability></s:Body></s:Envelope>\",\"delay\":null,\"headers\":{\"Content-Type\":[\"text/xml; charset=utf-8\"],\"Cache-Control\":[\"no-cache, no-store\"],\"connection\":[\"keep-alive\"]}},\"httpForward\":null,\"times\":{\"remainingTimes\":1,\"unlimited\":false},\"timeToLive\":null}]";

            // ACT
            var expectations = serializer.DeserializeList(serialisedExpectation);

            // ASSERT
            Assert.NotNull(expectations);
            Assert.Equal(3, expectations.Count);
        }

        /// <summary>
        /// Dispose operation.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose operation.
        /// </summary>
        /// <param name="disposing">The run identifier.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.canReset && this.canDispose)
            {
                this.mockServerClient?.Reset();
            }

            if (this.canDispose)
            {
                this.mockServerClient?.Dispose();
                this.httpResponseMessage?.Dispose();
                this.httpClient?.Dispose();
            }
        }

        private void CreateClient(string body, HttpStatusCode sc = HttpStatusCode.OK)
        {
            this.httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = sc,
                Content = new StringContent(body),
            };

            this.mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            this.mockHttpMessageHandler
               .Protected()

               // Setup the PROTECTED method to mock SendAsync call.
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())

               // Prepare the expected response of the mocked http call.
               .ReturnsAsync(this.httpResponseMessage)
               .Verifiable();

            this.mockHttpMessageHandler
               .Protected()

               // Setup the PROTECTED method to allow httpClient to call dispose.
               .Setup(
                  "Dispose",
                  ItExpr.IsAny<bool>());

            // Use real http client with mocked handler
            this.httpClient = new HttpClient(this.mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            this.mockServerClient = new MockServerClient(this.mockServerHost, this.mockServerPort, string.Empty, this.httpClient);
        }
    }
}
