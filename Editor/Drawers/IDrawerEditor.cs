using Editor.Utils;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal interface IDrawerEditor
    {
        bool AutoDrawTitle { get; }
        void OnOpen();
        void OnClose();
        internal abstract void OnDraw(IObject target);
        private protected void DrawTitle(IObject target)
        {
            var cursorY = ImGui.GetCursorPosY();
            ImGui.Dummy(new System.Numerics.Vector2(2, 0));
            ImGui.SameLine();

            ImGui.Image(EditorTextureDatabase.GetIconImGui(EditorIcon.Stop), new System.Numerics.Vector2(30, 30));
            ImGui.SameLine();
            ImGui.SetCursorPosY(cursorY + 6);
            ImGui.Text(target.Name);
            ImGui.SetItemTooltip((target as AssetResourceBase)?.Path ?? target.GetID().ToString());

            ImGui.Separator();

            ImGui.Dummy(new System.Numerics.Vector2(0, 2));
        }
    }

    internal interface IDrawerEditor<T> : IDrawerEditor where T : class, IObject
    {
        void IDrawerEditor.OnDraw(IObject target)
        {
            if (AutoDrawTitle)
            {
                DrawTitle(target);
            }
            OnDraw(target as T);
        }

        void OnDraw(T target);
    }
}
