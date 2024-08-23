#!/usr/bin/env bash

echo "::set-output name=new_release_version::$1"
dotnet tool install -g dotnet-setversion > /dev/null 2>&1
setversion $1 $2 > /dev/null 2>&1

