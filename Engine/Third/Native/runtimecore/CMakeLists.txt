cmake_minimum_required(VERSION 3.20)
project(RuntimeCore LANGUAGES C CXX)

set(CMAKE_BUILD_TYPE Release CACHE STRING "Build type" FORCE)
set(OUTPUT_DIR ${PROJECT_SOURCE_DIR}../../bin)
add_library(RuntimeCore SHARED
    RuntimeCore.cpp
)

set_target_properties(${PROJECT_NAME} PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${OUTPUT_DIR}/win"   # Windows DLL
    LIBRARY_OUTPUT_DIRECTORY "${OUTPUT_DIR}/osx"   # .so / .dylib
    ARCHIVE_OUTPUT_DIRECTORY "${OUTPUT_DIR}/lib"   # .lib / .a
)


target_compile_definitions(RuntimeCore PRIVATE RUNTIMECORE_EXPORTS)

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

target_include_directories(RuntimeCore PRIVATE 
"../glfw/include"
"../dualsense/Source/Public"
)

target_link_libraries(RuntimeCore PRIVATE 
glfw )

target_sources(RuntimeCore PRIVATE $<TARGET_OBJECTS:GamepadCore>)

if(APPLE)
    target_link_libraries(RuntimeCore PRIVATE "-framework Cocoa" "-framework OpenGL" "-framework IOKit" "-framework CoreVideo")
elseif(UNIX AND NOT APPLE)
    find_package(X11 REQUIRED)
    target_include_directories(RuntimeCore PRIVATE ${X11_INCLUDE_DIR})
    target_link_libraries(RuntimeCore PRIVATE ${X11_LIBRARIES} dl pthread)
endif()

# Compiler optimizations
if(MSVC)
    target_compile_options(RuntimeCore PRIVATE $<$<CONFIG:Release>:/O2 /DNDEBUG>)
else()
    target_compile_options(RuntimeCore PRIVATE $<$<CONFIG:Release>:-O3 -DNDEBUG>)
endif()