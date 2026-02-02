#ifdef MACOS

#import <Cocoa/Cocoa.h>
#import <dispatch/dispatch.h>
#define GLFW_EXPOSE_NATIVE_COCOA
#include <GLFW/glfw3.h>
#include <GLFW/glfw3native.h>

#include "NativeMenu.h"
#include <unordered_map>
#include <vector>
#include <string>
#include <functional>

using Callback = std::function<void()>;

// Bridge so Obj-C never references anon-namespace names directly
static void NM_InvokeCallback(int id);

// Obj-C target (global scope to satisfy Objective-C rules)
@interface NMFileScopedMenuTarget : NSObject
@end

@implementation NMFileScopedMenuTarget
- (void)nm_onMenu:(id)sender
{
    NSNumber* key = [sender representedObject];
    if (!key) return;
    NM_InvokeCallback(key.intValue);
}
@end

// File-scoped C++ state & helpers
namespace
{
    NSWindow* g_NsWindow = nil;
    int g_NextId = 30000;

    // Menus & items
    std::unordered_map<std::string, NSMenu*>     g_MenuByPath;   // "File" -> NSMenu*
    std::unordered_map<std::string, NSMenuItem*> g_ItemByPath;   // "File/Open" -> NSMenuItem*
    std::unordered_map<std::string, int>         g_IdByPath;     // path -> ID
    std::unordered_map<std::string, bool>        g_IsToggleByPath;

    struct NMItemInfo
    {
        Callback     cb;
        bool         isToggle = false;
        bool         checked  = false;
        NSMenuItem*  item     = nil; // backref
    };
    std::unordered_map<int, NMItemInfo> g_InfoById;              // ID -> info

    NMFileScopedMenuTarget* g_Target = nil;

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


        // Turn off AppKit's auto-enable so our manual [setEnabled:] sticks
    static inline void NM_DisableAutoEnable(NSMenu* menu)
    {
        if (!menu) return;
        if (menu.autoenablesItems)
            [menu setAutoenablesItems:NO];
    }

