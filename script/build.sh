#!/usr/bin/env bash

PROJECT_NAME="OsmiumMine.Core"
SOLUTION="OsmiumMine.Core/$PROJECT_NAME.sln"
DEPLOY_PROJECT="OsmiumMine.Core/OsmiumMine.Core.Server"

ARTIFACTS_FOLDER="./artifacts"

if [ -d $ARTIFACTS_FOLDER ]; then  
  rm -R $ARTIFACTS_FOLDER
fi

# dotnet CLI info

dotnet --version

echo "Restoring packages for solution..."
# Restore packages
dotnet restore $SOLUTION

echo "Building $PROJECT_NAME..."

# Build
dotnet build -c Release $DEPLOY_PROJECT

echo "Running tests..."

echo "Publishing $PROJECT_NAME..."

# Publish
dotnet publish -c Release $DEPLOY_PROJECT
