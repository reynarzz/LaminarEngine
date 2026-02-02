
#include "EditorNativesDefines.h"

EDITOR_NATIVES_API void InitImAllGui();


EDITOR_NATIVES_API void SetWindowHitTestHole(ImGuiWindow* window, const ImVec2& pos, const ImVec2& size);
EDITOR_NATIVES_API void SetCurrentWindowHitTestHole(float posX, float posY, float sizeX, float sizeY);
EDITOR_NATIVES_API void ImGuizmo_SetCurrentWindowDrawList();
EDITOR_NATIVES_API void imgui_NewFrame();

EDITOR_NATIVES_API void InitGLFWImguiInternal(void* windowPtr);
EDITOR_NATIVES_API void BeginGLFWImguiInternal();
EDITOR_NATIVES_API void EndGLFWImguiInternal();
