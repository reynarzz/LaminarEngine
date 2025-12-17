cd gamecooker

dotnet build -p:ForceConsoleOutput=true -c Release

dotnet build -p:ForceConsoleOutput=true -c Debug

cd ..
cd platforms/android


dotnet build -p:BuildAndroid=true -c Release

cd ../../_Ship/Android

adb install -r com.reynarzz.gfs-Signed.apk

adb shell am start -n com.reynarzz.gfs/crc64faceced24a29f4d5.MainActivity
