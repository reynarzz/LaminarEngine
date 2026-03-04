using Editor.Utils;
using Engine;
using GlmNet;
using ImGuiNET;
using SPIRVCross.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal abstract class EditorDrawerBase
    {
        internal virtual vec2 WindowsPadding { get; } = default;
        protected abstract bool AutoDrawTitle { get; }
        internal virtual void OnOpen(IObject target) { }
        internal virtual void OnClose() { }
        internal abstract void OnDraw(IObject target);
        protected virtual Texture2D GetIcon(IObject target)
        {
            return EditorTextureDatabase.GetIcon(target.GetType());
        }
        private protected void DrawTitle(IObject target)
        {
            var cursorY = ImGui.GetCursorPosY();
            ImGui.Dummy(new Vector2(2, 0));
            ImGui.SameLine();
            EditorImGui.Image(EditorTextureDatabase.GetIconImGui(GetIcon(target)), new vec2(30, 30));
            ImGui.SameLine();
            ImGui.SetCursorPosY(cursorY + 6);
            ImGui.Text(target.Name);
            ImGui.SetItemTooltip((target as AssetResourceBase)?.Path ?? target.GetID().ToString());

            ImGui.Separator();

            ImGui.Dummy(new Vector2(0, 2));
        }

    }

    internal abstract class EditorDrawerBase<T> : EditorDrawerBase where T : class, IObject
    {
        internal override sealed void OnOpen(IObject target)
        {
            OnOpen(target as T);
        }
        internal virtual void OnOpen(T target) {}

        protected sealed override Texture2D GetIcon(IObject target)
        {
           return GetTitleIcon(target as T);
        }

        internal sealed override void OnDraw(IObject target)
        {
            if (AutoDrawTitle)
            {
                DrawTitle(target);
            }
            OnDraw(target as T);
        }

        protected abstract void OnDraw(T target);
        protected virtual Texture2D GetTitleIcon(T target)
        {
           return base.GetIcon(target);
        }
    }
}