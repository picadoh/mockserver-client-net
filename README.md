[![Build Status](https://api.travis-ci.org/calrom-jtejero/dotnet-mockserver-client.svg?branch=master)](https://api.travis-ci.org/calrom-jtejero/dotnet-mockserver-client.svg?branch=master) [![NuGet version](https://img.shields.io/nuget/v/DotNetMockServerClient.svg)](https://www.nuget.org/packages/DotNetMockServerClient/) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/calrom-jtejero/dotnet-mockserver-client/blob/master/LICENSE.md)

C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/) targeting `dotnet 5`.

This client, available at [MockServerClientNet Repository](https://github.com/calrom-jtejero/dotnet-mockserver-client), is written in C# and based on the original Java client Fluent API, available at [Mock-Server Repository](https://github.com/mock-server/mockserver). Thanks to its authors for their contributions to open-source. I am **not** an author of the mentioned Java client and this client is **not** part of the [official Mock-Server clients](https://www.mock-server.com/mock_server/mockserver_clients.html).

## For users

### Usage

Refer to [Usage Samples](docs/Samples.md) for examples on how to use the Fluent API.

To start an instance of Mock-Server using Docker:

    docker run -d --rm --name mockserver -p 1080:1080 mockserver/mockserver

For more details on using the Docker image, check the [official Mock-Server documentation](https://www.mock-server.com/where/docker.html).

## For contributors

### Build

    dotnet build

### Testing

The below command will run the integration tests against a local running instance of Mock-Server.

    dotnet test

Use the following environment variables to change the target instance of the tests:

- MOCKSERVER\_TEST\_HOST (Defaults to `localhost`)
- MOCKSERVER\_TEST\_PORT (Defaults to `1080`)

### How to Contribute

Please take a look at [CONTRIBUTING](CONTRIBUTING.md) for details.
