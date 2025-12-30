
#include <imgui.h>
#ifdef IMGUI_ENABLE_FREETYPE
#include <imgui/misc/freetype/imgui_freetype.h>
#endif
#include <imgui/imgui_internal.h>
#include <cimgui.h>

#include <imnodes/imnodes.h>
#include <cimnodes.h>

#include "ImAllGui.h"
#include <imguizmo/ImGuizmo.h>

CIMGUI_API void InitImAllGui()
{
	ImGui::CreateContext();
	imnodes::CreateContext();
	imnodes::StyleColorsDark();
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