#pragma once

#if defined(_WIN32)
#define EDITOR_NATIVES_API extern "C" __declspec(dllexport)
#else
#define EDITOR_NATIVES_API extern "C" 
#endif