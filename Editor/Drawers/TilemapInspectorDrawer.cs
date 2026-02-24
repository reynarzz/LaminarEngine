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
        private TilemapLayerConfig[] _layerConfig;
        internal struct LayerInfo
        {
            public string Identifier { get; set; }
            public TilemapLayerType Type { get; set; }
        }

        private LayerInfo[] _layersInfo;

        internal override void OnOpen(TilemapAsset target)
        {
            _meta = EditorAssetUtils.GetAssetMeta(target) as TilemapMeta;


            var data = target.GetData();
            var level = data.Levels.FirstOrDefault().Value;

            if(level != null)
            {
                if(level.Layers != null && level.Layers.Count > 0)
                {
                    var layers = level.Layers.Values.ToArray();
                    _layersInfo = new LayerInfo[layers.Length];

                    for ( int i = 0; i < layers.Length; i++ ) 
                    {
                        _layersInfo[i] = new LayerInfo()
                        {
                            Identifier = layers[i].Identifier,
                            Type = layers[i].Type
                        };
                    }
                }
            }

            // TODO: maintain the meta in sync with the tilemap level and layers here

            // data.Levels.Layers
            _layerConfig = _meta.Layers;
        }

        protected override void OnDraw(TilemapAsset target)
        {
            var data = target.GetData();

          //  var show = ImGui.TreeNodeEx($"Layer");

          //  if (show)
            {
                for (int j = 0; j < _layerConfig.Length; j++)
                {
                    ref var layerConfig = ref _layerConfig[j];
                    ref var layer = ref _layersInfo[j];
                    ImGui.PushID($"{j}");

                    if (layer.Type == TilemapLayerType.Entities)
                    {
                        ImGui.Text(layer.Identifier + " (Pixels per unit)");
                        ImGui.SameLine();
                        ImGui.SetCursorPosX(MathF.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                        EditorGuiFieldsResolver.DrawIntField("##Pixels Per unit", ref layerConfig.EntityPixelPerUnit);
                    }
                    else
                    {
                        ImGui.Text(layer.Identifier);

                        var guid = layerConfig.TextureRef;
                        var texture = Assets.GetAssetFromGuid(guid) as TextureAsset;
                        ImGui.SameLine();
                        EditorGuiFieldsResolver.DrawEObjectSlot(texture?.Texture, typeof(Texture2D), (val) =>
                        {
                            guid = (val as EObject)?.GetID() ?? Guid.Empty;
                            return true;
                        });
                        layerConfig.TextureRef = guid;

                    }

                    ImGui.PopID();
                }

                //ImGui.TreePop();
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