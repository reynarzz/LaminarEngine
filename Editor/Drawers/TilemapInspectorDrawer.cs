using Engine;
using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using ImGuiNET;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class TilemapInspectorDrawer : EditorDrawerBase<TilemapAsset>
    {
        protected override bool AutoDrawTitle { get; } = true;

        private TilemapMeta _meta;
        private TilemapLevelConfig[] _levelConfig;
        internal override void OnOpen(TilemapAsset target)
        {
            _meta = EditorAssetUtils.GetAssetMeta(target) as TilemapMeta;
            
            // TODO: maintain the meta in sync with the tilemap level and layers here

            var data = target.GetData();
            // data.Levels.Layers
            _levelConfig = _meta.LevelConfig;
        }

        protected override void OnDraw(TilemapAsset target)
        {
            var data = target.GetData();
            
            for (int i = 0; i < data.Levels.Count; i++)
            {
                var level = data.Levels[data.Levels.Keys.ElementAt(i)];

                var levelConfig = _levelConfig[i];
                var show = ImGui.TreeNodeEx($"{level.Identifier}##{i}");

                if (show)
                {
                    for (int j = 0; j < level.Layers.Count; j++)
                    {
                        var layer = level.Layers[level.Layers.Keys.ElementAt(j)];

                        var guid = levelConfig.LayersTextureRef[j];
                        var texture = Assets.GetAssetFromGuid(guid) as TextureAsset;
                        ImGui.Text(layer.Identifier);
                        ImGui.SameLine();
                        ImGui.PushID($"{i}.{j}");
                        EditorGuiFieldsResolver.DrawEObjectSlot(texture?.Texture, typeof(Texture2D), (val) =>
                        {
                            guid = (val as EObject)?.GetID() ?? Guid.Empty;
                            return true;
                        });
                        ImGui.PopID();
                        levelConfig.LayersTextureRef[j] = guid;
                    }

                    ImGui.TreePop();
                }
            }

            ApplyMeta(target, _meta);
        }

        public void ApplyMeta(AssetResourceBase asset, AssetMeta meta)
        {
            if (ImGui.Button("Apply All", new Vector2(ImGui.GetContentRegionAvail().X, 23)))
            {
                EditorAssetUtils.WriteMeta(asset.Path, meta);

                EditorAssetUtils.RefreshAssetDatabase();
            }
        }
        internal override void OnClose()
        {
        }
    }
}