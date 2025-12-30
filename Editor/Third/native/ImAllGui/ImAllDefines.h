
#if defined(_WIN32)
#define IMALLGUI extern "C" __declspec(dllexport)
#else
#define IMALLGUI
#endif