  // Parse "Shift+Cmd+S"  set keyEquivalent and modifier mask on the NSMenuItem.
// Supports: Cmd/Command, Ctrl/Control, Alt/Option, Shift
// Keys: single visible char (letter/digit/punct) or special: Esc/Escape, Tab, Return/Enter, Space.
static void NM_ApplyShortcut(NSMenuItem* item, const std::string& shortcut)
{
    if (!item || shortcut.empty()) return;

    NSString* s = [NSString stringWithUTF8String:shortcut.c_str()];
    NSArray<NSString*>* raw = [s componentsSeparatedByString:@"+"];

    NSEventModifierFlags mods = 0;
    NSString* keyToken = nil;

    // Parse tokens: accumulate modifiers, last non-mod token becomes the key
    for (NSString* part in raw)
    {
        NSString* p = [part stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
        if (p.length == 0) continue;

        NSString* lower = p.lowercaseString;

        if ([lower isEqualToString:@"cmd"] || [lower isEqualToString:@"command"] || [p isEqualToString:@"⌘"])
        { mods |= NSEventModifierFlagCommand; continue; }

        if ([lower isEqualToString:@"ctrl"] || [lower isEqualToString:@"control"] || [p isEqualToString:@"⌃"])
        { mods |= NSEventModifierFlagControl; continue; }

        if ([lower isEqualToString:@"alt"] || [lower isEqualToString:@"option"] || [p isEqualToString:@"⌥"])
        { mods |= NSEventModifierFlagOption; continue; }

        if ([lower isEqualToString:@"shift"] || [p isEqualToString:@"⇧"])
        { mods |= NSEventModifierFlagShift; continue; }

        // Not a modifier: treat as key token
        keyToken = p;
    }

    if (!keyToken) return;

    // Map special names; otherwise expect a single visible character
    NSString* keyEq = nil;
    NSString* lower = keyToken.lowercaseString;
    if      ([lower isEqualToString:@"esc"] || [lower isEqualToString:@"escape"])          keyEq = [NSString stringWithCharacters:(unichar[]){0x1B} length:1];
    else if ([lower isEqualToString:@"tab"])                                               keyEq = @"\t";
    else if ([lower isEqualToString:@"return"] || [lower isEqualToString:@"enter"])        keyEq = @"\r";
    else if ([lower isEqualToString:@"space"] || [lower isEqualToString:@"spacebar"])      keyEq = @" ";
    else if (keyToken.length == 1)                                                         keyEq = [keyToken lowercaseString];
    else                                                                                   return; // Unsupported multi-char key (e.g., "F5")

    item.keyEquivalentModifierMask = mods;
    item.keyEquivalent = keyEq;
}


   NSMenu* EnsureMenu(const std::string& path)
   {
    if (auto it = g_MenuByPath.find(path); it != g_MenuByPath.end())
        return it->second;

    if (!NSApp) [NSApplication sharedApplication];

    // Ensure a main menubar exists
    if (!NSApp.mainMenu)
    {
        NSMenu* menubar = [[NSMenu alloc] init];
        [NSApp setMainMenu:menubar];

        // Add minimal App menu with Quit for sanity/user expectations
        NSMenuItem* appItem = [[NSMenuItem alloc] init];
        [menubar addItem:appItem];
        NSMenu* appMenu = [[NSMenu alloc] initWithTitle:@""];
        NSMenuItem* quitItem = [[NSMenuItem alloc] initWithTitle:@"Quit"
                                                          action:@selector(terminate:)
                                                   keyEquivalent:@"q"];
        [appMenu addItem:quitItem];
        [appItem setSubmenu:appMenu];

        // Do not auto-enable on the menubar either
        NM_DisableAutoEnable(menubar);
    }

    std::string parent = ParentPath(path);
    std::string label  = LastSeg(path);

    if (parent.empty())
    {
        // Create a new top level menu
        NSMenuItem* top = [[NSMenuItem alloc] init];
        [top setTitle:[NSString stringWithUTF8String:label.c_str()]];
        NSMenu* sub = [[NSMenu alloc] initWithTitle:[NSString stringWithUTF8String:label.c_str()]];
        [top setSubmenu:sub];
        [NSApp.mainMenu addItem:top];

        // Critical: stop AppKit from re-enabling our items
        NM_DisableAutoEnable(sub);

        g_MenuByPath[path] = sub;
        return sub;
    }
    else
    {
        NSMenu* p = EnsureMenu(parent);

        // Try to find an existing submenu under parent
        NSString* t = [NSString stringWithUTF8String:label.c_str()];
        for (NSMenuItem* mi in p.itemArray)
        {
            if ([[mi title] isEqualToString:t] && mi.submenu)
            {
                // Also make sure adopted submenus don't auto-enable
                NM_DisableAutoEnable(mi.submenu);

                g_MenuByPath[path] = mi.submenu;
                return mi.submenu;
            }
        }

        // Create new submenu under parent
        NSMenuItem* container = [[NSMenuItem alloc] initWithTitle:t action:nil keyEquivalent:@""];
        NSMenu* sub = [[NSMenu alloc] initWithTitle:t];
        [container setSubmenu:sub];
        [p addItem:container];

        // Critical here too
        NM_DisableAutoEnable(sub);

        g_MenuByPath[path] = sub;
        return sub;
    }
}


    // Remove subtree entries for prefix (menus, items, ids, toggles, infos)
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
        for (auto it = g_ItemByPath.begin(); it != g_ItemByPath.end(); )
        {
            if (it->first == prefix || it->first.rfind(pref, 0) == 0)
            {
                if (NSMenuItem* item = it->second)
                {
                    NSNumber* key = [item representedObject];
                    if (key) g_InfoById.erase(key.intValue);
                }
                it = g_ItemByPath.erase(it);
            }
            else ++it;
        }
        for (auto it = g_IdByPath.begin(); it != g_IdByPath.end(); )
        {
            if (it->first == prefix || it->first.rfind(pref, 0) == 0)
            {
                g_InfoById.erase(it->second);
                it = g_IdByPath.erase(it);
            }
            else ++it;
        }
        for (auto it = g_IsToggleByPath.begin(); it != g_IsToggleByPath.end(); )
        {
            if (it->first == prefix || it->first.rfind(pref, 0) == 0)
                it = g_IsToggleByPath.erase(it);
            else
                ++it;
        }
    }

