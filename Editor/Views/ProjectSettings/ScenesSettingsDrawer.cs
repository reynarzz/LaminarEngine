using Editor.Utils;
using Engine;
using Engine.Data;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ScenesSettingsDrawer : ProjectMenuDrawer
    {
        protected override void OnDraw(ProjectSettings settings)
        {
            // TODO: Draw from scratch
            var launchScene = Assets.GetAssetFromGuid(GetGuidSafe(settings.SceneSettings.LaunchScene));
            ImGui.Text("Launch Scene");
            ImGui.SameLine();
            EditorGuiFieldsResolver.DrawEObjectSlot(launchScene, typeof(SceneAsset), x =>
            {
                settings.SceneSettings.LaunchScene = ((x as SceneAsset)?.GetID() ?? Guid.Empty).ToString();
                return true;
            });

            DrawSceneList("Scenes", settings.SceneSettings.Scenes);
            // PropertiesGUIDrawEditor.DrawObject("__Scene_Settings__", settings.SceneSettings);
        }

        private Guid GetGuidSafe(string str)
        {
            Guid.TryParse(str, out var id);
            return id;
        }
        private void DrawSceneList(string title, List<string> sceneList)
        {
            ImGui.Text(title);
            EditorGuiFieldsResolver.DrawListField(title, sceneList, false, (index, width, item) =>
            {
                SceneAsset scene = null;
                if (Guid.TryParse(sceneList[index], out var guid))
                {
                    scene = Assets.GetAssetFromGuid(guid) as SceneAsset;
                }
                bool build = false;
                EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
                EditorGuiFieldsResolver.DrawBoolField("##Build", ref build);
                ImGui.SameLine();
                ImGui.PushID($"{title}_SCENES_SETTINGS_{index}");
                EditorGuiFieldsResolver.DrawEObjectSlot(scene, typeof(SceneAsset), x =>
                {
                    scene = x as SceneAsset;
                    return true;
                });
                ImGui.PopID();

                var id = (scene?.GetID() ?? Guid.Empty).ToString();

                if (!sceneList[index].Equals(id))
                {
                    sceneList[index] = id;
                    return true;
                }

                return false;
            });
        }
    }
}
