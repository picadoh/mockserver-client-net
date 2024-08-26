[![MockServer .NET Client](https://github.com/picadoh/mockserver-client-net/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/picadoh/mockserver-client-net/actions/workflows/dotnet-build.yml) ![NuGet Version](https://img.shields.io/nuget/v/MockServerClientNet) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/picadoh/mockserver-client-net/blob/master/LICENSE.md)

Unofficial C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/) targeting `.NET 8`.

This client, available at [MockServerClientNet Repository](https://github.com/picadoh/mockserver-client-net), is written in C# and based on the original Java client Fluent API, available at [Mock-Server Repository](https://github.com/mock-server/mockserver). Thanks to its authors for their contributions to open-source. I am **not** an author of the mentioned Java client and this client is **not** part of the [official Mock-Server clients](https://www.mock-server.com/mock_server/mockserver_clients.html).

## For users

### NuGet Package

Get the latest version from [NuGet Gallery](https://www.nuget.org/packages/MockServerClientNet/) or run the following command:

    dotnet add package MockServerClientNet

### Usage

Refer to [Usage Samples](docs/Samples.md) for examples on how to use the Fluent API.

To start an instance of Mock-Server using Docker:

    docker run -d --rm --name mockserver -p 1080:1080 mockserver/mockserver

For more details on using the Docker image, check the [official Mock-Server documentation](https://www.mock-server.com/where/docker.html).

## For contributors

### Build

    dotnet build

### Testing

The below command will run the integration tests against 
a [TestContainers](https://dotnet.testcontainers.org/) Mock-Server instance.

    dotnet test

### How to Contribute

Please take a look at [CONTRIBUTING](CONTRIBUTING.md) for details.