    NSMenuItem* FindChildItemByTitle(NSMenu* parent, const std::string& title)
    {
        if (!parent) return nil;
        NSString* t = [NSString stringWithUTF8String:title.c_str()];
        for (NSMenuItem* mi in parent.itemArray)
        {
            if ([[mi title] isEqualToString:t]) return mi;
        }
        return nil;
    }

// Ensure operations happen on the main thread
static inline void NM_RunOnMain(void (^block)(void))
{
    if ([NSThread isMainThread]) { block(); }
    else { dispatch_async(dispatch_get_main_queue(), block); }
}

// If `path` points to an existing *item*, return its parent menu and index of that item.
// If it points to an existing *menu*, return that menu and index = menu.count (append).
// If it’s new/ambiguous, create/resolve as a *menu* and return that menu (append).
static void NM_ResolveMenuAndIndexForSeparator(const std::string& path,
                                               /*in*/  bool hasIndex, /*in*/ NSInteger reqIndex,
                                               /*out*/ NSMenu** outMenu,
                                               /*out*/ NSInteger* outIndex)
{
    *outMenu = nil;
    *outIndex = NSNotFound;

    // 1- If path names an existing *item*, insert in its parent.
    if (auto itemIt = g_ItemByPath.find(path); itemIt != g_ItemByPath.end())
    {
        NSMenuItem* item = itemIt->second;
        NSMenu* parent = item ? item.menu : nil;
        if (parent)
        {
            NM_DisableAutoEnable(parent);
            NSInteger idx = [parent indexOfItem:item]; // valid index
            *outMenu = parent;
            *outIndex = hasIndex ? reqIndex : idx;     // default: insert *before* that item
            return;
        }
    }

    // 2-If path names an existing *menu*, insert into that menu.
    if (auto menuIt = g_MenuByPath.find(path); menuIt != g_MenuByPath.end())
    {
        NSMenu* m = menuIt->second;
        NM_DisableAutoEnable(m);
        *outMenu = m;
        *outIndex = hasIndex ? reqIndex : (NSInteger)m.numberOfItems; // append
        return;
    }

    // 3- New/ambiguous: if parent has an item with that title, insert in parent.
    std::string parentPath = ParentPath(path);
    std::string label      = LastSeg(path);
    NSMenu* parentMenu = parentPath.empty() ? NSApp.mainMenu : EnsureMenu(parentPath);
    if (parentMenu)
    {
        if (NSMenuItem* existing = FindChildItemByTitle(parentMenu, label))
        {
            NM_DisableAutoEnable(parentMenu);
            NSInteger idx = [parentMenu indexOfItem:existing];
            *outMenu = parentMenu;
            *outIndex = hasIndex ? reqIndex : idx;
            return;
        }
    }

    // 4- Otherwise, treat 'path' as a *menu* and append.
    NSMenu* m = EnsureMenu(path);
    NM_DisableAutoEnable(m);
    *outMenu = m;
    *outIndex = hasIndex ? reqIndex : (NSInteger)m.numberOfItems;
}

// Clamp to a visible position: avoid leading/trailing separators (Cocoa sometimes hides them).
static inline NSInteger NM_ClampSepIndex(NSMenu* menu, NSInteger idx)
{
    if (!menu) return NSNotFound;
    NSInteger count = (NSInteger)menu.numberOfItems;
    if (count <= 0) return 0;              // will show once items exist
    if (idx <= 0)  return 1;               // avoid separator as first item
    if (idx >= count) return count - 1;    // avoid separator as last item
    return idx;
}

// Return the NSMenu to receive a separator for the given path.
// - If path is an existing item -> parent menu.
// - If path is an existing menu -> that menu.
// - Else treat as a menu path and create it.
// Also disables auto-enabling to respect manual enabled states.
static NSMenu* NM_GetTargetMenuForPath(const std::string& path)
{
    // Existing leaf item? -> parent menu
    if (auto it = g_ItemByPath.find(path); it != g_ItemByPath.end())
    {
        NSMenuItem* leaf = it->second;
        NSMenu* pm = leaf ? leaf.menu : nil;
        if (pm)
        {
            NM_DisableAutoEnable(pm);
            return pm;
        }
    }

    // Existing menu?
    if (auto mit = g_MenuByPath.find(path); mit != g_MenuByPath.end())
    {
        NSMenu* m = mit->second;
        NM_DisableAutoEnable(m);
        return m;
    }

    // Ambiguous/new: if the parent has a same-named *item* (not submenu), insert in parent.
    std::string parent = ParentPath(path);
    std::string label  = LastSeg(path);
    NSMenu* parentMenu = parent.empty() ? NSApp.mainMenu : EnsureMenu(parent);
    if (parentMenu)
    {
        NSMenuItem* existing = FindChildItemByTitle(parentMenu, label);
        if (existing && existing.submenu == nil)
        {
            NM_DisableAutoEnable(parentMenu);
            return parentMenu; // insert among siblings of that item
        }
    }

    // Otherwise, ensure/create the menu at 'path'
    NSMenu* m = EnsureMenu(path);
    NM_DisableAutoEnable(m);
    return m;
}


