using Editor.Utils;
using Engine;
using Engine.Utils;
using ImGuiNET;
using Engine;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class MaterialInspectorDrawer : EditorDrawerBase<Material>
    {
        private readonly static Type[] _visibilityAttributes = [typeof(SerializedFieldAttribute), typeof(ShowFieldNoSerialize)];
        protected override bool AutoDrawTitle => true;

        private readonly List<string> _shadersNames = new();
        private int _shaderIndex = 0;
        private OrderedDictionary<Guid, AssetInfo> _shaders;
        internal override void OnOpen(Material target)
        {
            _shaders = EditorIOLayer.Database.GetAssetsInfoByType(AssetType.ShaderV2) as OrderedDictionary<Guid, AssetInfo>;
            _shaderIndex = target.Shader ? _shaders.IndexOf(target.Shader.GetID()):0;

            _shadersNames.Capacity = _shaders.Count;
            _shadersNames.Clear();
            foreach (var item in _shaders)
            {
                _shadersNames.Add(item.Value.Path.Substring(0, item.Value.Path.LastIndexOf('.')));
            }
        }
         
        protected override void OnDraw(Material target)
        {
            ImGui.Text("Shader");

            ImGui.SameLine();
            if (EditorGuiFieldsResolver.DrawCombo("Shaders", ref _shaderIndex, _shadersNames))
            {
                target.Shader = Assets.GetAssetFromGuid(_shaders.GetAt(_shaderIndex).Key) as Shader;
            }

            if (target.Shader)
            {
                for (int i = 0; i < target.Passes.Count; i++)
                {
                    var pass = target.Passes[i];
                    ImGui.Text($"Pass: {i}");
                    foreach (var uniform in pass.Shader.Uniforms)
                    {
                        ImGui.Text(uniform.Name);

                    }
                }

            }

            var members = ReflectionUtils.GetAllMembersWithAttributes(target.GetType(), _visibilityAttributes, true, true);

            foreach (var member in members)
            {
                PropertiesDrawerEditor.DrawVars(target.GetID().ToString(), target, member);
            }
        }

        internal override void OnClose()
        {
        }
    }
}
