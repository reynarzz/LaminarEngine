cmake_minimum_required(VERSION 3.10)
# Force clang-cl on Windows BEFORE project()
if (WIN32)
    set(CMAKE_C_COMPILER clang CACHE STRING "" FORCE)
    set(CMAKE_CXX_COMPILER clang++ CACHE STRING "" FORCE)
endif()

set(IMGUI_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../imgui-1.91.6)

project(cimnodes LANGUAGES C CXX)


add_library(cimnodes
    cimnodes.cpp
    cimnodes.h
)

# Define IMNODES_NAMESPACE as imnodes
target_compile_definitions(cimnodes PRIVATE IMNODES_NAMESPACE=imnodes
)

target_include_directories(cimnodes PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}/cimgui
    ${IMGUI_DIR}/
)

set_target_properties(cimnodes PROPERTIES CXX_STANDARD 17 CXX_STANDARD_REQUIRED ON)