    // Remove the NSMenuItem in the menubar that owns a given submenu
    static void NM_RemoveTopLevelMenuForSubmenu(NSMenu* submenu)
    {
        if (!submenu) return;
        NSMenu* menubar = NSApp.mainMenu;
        if (!menubar) return;

        NSArray<NSMenuItem*>* items = [[menubar itemArray] copy];
        for (NSMenuItem* mi in items)
        {
            if (mi.submenu == submenu)
            {
                [menubar removeItem:mi];
                break;
            }
        }
    }

    // Clear GLFW/Cocoa default menus like “Window”, “Help”, “Services”.
    static void NM_ClearDefaultMenus(BOOL keepAppMenu)
    {
        if (!NSApp) [NSApplication sharedApplication];

        if (!NSApp.mainMenu)
        {
            NSMenu* menubar = [[NSMenu alloc] init];
            [NSApp setMainMenu:menubar];
        }

        if (!keepAppMenu)
        {
            NSMenu* menubar = NSApp.mainMenu;
            if (menubar.numberOfItems > 0)
                [menubar removeItemAtIndex:0];
        }

        NSMenu* windowsMenu = [NSApp windowsMenu];
        if (windowsMenu)
        {
            NM_RemoveTopLevelMenuForSubmenu(windowsMenu);
            [NSApp setWindowsMenu:nil];
        }

        NSMenu* helpMenu = [NSApp helpMenu];
        if (helpMenu)
        {
            NM_RemoveTopLevelMenuForSubmenu(helpMenu);
            [NSApp setHelpMenu:nil];
        }

        NSMenu* servicesMenu = [NSApp servicesMenu];
        if (servicesMenu)
        {
            NM_RemoveTopLevelMenuForSubmenu(servicesMenu);
            // Also erase from the App menu if present
            NSMenu* menubar = NSApp.mainMenu;
            if (menubar.numberOfItems > 0)
            {
                NSMenuItem* appItem = [menubar itemAtIndex:0];
                if (appItem.submenu)
                {
                    NSMenu* appMenu = appItem.submenu;
                    NSArray<NSMenuItem*>* appItems = [[appMenu itemArray] copy];
                    for (NSMenuItem* mi in appItems)
                    {
                        if (mi.submenu == servicesMenu)
                        {
                            [appMenu removeItem:mi];
                            break;
                        }
                    }
                }
            }
            [NSApp setServicesMenu:nil];
        }

        // Remove any empty top-level menus
        NSMenu* menubar = NSApp.mainMenu;
        NSArray<NSMenuItem*>* items = [[menubar itemArray] copy];
        for (NSMenuItem* mi in items)
        {
            if (mi.submenu && mi.submenu.numberOfItems == 0)
                [menubar removeItem:mi];
        }
    }
} // anonymous namespace

