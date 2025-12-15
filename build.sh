cd gamecooker

dotnet build -p:ForceConsoleOutput=true -c Release

dotnet build -p:ForceConsoleOutput=true -c Debug

cd ..
cd platforms/android


dotnet build -p:BuildAndroid=true -c Release