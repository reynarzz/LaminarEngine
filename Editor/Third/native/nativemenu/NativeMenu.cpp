#if _WIN32
#include "NativeMenu.h"
#define GLFW_EXPOSE_NATIVE_WIN32
#include <GLFW/glfw3.h>
#include <GLFW/glfw3native.h>
#include <windows.h>

#include <unordered_map>
#include <vector>
#include <string>
#include <functional>

namespace
{
    HWND   g_Hwnd    = nullptr;
    HMENU  g_MenuBar = nullptr;
    WNDPROC g_PrevProc = nullptr;
    UINT   g_NextId  = 30000;

    // Menus & items
    std::unordered_map<std::string, HMENU> g_MenuByPath;  // "File" -> HMENU
    std::unordered_map<std::string, UINT>  g_IdByPath;    // path -> ID

    struct NMItemInfo
    {
        std::function<void()> cb;
        bool   isToggle = false;
        bool   checked  = false;
        HMENU  parent   = nullptr; // parent menu handle
        UINT   id       = 0;
    };
    std::unordered_map<UINT, NMItemInfo> g_InfoById;      // ID -> info

    std::unordered_map<std::string, bool> g_IsToggleByPath;

    std::vector<std::string> Split(const std::string& s)
    {
        std::vector<std::string> out;
        size_t i = 0, j;
        while ((j = s.find('/', i)) != std::string::npos)
        {
            if (j > i) out.push_back(s.substr(i, j - i));
            i = j + 1;
        }
        if (i < s.size()) out.push_back(s.substr(i));
        return out;
    }

    std::string ParentPath(const std::string& path)
    {
        auto pos = path.find_last_of('/');
        return (pos == std::string::npos) ? std::string() : path.substr(0, pos);
    }

    std::string LastSeg(const std::string& path)
    {
        auto pos = path.find_last_of('/');
        return (pos == std::string::npos) ? path : path.substr(pos + 1);
    }

    HMENU EnsureMenu(const std::string& path)
    {
        if (auto it = g_MenuByPath.find(path); it != g_MenuByPath.end())
            return it->second;

        const std::string parent = ParentPath(path);
        const std::string label  = LastSeg(path);

        HMENU parentMenu = parent.empty() ? g_MenuBar : EnsureMenu(parent);
        HMENU sub = CreatePopupMenu();
        AppendMenuA(parentMenu, MF_POPUP, (UINT_PTR)sub, label.c_str());
        g_MenuByPath[path] = sub;
        return sub;
    }

    int FindSubmenuIndex(HMENU parent, HMENU target)
    {
        int cnt = GetMenuItemCount(parent);
        for (int i = 0; i < cnt; ++i)
        {
            HMENU sm = GetSubMenu(parent, i);
            if (sm == target) return i;
        }
        return -1;
    }

    void EraseDescendants(const std::string& prefix)
    {
        const std::string pref = prefix + "/";

        for (auto it = g_MenuByPath.begin(); it != g_MenuByPath.end(); )
        {
            if (it->first == prefix || it->first.rfind(pref, 0) == 0)
                it = g_MenuByPath.erase(it);
            else
                ++it;
        }
        for (auto it = g_IdByPath.begin(); it != g_IdByPath.end(); )
        {
            if (it->first == prefix || it->first.rfind(pref, 0) == 0)
            {
                g_InfoById.erase(it->second);
                it = g_IdByPath.erase(it);
            }
            else
                ++it;
        }
        for (auto it = g_IsToggleByPath.begin(); it != g_IsToggleByPath.end(); )
        {
            if (it->first == prefix || it->first.rfind(pref, 0) == 0)
                it = g_IsToggleByPath.erase(it);
            else
                ++it;
        }
    }

    static LRESULT CALLBACK HookWndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == WM_COMMAND)
        {
            UINT id = LOWORD(wParam);
            auto it = g_InfoById.find(id);
            if (it != g_InfoById.end())
            {
                NMItemInfo& info = it->second;

                if (info.isToggle && info.parent)
                {
                    info.checked = !info.checked;
                    CheckMenuItem(info.parent, id, MF_BYCOMMAND | (info.checked ? MF_CHECKED : MF_UNCHECKED));
                    DrawMenuBar(g_Hwnd);
                }

                if (info.cb) info.cb();
                return 0;
            }
        }
        return CallWindowProc(g_PrevProc, hWnd, msg, wParam, lParam);
    }
} // anonymous namespace

