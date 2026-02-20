cmake_minimum_required(VERSION 3.20)

project(GamepadCore LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(SOURCE_DIR "${CMAKE_CURRENT_SOURCE_DIR}/Source")
# Enable Unicode for Windows
if(WIN32)
    add_compile_definitions(UNICODE _UNICODE)
endif()

file(GLOB_RECURSE GAMEPADCORE_HEADERS
    "${SOURCE_DIR}/Public/*.h"
    "${SOURCE_DIR}/Public/*.hpp"
    "${SOURCE_DIR}/Private/*.h"
    "${SOURCE_DIR}/Private/*.hpp"
)

file(GLOB_RECURSE GAMEPADCORE_SOURCES
    "${SOURCE_DIR}/Private/*.cpp"
    "${SOURCE_DIR}/Private/*.cxx"
)

add_library(GamepadCore OBJECT
    ${GAMEPADCORE_HEADERS}
    ${GAMEPADCORE_SOURCES}
    ${SOURCE_DIR}/Public/GImplementations/Utils/GamepadAudio.h
)

target_include_directories(GamepadCore
    PUBLIC "${SOURCE_DIR}/Public"
    PRIVATE "${SOURCE_DIR}/Private"
)

target_compile_features(GamepadCore PUBLIC cxx_std_20)

set_target_properties(GamepadCore PROPERTIES
    CXX_EXTENSIONS OFF
    POSITION_INDEPENDENT_CODE ON
)

set(AUDIO_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/Libs/miniaudio")

if(EXISTS "${AUDIO_MODULE_PATH}/miniaudio.h")
    target_sources(GamepadCore PRIVATE "${AUDIO_MODULE_PATH}/miniaudio_impl.cpp")
    target_include_directories(GamepadCore PUBLIC "${AUDIO_MODULE_PATH}")
    target_compile_definitions(GamepadCore PUBLIC GAMEPAD_CORE_HAS_AUDIO=1)
    message(STATUS "GamepadCore: Audio module ENABLED")
else()
    target_compile_definitions(GamepadCore PUBLIC GAMEPAD_CORE_HAS_AUDIO=0)
    message(STATUS "GamepadCore: Audio module not found")
    message(STATUS "  To enable: git submodule update --init Libs/miniaudio")
endif()

# ============================================
# Optional Tests Module
# ============================================
if(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/Tests/Integration/CMakeLists.txt")
    # Global definition for test-specific code blocks
    add_definitions(-DBUILD_GAMEPAD_CORE_TESTS)

    option(BUILD_TESTS "Build integration tests" OFF)
    option(AUTOMATED_TESTS "Enable automated tests (timeout for CI)" OFF)

    if(BUILD_TESTS)
        enable_testing()
        add_subdirectory(Tests)
    endif()
else()
    message(STATUS "Tests submodule not initialized - skipping tests")
endif()