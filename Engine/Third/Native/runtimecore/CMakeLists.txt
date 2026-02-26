cmake_minimum_required(VERSION 3.20)
project(RuntimeCore LANGUAGES C CXX)

set(CMAKE_BUILD_TYPE Release CACHE STRING "Build type" FORCE)
set(OUTPUT_DIR ${PROJECT_SOURCE_DIR}../../bin)

add_library(RuntimeCore SHARED
    RuntimeCore.cpp
    PlayStationGamepad.cpp
)

set_target_properties(${PROJECT_NAME} PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${OUTPUT_DIR}/win"   # Windows DLL
    LIBRARY_OUTPUT_DIRECTORY "${OUTPUT_DIR}/osx"   # .so / .dylib
    ARCHIVE_OUTPUT_DIRECTORY "${OUTPUT_DIR}/lib"   # .lib / .a
)


target_compile_definitions(RuntimeCore PUBLIC 
RUNTIMECORE_EXPORTS
GAMEPAD_CORE_HAS_AUDIO=1
)

target_compile_definitions(RuntimeCore PRIVATE GLFW_STATIC)

set_target_properties(RuntimeCore PROPERTIES
    CXX_STANDARD 20
    CXX_STANDARD_REQUIRED ON
)

# Force GLFW static build
set(BUILD_SHARED_LIBS OFF CACHE BOOL "" FORCE)
set(GLFW_BUILD_EXAMPLES OFF CACHE BOOL "" FORCE)
set(GLFW_BUILD_TESTS OFF CACHE BOOL "" FORCE)
set(GLFW_BUILD_DOCS OFF CACHE BOOL "" FORCE)

add_subdirectory("../glfw" glfw_build)
add_subdirectory("../dualsense" dualsense_build)
add_subdirectory("../miniaudio" miniaudio_build)

target_include_directories(RuntimeCore PRIVATE 
"../glfw/include"
"../dualsense/Source/Public"
"../dualsense/Source"
"../miniaudio/miniaudio"

)

target_link_libraries(RuntimeCore PRIVATE 
glfw 
)

target_sources(RuntimeCore PRIVATE $<TARGET_OBJECTS:GamepadCore>)
target_sources(RuntimeCore PRIVATE $<TARGET_OBJECTS:miniaudio>)

if(APPLE)
    target_link_libraries(RuntimeCore PRIVATE "-framework Cocoa" "-framework OpenGL" "-framework IOKit" "-framework CoreVideo")
elseif(WIN32)
    target_link_libraries(RuntimeCore PRIVATE 
    SetupAPI
    hid)
elseif(UNIX AND NOT APPLE)
    find_package(X11 REQUIRED)
    target_include_directories(RuntimeCore PRIVATE ${X11_INCLUDE_DIR})
    target_link_libraries(RuntimeCore PRIVATE ${X11_LIBRARIES} dl pthread)
endif()

if(MSVC)
    target_compile_options(RuntimeCore PRIVATE $<$<CONFIG:Release>:/O2 /DNDEBUG>)
    target_link_options(RuntimeCore PRIVATE "/WHOLEARCHIVE:glfw3") #make sure whole contents are added to the library.
else()
    target_compile_options(RuntimeCore PRIVATE $<$<CONFIG:Release>:-O3 -DNDEBUG>)
endif()