cd platforms/Ios

dotnet build -p:BuildIOS=true

#install to device:
xcrun devicectl device install app --device 0000 bin/Debug/net8.0-ios/ios-arm64/Entry_IOS.app

