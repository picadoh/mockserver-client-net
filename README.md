C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/)

### Requirements

- [.NET Core 2.0](https://www.microsoft.com/net/download/core)
- [Docker](https://www.docker.com/)

### Build

    dotnet build Src/

### Running Tests

    ./integration-tests-with-docker.sh

### Start Mock Server using Docker

    docker run -it -d --name mockserver -p 1080:1080 jamesdbloom/mockserver

### Using the NuGet Package

Get the latest version from https://www.nuget.org/packages/MockServerClientCSharp/ and refer to [this page](Docs/Samples.md) for usage examples.
