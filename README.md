C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/)

### Requirements

- [.NET Core 2.0](https://www.microsoft.com/net/download/core)
- [Docker](https://www.docker.com/)

### Build

    dotnet build

### Testing

The below command will run the integration tests against a local running instance of Mock-Server.

    dotnet test

Use the following enviornment variables to change the target instance:

- MOCKSERVER\_TEST\_HOST
- MOCKSERVER\_TEST\_PORT

### Using the NuGet Package

Get the latest version from https://www.nuget.org/packages/MockServerClientCSharp/ and refer to [this page](docs/Samples.md) for usage examples.