static std::string NM_FormatLabelWithShortcut(const std::string& label,
                                              const std::string& shortcut)
{
    if (shortcut.empty()) return label;
    return label + "\t" + shortcut; // text after tab shows as shortcut hint
}


// ======================= Public NativeMenu methods =======================
void NativeMenu::Init(GLFWwindow* window)
{
    g_Hwnd = glfwGetWin32Window(window);
    g_MenuBar = CreateMenu();
    SetMenu(g_Hwnd, g_MenuBar);
    g_PrevProc = (WNDPROC)SetWindowLongPtr(g_Hwnd, GWLP_WNDPROC, (LONG_PTR)HookWndProc);
}

void NativeMenu::Add(const std::string& path, std::function<void()> callback)
{
    Add(path, callback, false);
}

void NativeMenu::Add(const std::string& path, std::function<void()> callback, bool toggle)
{
    NativeMenu::Add(path, std::move(callback), toggle, std::string{});
}

// New overload with shortcut
void NativeMenu::Add(const std::string& path,
                     std::function<void()> callback,
                     bool toggle,
                     const std::string& shortcut)
{
    auto parts = Split(path);
    if (parts.empty()) return;

    // Build submenu chain
    std::string prefix;
    for (size_t i = 0; i + 1 < parts.size(); ++i)
    {
        prefix = prefix.empty() ? parts[i] : (prefix + "/" + parts[i]);
        EnsureMenu(prefix);
    }

    HMENU parent = (parts.size() == 1) ? g_MenuBar : g_MenuByPath[prefix];

    UINT id = g_NextId++;
    std::string label      = parts.back();
    std::string finalLabel = NM_FormatLabelWithShortcut(label, shortcut);

    UINT flags = MF_STRING;
    // (optional) you can set MF_UNCHECKED here for toggles; the check mark is controlled via CheckMenuItem anyway
    AppendMenuA(parent, flags, id, finalLabel.c_str());

    // Record info
    NMItemInfo info;
    info.cb       = std::move(callback);
    info.isToggle = toggle;
    info.checked  = false;
    info.parent   = parent;
    info.id       = id;

    g_IdByPath[path]       = id;
    g_InfoById[id]         = std::move(info);
    g_IsToggleByPath[path] = toggle;

    DrawMenuBar(g_Hwnd);
}

void NativeMenu::Toggle(const std::string& path, bool checked)
{
    auto it = g_IdByPath.find(path);
    if (it == g_IdByPath.end()) return;

    UINT id = it->second;
    auto infoIt = g_InfoById.find(id);
    if (infoIt == g_InfoById.end()) return;

    NMItemInfo& info = infoIt->second;
    if (!info.isToggle || !info.parent) return;

    info.checked = checked;
    CheckMenuItem(info.parent, id, MF_BYCOMMAND | (checked ? MF_CHECKED : MF_UNCHECKED));
    DrawMenuBar(g_Hwnd);
}

void NativeMenu::Enable(const std::string& path, bool enabled)
{
    // 1) Leaf item (by command ID)
    if (auto idIt = g_IdByPath.find(path); idIt != g_IdByPath.end())
    {
        UINT id = idIt->second;
        auto infoIt = g_InfoById.find(id);
        if (infoIt != g_InfoById.end() && infoIt->second.parent)
        {
            UINT flags = MF_BYCOMMAND | (enabled ? MF_ENABLED : (MF_DISABLED | MF_GRAYED));
            EnableMenuItem(infoIt->second.parent, id, flags);
            // Checkmark (if toggle) remains as-is; we do NOT change it here.
            DrawMenuBar(g_Hwnd);
            return;
        }
    }

    // 2) Submenu holder (enable/disable by position under its parent)
    std::string parentPath = ParentPath(path);
    std::string label = LastSeg(path);
    HMENU parent = parentPath.empty() ? g_MenuBar : g_MenuByPath[parentPath];
    if (!parent) return;

    int count = GetMenuItemCount(parent);
    for (int i = 0; i < count; ++i)
    {
        char buf[256] = {};
        MENUITEMINFOA mii = {};
        mii.cbSize = sizeof(mii);
        mii.fMask = MIIM_STRING;
        mii.dwTypeData = buf;
        mii.cch = static_cast<UINT>(sizeof(buf) - 1);
        if (!GetMenuItemInfoA(parent, i, TRUE, &mii))
            continue;

        if (label == std::string(buf))
        {
            UINT flags = MF_BYPOSITION | (enabled ? MF_ENABLED : (MF_DISABLED | MF_GRAYED));
            EnableMenuItem(parent, i, flags);
            DrawMenuBar(g_Hwnd);
            return;
        }
    }
}

