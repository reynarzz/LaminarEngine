cmake_minimum_required(VERSION 3.10)

# Force clang-cl BEFORE project()
if (WIN32)
    set(CMAKE_C_COMPILER clang CACHE STRING "" FORCE)
    set(CMAKE_CXX_COMPILER clang++ CACHE STRING "" FORCE)
endif()

if(APPLE)
    set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64" CACHE STRING "" FORCE)
endif()

project(ImAllGui LANGUAGES C CXX)

set(IMALLGUI_ROOT_DIR "${CMAKE_CURRENT_SOURCE_DIR}")
set(CIMGUI_DIR "${IMALLGUI_ROOT_DIR}/cimgui")
set(IMGUI_DIR "${CIMGUI_DIR}/imgui")
set(OUTPUT_DIR ${IMALLGUI_ROOT_DIR}../../../ImAllGui/bins)

add_subdirectory(cimgui)
add_subdirectory(cimnodes)
add_subdirectory(cimguizmo)

add_library(ImAllGui SHARED
ImAllGui.cpp
ImAllGui.h)

set_target_properties(ImAllGui PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${OUTPUT_DIR}/win"   # Windows DLL
    LIBRARY_OUTPUT_DIRECTORY "${OUTPUT_DIR}/osx"   # .so / .dylib
    ARCHIVE_OUTPUT_DIRECTORY "${OUTPUT_DIR}/lib"   # .lib / .a
)

set_target_properties(ImAllGui PROPERTIES
    C_VISIBILITY_PRESET default
    CXX_VISIBILITY_PRESET default
    VISIBILITY_INLINES_HIDDEN OFF
)
target_sources(ImAllGui PRIVATE
    $<TARGET_OBJECTS:cimgui_obj>
    $<TARGET_OBJECTS:imnodes_obj>
    $<TARGET_OBJECTS:cimnodes_obj>
    $<TARGET_OBJECTS:cimguizmo_obj>
)

# Export include dirs from subtargets
target_include_directories(ImAllGui PUBLIC
    # Top-level build dir
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    # Subprojects
    $<BUILD_INTERFACE:${IMALLGUI_ROOT_DIR}/cimgui>
    $<BUILD_INTERFACE:${IMALLGUI_ROOT_DIR}/cimnodes>
    $<BUILD_INTERFACE:${IMALLGUI_ROOT_DIR}/cimguizmo>
    $<BUILD_INTERFACE:${IMGUI_DIR}>
    # Install interface
    $<INSTALL_INTERFACE:include>
)

