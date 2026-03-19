#include "../../Types.h"
#include "../../EditorNativesDefines.h"
#include "portable-file-dialogs.h"
#include <string>

EDITOR_NATIVES_API int Open_MessageBox(const char* title, const char* text, int choice, int icon)
{
	pfd::button button = pfd::message(title, text, static_cast<pfd::choice>(choice), static_cast<pfd::icon>(icon)).result();
    return static_cast<int>(button);
}