cmake_minimum_required(VERSION 3.10)

#add_subdirectory(imguizmo)

add_library(cimguizmo_obj OBJECT
    imguizmo/ImGuizmo.cpp
    imguizmo/ImGuizmo.h
    imguizmo/ImCurveEdit.cpp
    imguizmo/ImCurveEdit.h
    cimguizmo.cpp
    cimguizmo.h
)

target_include_directories(cimguizmo_obj PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${IMGUI_DIR}
)
target_include_directories(cimguizmo_obj PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    $<INSTALL_INTERFACE:include>
    $<BUILD_INTERFACE:${CIMGUI_DIR}>
    $<BUILD_INTERFACE:${IMGUI_DIR}>    
)
set_target_properties(cimguizmo_obj PROPERTIES
    CXX_STANDARD 17
    CXX_STANDARD_REQUIRED ON
)
