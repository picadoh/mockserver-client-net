#!/usr/bin/env bash

echo "::set-output name=new_release_version::$1"
sed -i "s#<PackageVersion>.*#<PackageVersion>$1</PackageVersion>#" $2
sed -i "s#<Version>.*#<Version>$1</Version>#" $2

