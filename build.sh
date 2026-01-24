#!/usr/bin/env bash
set -e

PLATFORM="$1"
BUILD_TYPE="${2:-Release}"
PLATFORM_FOLDER_NAME=""

# normalize
PLATFORM="$(echo "$PLATFORM" | tr '[:upper:]' '[:lower:]')"
BUILD_TYPE="$(echo "$BUILD_TYPE" | tr '[:lower:]' '[:upper:]')"

if [ -z "$PLATFORM" ]; then
    echo "Error: platform not specified"
    echo "Usage: ./build.sh {windows|macos|linux|android|all} {Debug|Release}"
    exit 1
fi

dotnet_cmd() {

    dotnet publish \
        -c "$BUILD_TYPE" \
        -p:PublishTrimmed=true \
        "$@"
}

build_gamecooker() 
{
    cd gamecooker
    dotnet_cmd -p:ForceConsoleOutput=true
    cd ..
}

build_android() 
{
    cd platforms/android
    dotnet_cmd -p:BuildAndroid=true -p:PublishPlatform=android
    cd ../../_Ship/Android

    if [ "$BUILD_TYPE" = "RELEASE" ]; then
        adb install -r com.reynarzz.gfs-Signed.apk
    else
        adb install -r com.reynarzz.gfs-Debug.apk
    fi

    adb shell am start -n com.reynarzz.gfs/crc64faceced24a29f4d5.MainActivity

    cd ../../
}

build_desktop() 
{
    cd platforms/Desktop
    dotnet_cmd -p:ForceConsoleOutput=true -p:PublishPlatform=$PLATFORM_FOLDER_NAME 
    cd ../../
}

case "$PLATFORM" in
    windows)
	PLATFORM_FOLDER_NAME="win32"
        build_gamecooker
        build_desktop
        ;;
    macos)
        PLATFORM_FOLDER_NAME="macOS"
        build_gamecooker
        build_desktop
        ;;
    linux)
        PLATFORM_FOLDER_NAME="linux"
        build_gamecooker
        build_desktop
        ;;
    android)
        PLATFORM_FOLDER_NAME="android"
        build_gamecooker
        build_android
        ;;
    *)
        echo "Unknown platform: $PLATFORM"
        echo "Usage: ./build.sh {windows|macos|linux|android|all} {Debug|Release}"
        exit 1
        ;;
esac