cmake_minimum_required(VERSION 3.10)


add_subdirectory(imnodes)

add_library(cimnodes_obj OBJECT
    cimnodes.cpp
    cimnodes.h
)

target_compile_definitions(cimnodes_obj PRIVATE
    IMNODES_NAMESPACE=imnodes
)

target_include_directories(cimnodes_obj PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${IMGUI_DIR}
)
target_include_directories(cimnodes_obj PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    $<INSTALL_INTERFACE:include>
    $<BUILD_INTERFACE:${CIMGUI_DIR}>
)
set_target_properties(cimnodes_obj PROPERTIES
    CXX_STANDARD 17
    CXX_STANDARD_REQUIRED ON
)
