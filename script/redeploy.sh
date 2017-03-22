
PROJECT="OsmiumMine.Core"
DEPLOY_PROJECT="OsmiumMine.Core/OsmiumMine.Core.Server"

echo "$PROJECT redeploy script"

echo "Killing existing $PROJECT servers"
pkill -fe $PROJECT.dll

echo "Building $PROJECT..."
bash script/build.sh

pushd .
cd $DEPLOY_PROJECT/bin/Release/netcoreapp1.1/publish

echo "Starting server with nohup"
nohup dotnet $PROJECT.dll &

echo "Returning"
popd