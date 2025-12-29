cmake_minimum_required(VERSION 3.15)
cmake_policy(SET CMP0091 NEW)
project(cimgui LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# detect if imgui tables exist
if (EXISTS ${CMAKE_CURRENT_SOURCE_DIR}/imgui/imgui_tables.cpp)
    set(TABLES_SOURCE "imgui/imgui_tables.cpp")
else()
    set(TABLES_SOURCE "")
endif()

# collect sources
file(GLOB IMGUI_SOURCES
    cimgui.cpp
    imgui/imgui.cpp
    imgui/imgui_draw.cpp
    imgui/imgui_demo.cpp
    imgui/imgui_widgets.cpp
    ${TABLES_SOURCE}
)

# freetype support
set(IMGUI_FREETYPE "no" CACHE STRING "Build with freetype library")
if(IMGUI_FREETYPE)
    message(STATUS "Building with freetype")
    find_package(freetype REQUIRED PATHS ${FREETYPE_PATH})
    list(APPEND IMGUI_SOURCES imgui/misc/freetype/imgui_freetype.cpp)
endif()

# ---------------------------
# OBJECT library for embedding
# ---------------------------
add_library(cimgui_obj OBJECT ${IMGUI_SOURCES})

# compile definitions
if(IMGUI_FREETYPE)
    target_compile_definitions(cimgui_obj PUBLIC IMGUI_ENABLE_FREETYPE IMGUI_ENABLE_STB_TRUETYPE)
endif()

if(IMGUI_WCHAR32)
    target_compile_definitions(cimgui_obj PUBLIC IMGUI_USE_WCHAR32)
endif()

# always disable obsolete functions
target_compile_definitions(cimgui_obj PUBLIC IMGUI_DISABLE_OBSOLETE_FUNCTIONS=1)

# Windows export macro
if(WIN32)
    target_compile_definitions(cimgui_obj PUBLIC
        IMGUI_IMPL_API="__declspec(dllexport)"
    )
else()
    target_compile_definitions(cimgui_obj PUBLIC
        IMGUI_IMPL_API=""
    )
endif()
# includes
target_include_directories(cimgui_obj PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}/imgui
)

# set no prefix (optional)
set_target_properties(cimgui_obj PROPERTIES PREFIX "")

target_include_directories(cimgui_obj PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/imgui>
    $<INSTALL_INTERFACE:include>   # for installable targets
)
# ---------------------------
# optional standalone STATIC library (for backward compatibility)
# ---------------------------
option(BUILD_CIMGUI_STATIC "Build standalone static cimgui library" OFF)
if(BUILD_CIMGUI_STATIC)
    add_library(cimgui_static STATIC $<TARGET_OBJECTS:cimgui_obj>)
    target_include_directories(cimgui_static PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${CMAKE_CURRENT_SOURCE_DIR}/imgui
    )
    if(IMGUI_FREETYPE)
        target_link_libraries(cimgui_static PUBLIC freetype)
    endif()
    if(WIN32)
        target_link_libraries(cimgui_static PUBLIC imm32)
    endif()
endif()

# ---------------------------
# optional test
# ---------------------------
set(CIMGUI_TEST "no" CACHE STRING "Enable compilation of a test unit based on imgui null")
if(CIMGUI_TEST)
    add_subdirectory(test)
endif()
