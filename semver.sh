#!/usr/bin/env bash

dotnet tool install -g dotnet-setversion > /dev/null 2>&1
setversion $1 $2 > /dev/null 2>&1

