cd platforms/Ios

dotnet build -p:BuildIOS=true

#install to device:
xcrun devicectl device install app --device 0000 Entry_Ios.app

