#pragma once

#if defined(_WIN32) || defined(_WIN64)
	#ifdef RUNTIMECORE_EXPORTS
		#define RUNTIMECORE_API extern "C" __declspec(dllexport)
	#else
		#define RUNTIMECORE_API extern "C" __declspec(dllimport)
	#endif
#elif defined(__APPLE__) || defined(__linux__)
	#ifdef RUNTIMECORE_EXPORTS
		#define RUNTIMECORE_API extern "C" __attribute__((visibility("default")))
	#else
		#define RUNTIMECORE_API extern "C"
	#endif
#else
	#define RUNTIMECORE_API extern "C"
#endif