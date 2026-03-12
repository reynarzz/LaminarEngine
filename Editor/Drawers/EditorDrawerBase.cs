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
        private readonly vec2 _imageIconSize = new vec2(40, 40);

        internal virtual vec2? WindowsPadding { get; } = default;
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
            var cursorStart = ImGui.GetCursorPos();

            ImGui.Dummy(new Vector2(2, _imageIconSize.y));
            ImGui.SameLine();

            var icon = GetIcon(target);

            var scale = Mathf.Min(_imageIconSize.x / icon.Width, _imageIconSize.y / icon.Height);
            var scaleTooltip = Mathf.Min(100.0f / icon.Width, 100.0f / icon.Height);

            var scaledW = icon.Width * scale;
            var scaledH = icon.Height * scale;

            var offsetY = (_imageIconSize.y - scaledH) * 0.5f;

            ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), cursorStart.Y + offsetY));

            EditorImGui.Image(EditorTextureDatabase.GetIconImGui(icon), new vec2(scaledW, scaledH));
            ImGui.SameLine();

            ImGui.SetCursorPosY(cursorStart.Y + 6);
            ImGui.SetCursorPosX(cursorStart.X + 63);
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