// Bridge implementation
static void NM_InvokeCallback(int id)
{
    auto it = g_InfoById.find(id);
    if (it == g_InfoById.end())
        return;

    NMItemInfo& info = it->second;

    if (info.isToggle && info.item)
    {
        info.checked = !info.checked;
        [info.item setState:(info.checked ? NSControlStateValueOn : NSControlStateValueOff)];
    }
    if (info.cb) info.cb();
}

// Public NativeMenu methods
void NativeMenu::Init(GLFWwindow* window)
{
    g_NsWindow = glfwGetCocoaWindow(window);
    if (!NSApp) [NSApplication sharedApplication];
    if (!g_Target) g_Target = [NMFileScopedMenuTarget new];
    [NSApp setActivationPolicy:NSApplicationActivationPolicyRegular];
    [NSApp activateIgnoringOtherApps:YES];

    // Remove default macOS menus (like "Window").
    NM_ClearDefaultMenus(YES);

    // Ensure the menubar itself doesn't auto-enable items
    NM_DisableAutoEnable(NSApp.mainMenu);
}

void NativeMenu::Add(const std::string& path, std::function<void()> callback)
{
    NativeMenu::Add(path, std::move(callback), false);
}

void NativeMenu::Add(const std::string& path, std::function<void()> callback, bool toggle)
{
    if (!g_Target) g_Target = [NMFileScopedMenuTarget new];

    auto parts = Split(path);
    if (parts.empty()) return;

    // Build submenu chain (all but last)
    std::string prefix;
    for (size_t i = 0; i + 1 < parts.size(); ++i)
    {
        prefix = prefix.empty() ? parts[i] : (prefix + "/" + parts[i]);
        (void)EnsureMenu(prefix);
    }

    NSMenu* parent = (parts.size() == 1) ? EnsureMenu(parts[0]) : g_MenuByPath[prefix];

    int id = g_NextId++;
    g_IdByPath[path] = id;
    g_IsToggleByPath[path] = toggle;

    NSMenuItem* item = [[NSMenuItem alloc] initWithTitle:[NSString stringWithUTF8String:parts.back().c_str()]
                                                  action:@selector(nm_onMenu:)
                                           keyEquivalent:@""];
    [item setTarget:g_Target];
    [item setRepresentedObject:@(id)];
    if (toggle)
        [item setState:NSControlStateValueOff];

    [parent addItem:item];
    g_ItemByPath[path] = item;

    NMItemInfo info;
    info.cb       = std::move(callback);
    info.isToggle = toggle;
    info.checked  = false;
    info.item     = item;
    g_InfoById[id] = std::move(info);

    (void)g_NsWindow;
}

void NativeMenu::Toggle(const std::string& path, bool checked)
{
    auto idIt = g_IdByPath.find(path);
    auto itemIt = g_ItemByPath.find(path);
    if (idIt == g_IdByPath.end() || itemIt == g_ItemByPath.end())
        return;

    int id = idIt->second;
    NSMenuItem* item = itemIt->second;
    auto infoIt = g_InfoById.find(id);
    if (infoIt == g_InfoById.end())
        return;

    NMItemInfo& info = infoIt->second;
    if (!info.isToggle)
        return;

    info.checked = checked;
    [item setState:(checked ? NSControlStateValueOn : NSControlStateValueOff)];
}

void NativeMenu::Enable(const std::string& path, bool enabled)
{
    // 1- Leaf item
    if (auto it = g_ItemByPath.find(path); it != g_ItemByPath.end())
    {
        NSMenuItem* item = it->second;
        // Greys out, prevents click; checkmark (state) remains visible if set.
        [item setEnabled:enabled ? YES : NO];
        return;
    }

    // 2- Submenu holder (top-level or nested)
    std::string parentPath = ParentPath(path);
    std::string label = LastSeg(path);
    NSMenu* parentMenu = parentPath.empty() ? NSApp.mainMenu : g_MenuByPath[parentPath];
    if (!parentMenu) return;

    NSMenuItem* holder = FindChildItemByTitle(parentMenu, label);
    if (!holder) return;

    // Disabling the holder greys it out and prevents opening the submenu.
    [holder setEnabled:enabled ? YES : NO];
}