bool NativeMenu::IsEnabled(const std::string& path)
{
    // Leaf item by command ID
    if (auto it = g_IdByPath.find(path); it != g_IdByPath.end())
    {
        UINT id = it->second;
        auto infoIt = g_InfoById.find(id);
        if (infoIt == g_InfoById.end() || !infoIt->second.parent) return false;

        HMENU parent = infoIt->second.parent;
        UINT state = GetMenuState(parent, id, MF_BYCOMMAND);
        // Disabled if MF_GRAYED or MF_DISABLED is set
        bool enabled = ((state & (MF_GRAYED | MF_DISABLED)) == 0);
        return enabled;
    }

    // Submenu holder by position under its parent
    std::string parentPath = ParentPath(path);
    std::string label      = LastSeg(path);
    HMENU parent = parentPath.empty() ? g_MenuBar : g_MenuByPath[parentPath];
    if (!parent) return false;

    int count = GetMenuItemCount(parent);
    for (int i = 0; i < count; ++i)
    {
        char buf[256] = {};
        MENUITEMINFOA mii = {};
        mii.cbSize     = sizeof(mii);
        mii.fMask      = MIIM_STRING;
        mii.dwTypeData = buf;
        mii.cch        = static_cast<UINT>(sizeof(buf) - 1);
        if (!GetMenuItemInfoA(parent, i, TRUE, &mii))
            continue;

        if (label == std::string(buf))
        {
            UINT state = GetMenuState(parent, i, MF_BYPOSITION);
            bool enabled = ((state & (MF_GRAYED | MF_DISABLED)) == 0);
            return enabled;
        }
    }
    return false;
}

bool NativeMenu::IsChecked(const std::string& path)
{
    auto it = g_IdByPath.find(path);
    if (it == g_IdByPath.end()) return false;

    UINT id = it->second;
    auto infoIt = g_InfoById.find(id);
    if (infoIt == g_InfoById.end()) return false;

    const NMItemInfo& info = infoIt->second;
    if (!info.isToggle) return false;

    return info.checked;
}



void NativeMenu::DestroyMenu(const std::string& path)
{
    // Leaf item?
    if (auto it = g_IdByPath.find(path); it != g_IdByPath.end())
    {
        UINT id = it->second;
        auto infoIt = g_InfoById.find(id);
        HMENU parent = (infoIt != g_InfoById.end()) ? infoIt->second.parent : nullptr;
        if (parent)
        {
            RemoveMenu(parent, id, MF_BYCOMMAND);
        }
        g_InfoById.erase(id);
        g_IdByPath.erase(it);
        g_IsToggleByPath.erase(path);
        DrawMenuBar(g_Hwnd);
        return;
    }

    // Submenu?
    auto mit = g_MenuByPath.find(path);
    if (mit != g_MenuByPath.end())
    {
        HMENU submenu = mit->second;
        std::string parentPath = ParentPath(path);
        HMENU parent = parentPath.empty() ? g_MenuBar : g_MenuByPath[parentPath];

        if (parent)
        {
            int idx = FindSubmenuIndex(parent, submenu);
            if (idx >= 0)
            {
                EraseDescendants(path);
                RemoveMenu(parent, idx, MF_BYPOSITION);
                ::DestroyMenu(submenu); // qualify to avoid collision with method name
                DrawMenuBar(g_Hwnd);
            }
        }
        return;
    }
}

