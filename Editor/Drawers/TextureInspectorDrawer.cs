using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using ImGuiNET;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private Texture2D _texture;
        private TextureMetaFile _meta;
        private MemberInfo[] _configProperties;
        protected override Texture2D GetTitleIcon(Texture2D target)
        {
            // Pass the same texture as the icon.
            return target;
        }
        internal override void OnOpen(Texture2D target)
        {
            LoadTextureData(target);
        }

        private void LoadTextureData(Texture2D target)
        {
            _texture = target;
            _meta = EditorAssetUtils.GetAssetMeta(target) as TextureMetaFile;
            _configProperties = _meta.Config.GetType().GetProperties();
        }
        protected override void OnDraw(Texture2D target)
        {
            bool wasTextureChanged = _texture != target;

            if (wasTextureChanged)
            {
                LoadTextureData(target);
            }
            foreach (var property in _configProperties)
            {
                PropertyDrawer.DrawVars(target.GetID().ToString(), _meta.Config, property, false);
            }

            if (ImGui.Button("Edit Atlas"))
            {
                _atlasEditor.OnOpen(target, _meta);
                _openAtlasEditor = true;
            }

            if (_openAtlasEditor)
            {
                if (wasTextureChanged)
                {
                    _atlasEditor.OnOpen(target, _meta);
                }
                _atlasEditor.OnDraw(target);
            }
        }

        internal override void OnClose()
        {
            _openAtlasEditor = false;
            _texture = null;
            _meta = null;
            _configProperties = null;
        }
    }
}