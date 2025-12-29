cmake_minimum_required(VERSION 3.15)

project(imnodes LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# detect if standalone
if (NOT DEFINED IMNODES_STANDALONE_PROJECT)
    if (CMAKE_CURRENT_SOURCE_DIR STREQUAL CMAKE_SOURCE_DIR)
        set(IMNODES_STANDALONE_PROJECT ON)
    else()
        set(IMNODES_STANDALONE_PROJECT OFF)
    endif()
endif()

option(IMNODES_EXAMPLES "Build examples" ${IMNODES_STANDALONE_PROJECT})

# imnodes OBJECT library (compile only imnodes sources)
add_library(imnodes_obj OBJECT
    imnodes.cpp
    imnodes.h
    imnodes_internal.h
)

# Include directories for imnodes_obj
target_include_directories(imnodes_obj PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>   # imnodes headers
    $<BUILD_INTERFACE:${CIMGUI_DIR}>                 # cimgui headers (for #include <cimgui.h>)
    $<BUILD_INTERFACE:${IMGUI_DIR}>                  # imgui headers
    $<INSTALL_INTERFACE:include>
)

# Compile definitions
target_compile_definitions(imnodes_obj PRIVATE
    IMNODES_NAMESPACE=imnodes
)

# Optional examples (only if standalone)
if(IMNODES_EXAMPLES AND IMNODES_STANDALONE_PROJECT)
    find_package(SDL2 CONFIG REQUIRED)

    # Wrapper static library to link examples
    add_library(imnodes_tmp STATIC $<TARGET_OBJECTS:imnodes_obj>)
    target_include_directories(imnodes_tmp PUBLIC
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    )

    # Example: colornode
    add_executable(colornode
        ${CMAKE_CURRENT_SOURCE_DIR}/example/main.cpp
        ${CMAKE_CURRENT_SOURCE_DIR}/example/color_node_editor.cpp
    )
    target_link_libraries(colornode imnodes_tmp SDL2::SDL2)

    # Add other examples similarly if needed
endif()
