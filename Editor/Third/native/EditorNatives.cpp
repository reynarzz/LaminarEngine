
#include <imgui.h>
#ifdef IMGUI_ENABLE_FREETYPE
#include <imgui/misc/freetype/imgui_freetype.h>
#endif
#include <imgui/imgui_internal.h>
#include <cimgui.h>

#include <imnodes/imnodes.h>
#include <cimnodes.h>

#include "EditorNatives.h"
#include <imguizmo/ImGuizmo.h>
#include <stdio.h>
#include "Logger.h"

#include <GLFW/glfw3.h>
#include <backends/imgui_impl_glfw.h>
#include <backends/imgui_impl_opengl3.h>
#include "nativemenu/NativeMenu.h"

LogCallback _Log = nullptr;

CIMGUI_API void InitImAllGui()
{
	ImGui::CreateContext();
	imnodes::CreateContext();
	imnodes::StyleColorsDark();

	_Log("Editor Natives Init");
}



CIMGUI_API void SetCurrentWindowHitTestHole(float posX, float posY, float sizeX, float sizeY)
{
	return ImGui::SetWindowHitTestHole(ImGui::GetCurrentWindow(), { posX, posY }, { sizeX, sizeY });
}

CIMGUI_API void ImGuizmo_SetCurrentWindowDrawList()
{
	ImGuizmo::SetDrawlist(ImGui::GetWindowDrawList());
}

CIMGUI_API void imgui_NewFrame()
{
	ImGui::NewFrame();
}

EDITOR_NATIVES_API void RegisterLogCallback(LogCallback callback)
{
	_Log = callback;
}


EDITOR_NATIVES_API void InitGLFWImguiInternal(void* windowPtr)
{
	GLFWwindow* win = static_cast<GLFWwindow*>(windowPtr);

	ImGui_ImplGlfw_InitForOpenGL(win, true);
	ImGui_ImplOpenGL3_Init();
	NativeMenu::Init(win);
}


EDITOR_NATIVES_API void BeginGLFWImguiInternal() 
{
	ImGui_ImplOpenGL3_NewFrame();
	ImGui_ImplGlfw_NewFrame();
	ImGui::NewFrame();
}

EDITOR_NATIVES_API void EndGLFWImguiInternal()
{
	ImGui::Render();
	ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());

	if (ImGui::GetIO().ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
	{
		GLFWwindow* backup_current_context = glfwGetCurrentContext();
		ImGui::UpdatePlatformWindows();
		ImGui::RenderPlatformWindowsDefault();
		glfwMakeContextCurrent(backup_current_context);
	}
}


void NativeLogSomething()
{
	if (_Log)
		_Log("Hello from native code");
}