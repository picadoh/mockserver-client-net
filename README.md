[![Build Status](https://travis-ci.org/picadoh/mockserver-client-net.svg?branch=master)](https://travis-ci.org/picadoh/mockserver-client-net) [![NuGet version](https://badge.fury.io/nu/MockServerClientNet.svg)](https://badge.fury.io/nu/MockServerClientNet)

C# Fluent API for interacting with [Mock-Server](http://www.mock-server.com/) targetting .netstandard2.0.

This client is written in C# and based on the original Java client Fluent API, available at [Mock-Server](http://www.mock-server.com/). Thanks to its authors for their contributions to open-source. I am not an author of the mentioned Java client.

# Build

    dotnet build

# Testing

The below command will run the integration tests against a local running instance of Mock-Server.

    dotnet test

Use the following enviornment variables to change the target instance:

- MOCKSERVER\_TEST\_HOST
- MOCKSERVER\_TEST\_PORT

# Using the NuGet Package

Get the latest version from https://www.nuget.org/packages/MockServerClientNet/ and refer to [this page](docs/Samples.md) for usage examples.

# How to Contribute

Please take a look at [CONTRIBUTING](CONTRIBUTING.md) for instructions.