void NativeMenu::DestroyMenu(const std::string& path)
{
    // Remove leaf item
    if (auto it = g_ItemByPath.find(path); it != g_ItemByPath.end())
    {
        NSMenuItem* item = it->second;
        if (NSMenu* parent = item.menu)
        {
            NSNumber* key = [item representedObject];
            if (key) g_InfoById.erase(key.intValue);
            [parent removeItem:item];
        }
        g_ItemByPath.erase(it);

        if (auto idIt = g_IdByPath.find(path); idIt != g_IdByPath.end())
        {
            g_InfoById.erase(idIt->second);
            g_IdByPath.erase(idIt);
        }
        g_IsToggleByPath.erase(path);
        return;
    }

    // Remove submenu tree
    if (auto mit = g_MenuByPath.find(path); mit != g_MenuByPath.end())
    {
        NSMenu* submenu = mit->second;
        std::string parentPath = ParentPath(path);
        NSMenu* parent = parentPath.empty() ? NSApp.mainMenu : g_MenuByPath[parentPath];

        if (parent)
        {
            NSMenuItem* holder = FindChildItemByTitle(parent, LastSeg(path));
            if (holder)
            {
                EraseDescendants(path);
                [parent removeItem:holder];
            }
        }
        return;
    }
}

bool NativeMenu::IsEnabled(const std::string& path)
{
    // Leaf item?
    if (auto it = g_ItemByPath.find(path); it != g_ItemByPath.end())
    {
        NSMenuItem* item = it->second;
        return item ? ([item isEnabled] == YES) : false;
    }

    // Submenu holder?
    std::string parentPath = ParentPath(path);
    std::string label      = LastSeg(path);
    NSMenu* parentMenu = parentPath.empty() ? NSApp.mainMenu : g_MenuByPath[parentPath];
    if (!parentMenu) return false;

    NSMenuItem* holder = FindChildItemByTitle(parentMenu, label);
    if (!holder) return false;

    return [holder isEnabled] == YES;
}

bool NativeMenu::IsChecked(const std::string& path)
{
    // Only for toggle items added with toggle=true
    auto idIt   = g_IdByPath.find(path);
    auto itemIt = g_ItemByPath.find(path);
    if (idIt == g_IdByPath.end() || itemIt == g_ItemByPath.end())
        return false;

    int id = idIt->second;
    auto infoIt = g_InfoById.find(id);
    if (infoIt == g_InfoById.end())
        return false;

    const NMItemInfo& info = infoIt->second;
    if (!info.isToggle) return false;

    // Trust our stored state (fast), or you could read item.state != off.
    return info.checked;
}

void NativeMenu::Separator(const std::string& path)
{
    NM_RunOnMain(^{
        NSMenu* menu = nil; NSInteger idx = NSNotFound;
        NM_ResolveMenuAndIndexForSeparator(path, /*hasIndex=*/false, 0, &menu, &idx);
        if (!menu) return;

        idx = NM_ClampSepIndex(menu, (idx == NSNotFound) ? (NSInteger)menu.numberOfItems : idx);
        [menu insertItem:[NSMenuItem separatorItem] atIndex:idx];
    });
}

void NativeMenu::Separator(const std::string& path, int index)
{
    NM_RunOnMain(^{
        NSMenu* menu = nil; NSInteger idx = NSNotFound;
        NM_ResolveMenuAndIndexForSeparator(path, /*hasIndex=*/true, (NSInteger)index, &menu, &idx);
        if (!menu) return;

        idx = NM_ClampSepIndex(menu, idx);
        [menu insertItem:[NSMenuItem separatorItem] atIndex:idx];
    });
}

void NativeMenu::RemoveSeparators(const std::string& path)
{
    NSMenu* menu = NM_GetTargetMenuForPath(path);
    if (!menu) return;

    NSArray<NSMenuItem*>* items = [[menu itemArray] copy];
    for (NSMenuItem* mi in items)
    {
        if (mi.separatorItem)
            [menu removeItem:mi];
    }
}

