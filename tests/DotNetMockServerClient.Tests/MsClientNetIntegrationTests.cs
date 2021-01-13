// -----------------------------------------------------------------------
// <copyright file="MsClientNetIntegrationTests.cs" company="Calrom Ltd.">
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
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.Extensions;
    using DotNetMockServerClient.Serializer;
    using Xunit;
    using HttpRequest = DotNetMockServerClient.DataContracts.HttpRequest;
    using HttpResponse = DotNetMockServerClient.DataContracts.HttpResponse;

    /// <summary>
    /// The implementation of MockServerClientNetTests tests.
    /// </summary>
    public class MsClientNetIntegrationTests : IDisposable
    {
        private readonly MockServerClient mockServerClient;

        private readonly string mockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "localhost";

        private readonly int mockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080", CultureInfo.InvariantCulture);

        /// <summary>
        /// Initialises a new instance of the <see cref="MsClientNetIntegrationTests"/> class.
        /// </summary>
        public MsClientNetIntegrationTests()
        {
            this.mockServerClient = new MockServerClient(this.mockServerHost, this.mockServerPort);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldCheckIfThereAreAnyExpectationAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockServerClient.RetrieveActiveExpectationsAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldCheckIfThereAreAnyExpectation()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = this.mockServerClient.RetrieveActiveExpectations(request);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldClearExpectationAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            // act 1
            var result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            string responseBody = result.Item1;
            HttpStatusCode? statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.OK, statusCode.Value);
            Assert.Equal("hello", responseBody);

            // act 2
            await this.mockServerClient.ClearAsync(request).ConfigureAwait(false);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");

            result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldClearExpectation()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            // act 1
            SendRequest(getRequest, out var responseBody, out var statusCode);

            // assert
            Assert.Equal(HttpStatusCode.OK, statusCode.Value);
            Assert.Equal("hello", responseBody);

            // act 2
            this.mockServerClient.Clear(request);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            SendRequest(getRequest, out _, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRespondAccordingToExpectationAsync()
        {
            // arrange
            this.SetupLoginResponse(unlimited: true);
            var getRequest = this.BuildLoginRequest();

            // act
            var result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            string responseBody = result.Item1;
            HttpStatusCode? statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRespondAccordingToExpectation()
        {
            // arrange
            this.SetupLoginResponse(unlimited: true);
            var getRequest = this.BuildLoginRequest();

            // act
            SendRequest(getRequest, out string responseBody, out HttpStatusCode? statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRespondAccordingToExpectationOnly2TimesAsync()
        {
            // arrange
            this.SetupLoginResponse(unlimited: false, times: 2);
            var getRequest = this.BuildLoginRequest();
            var getRequest2 = this.BuildLoginRequest();
            var getRequest3 = this.BuildLoginRequest();

            // act 1
            var result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            string responseBody = result.Item1;
            HttpStatusCode? statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 2
            result = await SendRequestAsync(getRequest2).ConfigureAwait(false);

            responseBody = result.Item1;
            statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 3
            result = await SendRequestAsync(getRequest3).ConfigureAwait(false);

            statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
            getRequest2.Dispose();
            getRequest3.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRespondAccordingToExpectationOnly2Times()
        {
            // arrange
            this.SetupLoginResponse(unlimited: false, times: 2);
            var getRequest = this.BuildLoginRequest();
            var getRequest2 = this.BuildLoginRequest();
            var getRequest3 = this.BuildLoginRequest();

            // act 1
            SendRequest(getRequest, out string responseBody, out HttpStatusCode? statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 2
            SendRequest(getRequest2, out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 3
            SendRequest(getRequest3, out _, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
            getRequest2.Dispose();
            getRequest3.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRetrieveLogsAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildLoginRequest();

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            await SendRequestAsync(getRequest).ConfigureAwait(false);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            await SendRequestAsync(getRequest).ConfigureAwait(false);

            var result = await this.mockServerClient.RetrieveLogMessagesAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotEqual(0, result.Length);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRetrieveLogs()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildLoginRequest();

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(getRequest, out _, out _);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            SendRequest(getRequest, out _, out _);

            var result = this.mockServerClient.RetrieveLogMessages(request);

            // assert
            Assert.NotEqual(0, result.Length);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRetrieveRecordedRequestsAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/hello");

            this.mockServerClient
            .When(request, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            await SendRequestAsync(getRequest).ConfigureAwait(false);
            await SendRequestAsync(getRequest2).ConfigureAwait(false);

            var result = await this.mockServerClient.RetrieveRecordedRequestsAsync(request).ConfigureAwait(false);

            // assert
            Assert.Equal(2, result.Length);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRetrieveRecordedRequests()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/hello");

            this.mockServerClient
            .When(request, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(getRequest, out _, out _);
            SendRequest(getRequest2, out _, out _);

            var result = this.mockServerClient.RetrieveRecordedRequests(request);

            // assert
            Assert.Equal(2, result.Length);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldSerialise_And_Deserialise_ExpectationsAsync()
        {
            // ARRANGE
            var expectation = new Expectation(new HttpRequest(), new Times(1, false), null);
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
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldSerialise_And_Deserialise_Expectations()
        {
            // ARRANGE
            var expectation = new Expectation(new HttpRequest(), new Times(1, false), null);
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
            var serialisedExpectation = serializer.Serialize(expectation);

            // ACT
            Expectation newExpectation = serializer.DeserializeObject(serialisedExpectation);

            // ASSERT
            Assert.NotNull(newExpectation);
            Assert.Equal(expectation.HttpRequest.HeadersList.Count, newExpectation.HttpRequest.HeadersList.Count);

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
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            await SendRequestAsync(getRequest).ConfigureAwait(false);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            await SendRequestAsync(getRequest).ConfigureAwait(false);

            // act
            var result = await this.mockServerClient.VerifyAsync(
                HttpRequest.Request()
                                                .WithMethod("GET")
                                                .WithPath("/hello"),
                VerificationTimes.Exactly(2)).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerify()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
            .When(request, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            SendRequest(getRequest, out _, out _);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            SendRequest(getRequest, out _, out _);

            // act
            var result = this.mockServerClient.Verify(
                HttpRequest.Request()
                                                .WithMethod("GET")
                                                .WithPath("/hello"),
                VerificationTimes.Exactly(2));

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyMultipleAsync()
        {
            // arrange
            HttpRequest request1 = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            HttpRequest request2 = HttpRequest.Request().WithMethod("GET").WithPath("/world");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/world");

            this.mockServerClient
            .When(request1, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            this.mockServerClient
            .When(request2, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);
            await SendRequestAsync(getRequest2).ConfigureAwait(false);

            // act
            var result = await this.mockServerClient.VerifyAsync(request1, request2).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyMultiple()
        {
            // arrange
            HttpRequest request1 = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            HttpRequest request2 = HttpRequest.Request().WithMethod("GET").WithPath("/world");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/world");

            this.mockServerClient
            .When(request1, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            this.mockServerClient
            .When(request2, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out _, out _);
            SendRequest(getRequest2, out _, out _);

            // act
            var result = this.mockServerClient.Verify(request1, request2);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyXPathAsync()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>111</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);

            // act
            var result = await this.mockServerClient.VerifyAsync(request, VerificationTimes.Exactly(1)).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyXPath()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>111</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out var _, out var _);

            // act
            var result = this.mockServerClient.Verify(request, VerificationTimes.Exactly(1));

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyXPath_And_Not_ResponseAsync()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[not(text() != '111')]"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>112</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);

            async Task Act() => await this.mockServerClient.VerifyAsync(request, VerificationTimes.Exactly(1)).ConfigureAwait(false);

            // assert
            await Assert.ThrowsAsync<AssertionException>(Act).ConfigureAwait(false);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyXPath_And_Not_Response()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[not(text() != '111')]"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>112</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out var _, out var _);

            void Act() => this.mockServerClient.Verify(request, VerificationTimes.Exactly(1));

            // assert
            Assert.Throws<AssertionException>(Act);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyXPathFailsAsync()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>222</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);

            // act
            async Task Act() => await this.mockServerClient.VerifyAsync(request, VerificationTimes.Exactly(1)).ConfigureAwait(false);

            // assert
            await Assert.ThrowsAsync<AssertionException>(Act).ConfigureAwait(false);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyXPathFails()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>222</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out var _, out var _);

            // act
            void Act() => this.mockServerClient.Verify(request, VerificationTimes.Exactly(1));

            // assert
            Assert.Throws<AssertionException>(Act);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyZeroInteractionsAsync()
        {
            // act
            var result = await this.mockServerClient.VerifyZeroInteractionsAsync().ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyZeroInteractions()
        {
            // act
            var result = this.mockServerClient.VerifyZeroInteractions();

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task WhenExpectationsAreLoadedFromFile_ShoulRespondFromTheConfiguredRoutesAsync()
        {
            // arrange
            var filePath = Path.Combine("ExpectationFiles", "TestExpectations.json");
            var getRequest = this.BuildGetRequest("/hello?id=1");
            var getRequest2 = this.BuildGetRequest("/hello2");

            // act
            await this.mockServerClient.LoadExpectationsFromFileAsync(filePath).ConfigureAwait(false);

            var result = SendRequestAsync(getRequest).ConfigureAwait(false);

            var otherResult = SendRequestAsync(getRequest2).ConfigureAwait(false);

            // assert
            var result1 = await result;
            var result2 = await otherResult;
            Assert.NotNull(result1.Item1);
            Assert.NotNull(result2.Item1);

            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void WhenExpectationsAreLoadedFromFile_ShoulRespondFromTheConfiguredRoutes()
        {
            // arrange
            var filePath = Path.Combine("ExpectationFiles", "TestExpectations.json");
            var getRequest = this.BuildGetRequest("/hello?id=1");
            var getRequest2 = this.BuildGetRequest("/hello2");

            // act
            this.mockServerClient.LoadExpectationsFromFile(filePath);
            SendRequest(getRequest, out var responseBody1, out _);
            SendRequest(getRequest2, out var responseBody2, out _);

            // assert
            Assert.NotNull(responseBody1);
            Assert.NotNull(responseBody2);
            getRequest.Dispose();
            getRequest2.Dispose();
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
            this.mockServerClient.Reset();
            this.mockServerClient.Dispose();
        }

        private static async Task<Tuple<string, HttpStatusCode?>> SendRequestAsync(HttpRequestMessage request)
        {
            string responseBody;
            HttpStatusCode? statusCode;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.SendAsync(request).ConfigureAwait(false))
            using (HttpContent content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = await content.ReadAsStringAsync().ConfigureAwait(false);
            }

            return new Tuple<string, HttpStatusCode?>(responseBody, statusCode);
        }

        private static void SendRequest(HttpRequestMessage request, out string responseBody, out HttpStatusCode? statusCode)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = client.SendAsync(request).Result)
            using (HttpContent content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = content.ReadAsStringAsync().Result;
            }
        }

        private HttpRequestMessage BuildGetRequest(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://" + this.mockServerHost + ":" + this.mockServerPort + path),
            };
            return request;
        }

        private HttpRequestMessage BuildLoginRequest(bool ssl = false)
        {
            string scheme = ssl ? "https" : "http";
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(scheme + "://" + this.mockServerHost + ":" + this.mockServerPort + "/login?returnUrl=/account"),
                Content = new StringContent("{\"username\": \"foo\", \"password\": \"bar\"}"),
            };

            return request;
        }

        private HttpRequestMessage BuildPostRequest(string path, string body, string mediaType)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://" + this.mockServerHost + ":" + this.mockServerPort + path),
                Content = new StringContent(body, Encoding.UTF8, mediaType),
            };
            return request;
        }

        private void SetupLoginResponse(bool ssl = false, bool unlimited = true, int times = 0)
        {
            this.mockServerClient
            .When(
                HttpRequest.Request()
                    .WithMethod("POST")
                    .WithPath("/login")
                    .WithQueryStringParameters(
                    new Parameter("returnUrl", "/account"))
                    .WithBody("{\"username\": \"foo\", \"password\": \"bar\"}")
                    .WithSecure(ssl),
                unlimited ? Times.Unlimited() : Times.Exactly(times))
            .Respond(HttpResponse.Response()
                        .WithStatusCode(401)
                        .WithHeaders(
                        new Header("Content-Type", "application/json; charset=utf-8"),
                        new Header("Cache-Control", "public, max-age=86400"))
                        .WithBody("{ \"message\": \"incorrect username and password combination\" }")
                        .WithDelay(TimeSpan.FromSeconds(0)));
        }
    }
}
