using Editor;
using Editor.Utils;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    [PropertyDrawer(nameof(TilemapRenderer.Options))]
    internal class TilemapPropertyDrawer : PropertyDrawer<TilemapRenderer, TilemapRenderingOptions>
    {
        private static readonly string[] _none = { "None" };
        protected internal override bool OnDrawProperty(Type type, string name, object target, ref TilemapRenderingOptions valueIn,
                                                        out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            var tilemap = target.GetType().GetProperty("Tilemap").GetValue(target) as TilemapAsset;

            ImGui.BeginDisabled(!tilemap);
            bool anyChange = false;
            if (tilemap)
            {
                var data = tilemap.GetData();
                if (data.Levels.Count > 0)
                {
                    var levelNames = data.Levels.Values.Select(x => x.Identifier);
                    if(DrawLevelCombo("Level", ref valueIn.LevelIndex, levelNames.ToArray())) // TODO: Improve, memory alloc
                    {
                        anyChange = true;
                    }

                    var anyLevel = data.Levels.First().Value;
                    var layerNames = anyLevel.Layers.Values.Select(x => x.Identifier);
                    // var layerNames = anyLevel.Layers.Values.Where(x => x.Type != TilemapLayerType.Entities).Select(x => x.Identifier);

                    if(DrawLevelCombo("Layer", ref valueIn.LayerIndex, layerNames.ToArray())) // TODO: Improve, memory alloc
                    {
                        anyChange = true;
                    }
                }
            }
            else
            {
                int defaultVal = 0;
                DrawLevelCombo("Level", ref defaultVal, _none);
                DrawLevelCombo("Layer", ref defaultVal, _none);
            }

            valueOut = valueIn;
            ImGui.EndDisabled();
            return anyChange;
        }

        private bool DrawLevelCombo(string name, ref int index, string[] elements)
        {
            IndentProperty();
            ImGui.Text(name);
            ImGui.SameLine();
            AlignProperty();
            var result = EditorGuiFieldsResolver.DrawCombo("##__Tilemap_Option__" + name, ref index, elements);
            UnindentProperty();

            return result;
        }
    }
}