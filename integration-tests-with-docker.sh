#!/bin/sh
docker run -it -d --name mockserver_it_tests -p 1080:1080 jamesdbloom/mockserver
dotnet test Src/MockServerClientCSharp.Tests
docker kill mockserver_it_tests
docker rm mockserver_it_tests