void NativeMenu::RemoveSeparator(const std::string& path)
{
    // If path points to an item, remove any separator immediately before it
    auto it = g_ItemByPath.find(path);
    if (it == g_ItemByPath.end()) return;

    NSMenuItem* item = it->second;
    if (!item) return;

    NSMenu* parent = item.menu;
    if (!parent) return;

    NSInteger idx = [parent indexOfItem:item];
    if (idx > 0)
    {
        NSMenuItem* prev = [parent itemAtIndex:idx - 1];
        if (prev.separatorItem)
            [parent removeItemAtIndex:idx - 1];
    }
}

void NativeMenu::RemoveSeparator(const std::string& path, int index)
{
    NSMenu* menu = NM_GetTargetMenuForPath(path);
    if (!menu) return;

    if (index < 0 || index >= menu.numberOfItems) return;

    NSMenuItem* mi = [menu itemAtIndex:index];
    if (mi.separatorItem)
        [menu removeItemAtIndex:index];
}

static void NM_SetShortcut(NSMenuItem* item, const std::string& shortcut)
{
    if (shortcut.empty() || !item) return;

    NSString* s = [NSString stringWithUTF8String:shortcut.c_str()];
    NSArray<NSString*>* parts = [s componentsSeparatedByString:@"+"];

    NSEventModifierFlags mods = 0;
    NSString* key = nil;

    for (NSString* part in parts)
    {
        NSString* p = [part lowercaseString];
        if ([p isEqualToString:@"cmd"] || [p isEqualToString:@"command"])
            mods |= NSEventModifierFlagCommand;
        else if ([p isEqualToString:@"ctrl"] || [p isEqualToString:@"control"])
            mods |= NSEventModifierFlagControl;
        else if ([p isEqualToString:@"alt"] || [p isEqualToString:@"option"])
            mods |= NSEventModifierFlagOption;
        else if ([p isEqualToString:@"shift"])
            mods |= NSEventModifierFlagShift;
        else
            key = part; // last token = key
    }

    if (key)
    {
        item.keyEquivalentModifierMask = mods;
        item.keyEquivalent = [key lowercaseString]; // ej. "o", "s", "f5" doesn't work, needs manual mapping
    }
}

void NativeMenu::Add(const std::string& path,
                     std::function<void()> callback,
                     bool toggle,
                     const std::string& shortcut)
{
    if (!g_Target) g_Target = [NMFileScopedMenuTarget new];

    auto parts = Split(path);
    if (parts.empty()) return;

    // Build submenu chain
    std::string prefix;
    for (size_t i = 0; i + 1 < parts.size(); ++i)
    {
        prefix = prefix.empty() ? parts[i] : (prefix + "/" + parts[i]);
        (void)EnsureMenu(prefix);
    }

    NSMenu* parent = (parts.size() == 1) ? EnsureMenu(parts[0]) : g_MenuByPath[prefix];

    int id = g_NextId++;
    g_IdByPath[path] = id;
    g_IsToggleByPath[path] = toggle;

    NSMenuItem* item = [[NSMenuItem alloc] initWithTitle:[NSString stringWithUTF8String:parts.back().c_str()]
                                                  action:@selector(nm_onMenu:)
                                           keyEquivalent:@""];
    [item setTarget:g_Target];
    [item setRepresentedObject:@(id)];

    if (toggle)
        [item setState:NSControlStateValueOff];

    // Set shortcut if provided
    //NM_SetShortcut(item, shortcut);
    NM_ApplyShortcut(item, shortcut);

    [parent addItem:item];
    g_ItemByPath[path] = item;

    NMItemInfo info;
    info.cb       = std::move(callback);
    info.isToggle = toggle;
    info.checked  = false;
    info.item     = item;
    g_InfoById[id] = std::move(info);
}


void NativeMenu::Shutdown(GLFWwindow* /*window*/)
{
    g_MenuByPath.clear();
    g_ItemByPath.clear();
    g_IdByPath.clear();
    g_IsToggleByPath.clear();
    g_InfoById.clear();
    g_NextId = 30000;
}
#endif