void NativeMenu::Separator(const std::string& path)
{
    HMENU menu = EnsureMenu(path);
    if (!menu) return;

    AppendMenuA(menu, MF_SEPARATOR, 0, nullptr);
    DrawMenuBar(g_Hwnd);
}

void NativeMenu::Separator(const std::string& path, int index)
{
    HMENU menu = EnsureMenu(path);
    if (!menu) return;

    int count = GetMenuItemCount(menu);
    if (index < 0 || index >= count)
    {
        AppendMenuA(menu, MF_SEPARATOR, 0, nullptr);
    }
    else
    {
        MENUITEMINFOA mii = {};
        mii.cbSize = sizeof(mii);
        mii.fMask  = MIIM_FTYPE;
        mii.fType  = MFT_SEPARATOR;
        InsertMenuItemA(menu, static_cast<UINT>(index), TRUE, &mii);
    }
    DrawMenuBar(g_Hwnd);
}

void NativeMenu::RemoveSeparators(const std::string& path)
{
    HMENU menu = EnsureMenu(path);
    if (!menu) return;

    int count = GetMenuItemCount(menu);
    for (int i = count - 1; i >= 0; --i) // reverse to keep indices valid
    {
        MENUITEMINFOA mii = {};
        mii.cbSize = sizeof(mii);
        mii.fMask = MIIM_FTYPE;
        if (GetMenuItemInfoA(menu, i, TRUE, &mii))
        {
            if (mii.fType & MFT_SEPARATOR)
                RemoveMenu(menu, i, MF_BYPOSITION);
        }
    }
    DrawMenuBar(g_Hwnd);
}

void NativeMenu::RemoveSeparator(const std::string& path)
{
    // If path points to an item, remove any separator immediately before it
    auto idIt = g_IdByPath.find(path);
    if (idIt == g_IdByPath.end()) return;

    UINT id = idIt->second;
    auto infoIt = g_InfoById.find(id);
    if (infoIt == g_InfoById.end() || !infoIt->second.parent) return;

    HMENU parent = infoIt->second.parent;
    int idx = -1;
    int count = GetMenuItemCount(parent);
    for (int i = 0; i < count; ++i)
    {
        if (GetMenuItemID(parent, i) == id) { idx = i; break; }
    }
    if (idx > 0)
    {
        MENUITEMINFOA mii = {};
        mii.cbSize = sizeof(mii);
        mii.fMask = MIIM_FTYPE;
        if (GetMenuItemInfoA(parent, idx - 1, TRUE, &mii))
        {
            if (mii.fType & MFT_SEPARATOR)
                RemoveMenu(parent, idx - 1, MF_BYPOSITION);
        }
    }
    DrawMenuBar(g_Hwnd);
}

void NativeMenu::RemoveSeparator(const std::string& path, int index)
{
    HMENU menu = EnsureMenu(path);
    if (!menu) return;

    int count = GetMenuItemCount(menu);
    if (index < 0 || index >= count) return;

    MENUITEMINFOA mii = {};
    mii.cbSize = sizeof(mii);
    mii.fMask = MIIM_FTYPE;
    if (GetMenuItemInfoA(menu, index, TRUE, &mii))
    {
        if (mii.fType & MFT_SEPARATOR)
            RemoveMenu(menu, index, MF_BYPOSITION);
    }
    DrawMenuBar(g_Hwnd);
}




void NativeMenu::Shutdown(GLFWwindow* /*window*/)
{
    if (g_Hwnd && g_PrevProc)
    {
        SetWindowLongPtr(g_Hwnd, GWLP_WNDPROC, (LONG_PTR)g_PrevProc);
        g_PrevProc = nullptr;
    }
    if (g_Hwnd)
    {
        SetMenu(g_Hwnd, nullptr);
        g_Hwnd = nullptr;
    }
    if (g_MenuBar)
    {
        ::DestroyMenu(g_MenuBar);
        g_MenuBar = nullptr;
    }

    g_MenuByPath.clear();
    g_IdByPath.clear();
    g_InfoById.clear();
    g_IsToggleByPath.clear();
    g_NextId = 30000;
}
#endif