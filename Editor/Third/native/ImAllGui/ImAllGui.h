
#include "ImAllDefines.h"

IMALLGUI void InitImAllGui();


IMALLGUI void SetWindowHitTestHole(ImGuiWindow* window, const ImVec2& pos, const ImVec2& size);
IMALLGUI void SetCurrentWindowHitTestHole(float posX, float posY, float sizeX, float sizeY);
IMALLGUI void ImGuizmo_SetCurrentWindowDrawList();
IMALLGUI void imgui_NewFrame();

