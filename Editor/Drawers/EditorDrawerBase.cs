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
        internal virtual void OnOpen(IObject target) { }
        internal virtual void OnClose() { }
        internal abstract void OnDraw(IObject target);
        protected virtual TitleIconInfo GetIcon(IObject target)
        {
            return new TitleIconInfo() { Texture = EditorTextureDatabase.GetIcon(target.GetType()) };
        }
        private protected void DrawTitle(IObject target)
        {
            var cursorStart = ImGui.GetCursorPos();

            ImGui.Dummy(new Vector2(2, _imageIconSize.y));
            ImGui.SameLine();

            var iconInfo = GetIcon(target);
            var icon = iconInfo.Texture;

            if (icon)
            {
                var width = iconInfo.Size == null ? icon.Width: iconInfo.Size.Value.x;
                var height = iconInfo.Size == null ? icon.Height : iconInfo.Size.Value.y;

                var scale = Mathf.Min(_imageIconSize.x / width, _imageIconSize.y / height);
                var scaleTooltip = Mathf.Min(100.0f / width, 100.0f / height);

                var scaledW = width * scale;
                var scaledH = height * scale;

                var offsetY = (_imageIconSize.y - scaledH) * 0.5f;

                ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), cursorStart.Y + offsetY));

                if (iconInfo.Uvs == null)
                {
                    EditorImGui.Image(EditorTextureDatabase.GetIconImGui(icon), new vec2(scaledW, scaledH));
                }
                else
                {
                    EditorImGui.Image(EditorTextureDatabase.GetIconImGui(icon), new vec2(scaledW, scaledH), iconInfo.Uvs.Value);
                }
                ImGui.SameLine();
            }

            ImGui.SetCursorPosY(cursorStart.Y + 6);
            ImGui.SetCursorPosX(cursorStart.X + 63);
            ImGui.Text(target.Name);


            ImGui.SetItemTooltip((target as Asset)?.Path ?? target.GetID().ToString());

            ImGui.Separator();

            ImGui.Dummy(new Vector2(0, 2));
        }

        protected struct TitleIconInfo
        {
            public Texture2D Texture { get; set; }
            public QuadUV? Uvs { get; set; }
            public vec2? Size { get; set; }
        }
    }

    internal abstract class EditorDrawerBase<T> : EditorDrawerBase where T : class, IObject
    {
        protected abstract bool AutoDrawTitle { get; }

        internal override sealed void OnOpen(IObject target)
        {
            OnOpen(target as T);
        }
        internal virtual void OnOpen(T target) { }

        protected sealed override TitleIconInfo GetIcon(IObject target)
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
        protected virtual TitleIconInfo GetTitleIcon(T target)
        {
            return base.GetIcon(target);
        }

    }
}