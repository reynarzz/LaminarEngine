cd gamecooker

dotnet build -p:ForceConsoleOutput=true -c Release

cd ..
cd platforms/android


dotnet build -p:BuildAndroid=true -c Release


#dotnet build -t:Run -f net8.0-ios -r ios-arm64 /p:_DeviceName=:v2:device
