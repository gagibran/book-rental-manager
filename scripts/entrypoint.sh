#!/bin/sh

if [ "$WITH_INITIAL_DATA" = "TRUE" ]; then
    exec dotnet BookRentalManager.Api.dll --with-initial-data=true
else
    exec dotnet BookRentalManager.Api.dll
fi
