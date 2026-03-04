cmake_minimum_required(VERSION 3.20)

# Force clang-cl BEFORE project()

set(PROJECT_NAME EditorNatives)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

if (WIN32)
    set(CMAKE_C_COMPILER clang CACHE STRING "" FORCE)
    set(CMAKE_CXX_COMPILER clang++ CACHE STRING "" FORCE)


 project(${PROJECT_NAME} LANGUAGES C CXX)

endif()

if(APPLE)
    set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64" CACHE STRING "" FORCE)
project(${PROJECT_NAME} LANGUAGES C CXX OBJCXX)
else()


endif()



set(EDITOR_NATIVES_ROOT_DIR "${CMAKE_CURRENT_SOURCE_DIR}")
set(CIMGUI_DIR "${EDITOR_NATIVES_ROOT_DIR}/cimgui")
set(IMGUI_DIR "${CIMGUI_DIR}/imgui")
set(OUTPUT_DIR ${EDITOR_NATIVES_ROOT_DIR}../../EditorNatives/bin)
cmake_path(SET ENGINE_NATIVES
    NORMALIZE
    "${PROJECT_SOURCE_DIR}/../../../Engine/Third/Native"
)
add_subdirectory(cimgui)
add_subdirectory(cimnodes)
add_subdirectory(cimguizmo)
add_subdirectory(nativefiledialog)

add_library(${PROJECT_NAME} SHARED
EditorNatives.cpp
EditorNatives.h
nativemenu/cnativemenu.cpp
avirimageresize/cavirresize.cpp
stbimagewrite/cstbimagewrite.cpp
nativefiledialog/src/cfiledialog.cpp
$<$<BOOL:WIN32>:nativemenu/NativeMenu.cpp>
$<$<BOOL:APPLE>:nativemenu/NativeMenu.mm>
)

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
    $<TARGET_OBJECTS:cimgui_obj>
    $<TARGET_OBJECTS:imnodes_obj>
    $<TARGET_OBJECTS:cimnodes_obj>
    $<TARGET_OBJECTS:cimguizmo_obj>
)

if(MSVC)
foreach(config DEBUG RELEASE RELWITHDEBINFO MINSIZEREL)
    set_target_properties(${PROJECT_NAME} PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY_${config} "${OUTPUT_DIR}/win"
        LIBRARY_OUTPUT_DIRECTORY_${config} "${OUTPUT_DIR}/win"
        ARCHIVE_OUTPUT_DIRECTORY_${config} "${OUTPUT_DIR}/lib"
    )
endforeach()
endif()

message(STATUS "GLFW include path: ${ENGINE_NATIVES}/glfw/include")

# old
include_directories(${ENGINE_NATIVES}/glfw/include)

# Export include dirs from subtargets
target_include_directories(${PROJECT_NAME} PUBLIC
    # Top-level build dir
    ${CMAKE_SOURCE_DIR}
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    # Subprojects
    $<BUILD_INTERFACE:${EDITOR_NATIVES_ROOT_DIR}/cimgui>
    $<BUILD_INTERFACE:${EDITOR_NATIVES_ROOT_DIR}/cimnodes>
    $<BUILD_INTERFACE:${EDITOR_NATIVES_ROOT_DIR}/cimguizmo>
    $<BUILD_INTERFACE:${IMGUI_DIR}>
    # Install interface
    $<INSTALL_INTERFACE:include>
)

if (WIN32)
target_link_libraries(${PROJECT_NAME} PUBLIC 

    ${ENGINE_NATIVES}/bin/lib/glfw3dll.lib
    nfd
)

    target_compile_definitions(${PROJECT_NAME} PRIVATE WINDOWS)
elseif (APPLE)
    find_library(COCOA_LIBRARY Cocoa REQUIRED)
    find_library(IOKIT_LIBRARY IOKit REQUIRED)
    find_library(COREVIDEO_LIBRARY CoreVideo REQUIRED)

    target_link_libraries(${PROJECT_NAME} PRIVATE
        ${COCOA_LIBRARY}
        ${IOKIT_LIBRARY}
        ${COREVIDEO_LIBRARY}
    ${ENGINE_NATIVES}/bin/osx/libglfw.3.4.dylib
    nfd

        
    )

    target_compile_definitions(${PROJECT_NAME} PRIVATE MACOS)
elseif (UNIX)
    target_compile_definitions(${PROJECT_NAME} PRIVATE LINUX)
endif()