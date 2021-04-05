﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MockServerClientNet.Model;
using MockServerClientNet.Model.Body;
using Newtonsoft.Json;

namespace MockServerClientNet.Extensions
{
    public static class MockServerExtensions
    {
        public static void LoadExpectationsFromFile(this MockServerClient mockServerClient, string expectationsFilePath)
        {
            LoadExpectationsFromFileAsync(mockServerClient, expectationsFilePath).AwaitResult();
        }

        public static async Task LoadExpectationsFromFileAsync(this MockServerClient mockServerClient,
            string expectationsFilePath)
        {
            if (!File.Exists(expectationsFilePath))
            {
                throw new FileNotFoundException($"File: {expectationsFilePath} not found!");
            }

            var fileContent = File.ReadAllText(expectationsFilePath);
            var expectations = JsonConvert.DeserializeObject<IEnumerable<Expectation>>(fileContent);

            foreach (var expectation in expectations)
            {
                var httpRequest = HttpRequest.Request();
                if (expectation.HttpResponseTemplate != null)
                {
                    var httpResponse = HttpResponseTemplate.ResponseTemplate();
                    await SetupMockServerRequestAsync(mockServerClient, expectation, httpRequest, httpResponse);
                }
                else
                {
                    var httpResponse = HttpResponse.Response();
                    await SetupMockServerRequestAsync(mockServerClient, expectation, httpRequest, httpResponse);
                }
            }
        }

        private static async Task SetupMockServerRequestAsync(
            MockServerClient mockServerClient,
            Expectation expectation,
            HttpRequest httpRequest,
            HttpResponse httpResponse)
        {
            var isSecure = expectation.HttpRequest.IsSecure.HasValue && expectation.HttpRequest.IsSecure.Value;
            var unlimitedTimes = expectation.Times == null || expectation.Times.IsUnlimited;

            await mockServerClient
                .When(httpRequest
                        .WithMethod(expectation.HttpRequest.Method)
                        .WithPath(expectation.HttpRequest.Path)
                        .WithQueryStringParameters(expectation.HttpRequest.Parameters.ToArray())
                        .WithBody(expectation.HttpRequest.Body)
                        .WithSecure(isSecure),
                    unlimitedTimes ? Times.Unlimited() : Times.Exactly(expectation.Times.Count))
                .RespondAsync(httpResponse
                    .WithStatusCode(expectation.HttpResponse.StatusCode)
                    .WithHeaders(expectation.HttpResponse.Headers.ToArray())
                    .WithBody(expectation.HttpResponse.Body ?? Contents.EmptyText())
                    .WithDelay(GetTimeSpanDelay(expectation.HttpResponse.Delay)));
        }

        private static async Task SetupMockServerRequestAsync(
            MockServerClient mockServerClient,
            Expectation expectation,
            HttpRequest httpRequest,
            HttpResponseTemplate httpResponseTemplate)
        {
            var isSecure = expectation.HttpRequest.IsSecure.HasValue && expectation.HttpRequest.IsSecure.Value;
            var unlimitedTimes = expectation.Times == null || expectation.Times.IsUnlimited;

            await mockServerClient
                .When(httpRequest
                        .WithMethod(expectation.HttpRequest.Method)
                        .WithPath(expectation.HttpRequest.Path)
                        .WithQueryStringParameters(expectation.HttpRequest.Parameters.ToArray())
                        .WithBody(expectation.HttpRequest.Body)
                        .WithSecure(isSecure),
                    unlimitedTimes ? Times.Unlimited() : Times.Exactly(expectation.Times.Count))
                .RespondAsync(httpResponseTemplate
                    .WithTemplate(expectation.HttpResponseTemplate.Template, expectation.HttpResponseTemplate.TemplateType)
                    .WithDelay(GetTimeSpanDelay(expectation.HttpResponseTemplate.Delay)));
        }

        private static TimeSpan GetTimeSpanDelay(Delay delay)
        {
            if (delay == null)
            {
                return TimeSpan.FromSeconds(0);
            }

            switch (delay.TimeUnit)
            {
                case "MILLISECONDS":
                    return TimeSpan.FromMilliseconds(delay.Value);
                case "SECONDS":
                    return TimeSpan.FromSeconds(delay.Value);
                case "MINUTES":
                    return TimeSpan.FromMinutes(delay.Value);
                case "HOURS":
                    return TimeSpan.FromHours(delay.Value);
                case "DAYS":
                    return TimeSpan.FromDays(delay.Value);
                case "TICKS":
                    return TimeSpan.FromTicks(delay.Value);
                default:
                    return TimeSpan.FromSeconds(0);
            }
        }
    }
}