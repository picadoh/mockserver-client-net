[![Build Status](https://travis-ci.org/picadoh/mockserver-client-net.svg?branch=master)](https://travis-ci.org/picadoh/mockserver-client-net) [![NuGet version](https://badge.fury.io/nu/MockServerClientNet.svg)](https://badge.fury.io/nu/MockServerClientNet)

C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/) targetting `.netstandard2.0`.

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

The below command will run the integration tests against a local running instance of Mock-Server.

    dotnet test

Use the following environment variables to change the target instance of the tests:

- MOCKSERVER\_TEST\_HOST (Defaults to `localhost`)
- MOCKSERVER\_TEST\_PORT (Defaults to `1080`)

### How to Contribute

Please take a look at [CONTRIBUTING](CONTRIBUTING.md) for details.
