using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class TextureInspectorDrawer : EditorDrawerBase<Texture2D>
    {
        private readonly static Type[] _visibilityAttributes = [typeof(SerializedFieldAttribute), typeof(ShowFieldNoSerialize)];
        protected override bool AutoDrawTitle => true;
        private readonly TextureAtlasEditorView _atlasEditor = new();
        private bool _openAtlasEditor = false;
        protected override Texture2D GetTitleIcon(Texture2D target)
        {
            // Pass the same texture as the icon.
            return target;
        }

        protected override void OnDraw(Texture2D target)
        {
            var members = ReflectionUtils.GetAllMembersWithAttributes(target.GetType(), _visibilityAttributes, true, true);
            foreach (var member in members)
            {
                PropertyDrawer.DrawVars(target.GetID().ToString(), target, member);
            }

            if (ImGui.Button("Edit Atlas"))
            {
                _atlasEditor.OnOpen(target);
                _openAtlasEditor = true;
            }

            if (_openAtlasEditor)
            {
                _atlasEditor.OnDraw(target);
            }
        }
    }
}