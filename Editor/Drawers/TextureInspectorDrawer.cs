using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    internal class TextureInspectorDrawer : EditorDrawerBase<Texture2D>
    {
        protected override bool AutoDrawTitle => true;
        private readonly TextureAtlasEditorView _atlasEditor = new();
        private bool _openAtlasEditor = false;
        private Texture2D _texture;
        private TextureMetaFile _meta;

        internal TextureType ToTextureType(AssetType type)
        {
            switch (type)
            {
                case AssetType.Texture:
                    return TextureType.Texture;
                case AssetType.Texture2D:
                    return TextureType.Texture2D;
                case AssetType.Texture3D:
                    return TextureType.Texture3D;
                case AssetType.Sprite:
                    return TextureType.Sprite;
                case AssetType.Cubemap:
                    return TextureType.CubeMap;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unknown AssetType: {type}");
            }
        }
        internal AssetType ToAssetType(TextureType type)
        {
            switch (type)
            {
                case TextureType.Texture:
                    return AssetType.Texture;
                case TextureType.Texture2D:
                    return AssetType.Texture2D;
                case TextureType.Texture3D:
                    return AssetType.Texture3D;
                case TextureType.Sprite:
                    return AssetType.Sprite;
                case TextureType.CubeMap:
                    return AssetType.Cubemap;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unknown TextureType: {type}");
            }
        }
        protected override TitleIconInfo GetTitleIcon(Texture2D target)
        {
            // Pass the same texture as the icon.
            return new TitleIconInfo() { Texture = target };
        }
        internal override void OnOpen(Texture2D target)
        {
            LoadTextureData(target);
        }

        private void LoadTextureData(Texture2D target)
        {
            _texture = target;
            _meta = EditorAssetUtils.GetAssetMeta(target) as TextureMetaFile;
        }
        protected override void OnDraw(Texture2D target)
        {
            bool wasTextureChanged = _texture != target;

            if (wasTextureChanged)
            {
                LoadTextureData(target);
            }
            var texType = _meta.Config.Type;
            ImGui.Text("Texture Type");
            ImGui.SameLine();
            if (EditorGuiFieldsResolver.DrawEnum("###_TEXTURE_TYPE__", ref texType))
            {
                _meta.Config.Type = texType;
            }

            if (_meta.Config.Type == TextureType.Sprite)
            {
                var isAtlas = _meta.Config.IsAtlas;
                ImGui.Text("Is Atlas");
                ImGui.SameLine();
                EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
                if (EditorGuiFieldsResolver.DrawBoolField("###_TEXTURE_IS_ATLAS__", ref isAtlas, EditorGuiFieldsResolver.XPosOffset))
                {
                    _meta.Config.IsAtlas = isAtlas;
                }

                var ppu = _meta.Config.PixelPerUnit;
                ImGui.Text("Pixels per Unit");
                ImGui.SameLine();
                EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
                if (EditorGuiFieldsResolver.DrawIntField("###_PIXELS_PER_UNIT", ref ppu))
                {
                    _meta.Config.PixelPerUnit = ppu;
                }
            }
            else
            {
                _openAtlasEditor = false;
            }

            var wrapMode = _meta.Config.Mode;
            ImGui.Text("Wrap Mode");
            ImGui.SameLine();
            if (EditorGuiFieldsResolver.DrawEnum("###_WRAP_MODE_", ref wrapMode))
            {
                _meta.Config.Mode = wrapMode;
            }

            var filterMode = _meta.Config.Filter;
            ImGui.Text("Filter Mode");
            ImGui.SameLine();
            if (EditorGuiFieldsResolver.DrawEnum("###_Filter_MODE_", ref filterMode))
            {
                _meta.Config.Filter = filterMode;
            }

            ImGui.BeginDisabled(!_meta.Config.IsAtlas);

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
            if (ImGui.Button("Apply", new Vector2(ImGui.GetContentRegionAvail().X, 23)))
            {
                EditorAssetUtils.WriteMeta(target.Path, _meta);
                EditorAssetUtils.RefreshAssetDatabase();
            }
            ImGui.EndDisabled();
        }

        internal override void OnClose()
        {
            _openAtlasEditor = false;
            _texture = null;
            _meta = null;
        }
    }
}