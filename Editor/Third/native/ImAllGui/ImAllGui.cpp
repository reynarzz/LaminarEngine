
#include <imgui.h>
#ifdef IMGUI_ENABLE_FREETYPE
#include <imgui/misc/freetype/imgui_freetype.h>
#endif
#include <imgui/imgui_internal.h>
#include <cimgui.h>

#include <imnodes/imnodes.h>
#include <cimnodes.h>

#include "ImAllGui.h"

CIMGUI_API void InitImAllGui()
{
	ImGui::CreateContext(nullptr);
	imnodes::CreateContext();
	imnodes::EditorContextSet(imnodes::EditorContextCreate());
}
