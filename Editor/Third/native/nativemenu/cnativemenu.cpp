#include <EditorNativesDefines.h>
#include <functional>
#include "NativeMenu.h"
#include "Types.h"

using CallbackFn = void(*)();

std::function<void()> GetCallback(void* callbackPtr)
{
	CallbackFn cb = reinterpret_cast<CallbackFn>(callbackPtr);
	return [cb]() { cb(); };
}

EDITOR_NATIVES_API void PushMenu(const char* path, void* callback)
{
	NativeMenu::Add(path, GetCallback(callback));
}

EDITOR_NATIVES_API void PushMenuToggle(const char* path, void* callback, u8 toggle)
{
	NativeMenu::Add(path, GetCallback(callback), toggle);
}

EDITOR_NATIVES_API void PushMenuShortcut(const char* path, void* callback, u8 toggle, const char* shortcut)
{
	NativeMenu::Add(path, GetCallback(callback), toggle, shortcut);
}

EDITOR_NATIVES_API void Toggle(const char* path, u8 checked)
{
	NativeMenu::Toggle(path, checked);
}

EDITOR_NATIVES_API void Enable(const char* path, u8 enabled)
{
	NativeMenu::Enable(path, enabled);
}

EDITOR_NATIVES_API u8 IsEnabled(const char* path)
{
	return NativeMenu::IsEnabled(path);
}

EDITOR_NATIVES_API u8 IsChecked(const char* path)
{
	return NativeMenu::IsChecked(path);
}

EDITOR_NATIVES_API void Separator(const char* path)
{
	NativeMenu::Separator(path);
}

EDITOR_NATIVES_API void SeparatorAt(const char* path, int index)
{
	NativeMenu::Separator(path, index);
}

EDITOR_NATIVES_API void RemoveSeparators(const char* path)
{
	NativeMenu::RemoveSeparators(path);
}

EDITOR_NATIVES_API void RemoveSeparator(const char* path)
{
	NativeMenu::RemoveSeparator(path);
}
EDITOR_NATIVES_API void RemoveSeparatorAt(const char* path, int index)
{
	NativeMenu::RemoveSeparator(path, index);
}

EDITOR_NATIVES_API void DestroyMenuAtPath(const char* path)
{
	NativeMenu::DestroyMenu(path);
}
