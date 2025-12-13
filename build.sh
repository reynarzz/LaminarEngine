cd gamecooker

dotnet build -p:ForceConsoleOutput=true -c Release

cd ..
cd platforms/android


dotnet build -p:BuildAndroid=true -c Release