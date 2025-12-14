cd gamecooker

dotnet build -p:ForceConsoleOutput=true -c Release

cd ..
cd platforms/android


dotnet build -p:BuildAndroid=true -c Release

#cd ..
#cd platforms/Ios

#dotnet build -p:BuildIOS=true

#install to device:
#xcrun devicectl device install app --device 0000 Entry_Ios.app

