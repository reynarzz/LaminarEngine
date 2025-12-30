#pragma once
// imallgui_export.h
#include "ImAllDefines.h"

#ifdef __cplusplus
extern "C" {
#endif
	typedef void (*LogCallback)(const char* message);
	IMALLGUI void RegisterLogCallback(LogCallback callback);

#ifdef __cplusplus
}
#endif
extern LogCallback _Log;
