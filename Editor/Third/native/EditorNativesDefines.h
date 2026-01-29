#pragma once

#if defined(_WIN32)
    #define EDITOR_NATIVES_API extern "C" __declspec(dllexport)
#elif defined(__APPLE__) || defined(__linux__)
    #define EDITOR_NATIVES_API extern "C" __attribute__((visibility("default")))
#else
    #define EDITOR_NATIVES_API extern "C"
#endif