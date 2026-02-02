#pragma once
// imallgui_export.h
#include "EditorNativesDefines.h"

#ifdef __cplusplus
extern "C" {
#endif
	typedef void (*LogCallback)(const char* message);
	EDITOR_NATIVES_API void RegisterLogCallback(LogCallback callback);

#ifdef __cplusplus
}
#endif
extern LogCallback _Log;
