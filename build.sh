#!/bin/bash

#set -x

DIRNAME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
PROJLOC='/Xamarin.Android.PdfBox/Xamarin.Android.PdfBox.csproj'
CSPROJ="${DIRNAME}${PROJLOC}"

if [ -z "$1" ]
  then
    msbuild $CSPROJ /t:Pack /restore /p:Configuration=Release /p:PackageOutputPath=$DIRNAME
  else
    msbuild $CSPROJ /t:Pack /restore /p:Configuration=Release /p:PackageOutputPath=$DIRNAME /p:PackageVersion=$1
fi

#set +x