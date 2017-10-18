C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/)

### Requirements

- [.NET Core 2.0](https://www.microsoft.com/net/download/core)
- [Docker](https://www.docker.com/)

### Build

    dotnet build

### Running Tests

    docker run -d --name mockserver -p 1080:1080 jamesdbloom/mockserver
    dotnet test

### Using the NuGet Package

Get the latest version from https://www.nuget.org/packages/MockServerClientCSharp/ and refer to [this page](docs/Samples.md) for usage examples.
