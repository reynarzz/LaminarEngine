cmake_minimum_required(VERSION 3.10)

# Force clang-cl BEFORE project()
if (WIN32)
    set(CMAKE_C_COMPILER clang CACHE STRING "" FORCE)
    set(CMAKE_CXX_COMPILER clang++ CACHE STRING "" FORCE)
endif()

if(APPLE)
    set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64" CACHE STRING "" FORCE)
endif()

set(PROJECT_NAME EditorNatives)

project(${PROJECT_NAME} LANGUAGES C CXX)

set(EDITOR_NATIVES_ROOT_DIR "${CMAKE_CURRENT_SOURCE_DIR}")
set(CIMGUI_DIR "${EDITOR_NATIVES_ROOT_DIR}/cimgui")
set(IMGUI_DIR "${CIMGUI_DIR}/imgui")
set(OUTPUT_DIR ${EDITOR_NATIVES_ROOT_DIR}../../EditorNatives/bin)
set(ENGINE_NATIVES ${CMAKE_CURRENT_SOURCE_DIR}../../../../Engine/Third/Native)

## Force GLFW to build as OBJECT library
#set(GLFW_LIBRARY_TYPE OBJECT CACHE STRING "" FORCE)
#
## Disable GLFW stuff we don't need
#set(GLFW_BUILD_EXAMPLES OFF CACHE BOOL "" FORCE)
#set(GLFW_BUILD_TESTS OFF CACHE BOOL "" FORCE)
#set(GLFW_BUILD_DOCS OFF CACHE BOOL "" FORCE)
#set(GLFW_INSTALL OFF CACHE BOOL "" FORCE)

#add_subdirectory(glfw)
add_subdirectory(cimgui)
add_subdirectory(cimnodes)
add_subdirectory(cimguizmo)

#target_compile_definitions(glfw PRIVATE _GLFW_BUILD_DLL)

add_library(${PROJECT_NAME} SHARED
EditorNatives.cpp
EditorNatives.h)

set_target_properties(${PROJECT_NAME} PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${OUTPUT_DIR}/win"   # Windows DLL
    LIBRARY_OUTPUT_DIRECTORY "${OUTPUT_DIR}/osx"   # .so / .dylib
    ARCHIVE_OUTPUT_DIRECTORY "${OUTPUT_DIR}/lib"   # .lib / .a
)

set_target_properties(${PROJECT_NAME} PROPERTIES
    C_VISIBILITY_PRESET default
    CXX_VISIBILITY_PRESET default
    VISIBILITY_INLINES_HIDDEN OFF
)
target_sources(${PROJECT_NAME} PRIVATE
    #$<TARGET_OBJECTS:glfw>
    $<TARGET_OBJECTS:cimgui_obj>
    $<TARGET_OBJECTS:imnodes_obj>
    $<TARGET_OBJECTS:cimnodes_obj>
    $<TARGET_OBJECTS:cimguizmo_obj>
)

# Export include dirs from subtargets
target_include_directories(${PROJECT_NAME} PUBLIC
    # Top-level build dir
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    # Subprojects
    $<BUILD_INTERFACE:${EDITOR_NATIVES_ROOT_DIR}/cimgui>
    $<BUILD_INTERFACE:${EDITOR_NATIVES_ROOT_DIR}/cimnodes>
    $<BUILD_INTERFACE:${EDITOR_NATIVES_ROOT_DIR}/cimguizmo>
    ${ENGINE_NATIVES}/glfw/include
    $<BUILD_INTERFACE:${IMGUI_DIR}>
    # Install interface
    $<INSTALL_INTERFACE:include>
)

target_link_libraries(${PROJECT_NAME} PUBLIC 

    ${ENGINE_NATIVES}/bin/lib/glfw3dll.lib
)


