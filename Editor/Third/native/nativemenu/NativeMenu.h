#pragma once
#include <functional>
#include <string>

// Forward declaration to avoid leaking GLFW into dependents
struct GLFWwindow;

class NativeMenu
{
public:
    // Initialize after creating the GLFW window.
    static void Init(GLFWwindow* window);

    // Add a menu item at "A/B/C" -> creates submenus A->B and an item "C".
    static void Add(const std::string& path, std::function<void()> callback);

    // Add a menu item, optionally checkable (toggle=true). Click auto-toggles, then calls callback.
    static void Add(const std::string& path, std::function<void()> callback, bool toggle);

    static void Add(const std::string& path, std::function<void()> callback, bool toggle, const std::string& shortcut);
    
    // Programmatically set a checkable item's check state (ignored if item isn't checkable).
    static void Toggle(const std::string& path, bool checked);

    // Enable/disable a menu item or submenu holder (grays it out when disabled).
    static void Enable(const std::string& path, bool enabled);

    static bool IsEnabled(const std::string& path);
    static bool IsChecked(const std::string& path);

    // Add a separator at the end of the given menu.
    static void Separator(const std::string& path);

    // Insert a separator at a specific position (0-based) within the given menu.
    // If index < 0 or index >= count, it appends at the end.
    static void Separator(const std::string& path, int index);

    // Remove all separators inside the given menu
    static void RemoveSeparators(const std::string& path);

    // Remove separator(s) around a specific item path, or at a specific index
    static void RemoveSeparator(const std::string& path);
    static void RemoveSeparator(const std::string& path, int index);

    // Remove a single item or an entire submenu tree at "path".
    static void DestroyMenu(const std::string& path);

    // Optional cleanup before destroying the window/process.
    static void Shutdown(GLFWwindow* window);
};
