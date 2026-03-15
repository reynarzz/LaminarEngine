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
        protected override void OnDraw(ProjectSettingsData settings)
        {
            var launchScene = Assets.GetAssetFromGuid(GetGuidSafe(settings.SceneSettings.MainScene));
            ImGui.Text("Launch Scene");
            ImGui.SameLine();
            EditorGuiFieldsResolver.DrawEObjectSlot(launchScene, typeof(SceneAsset), x =>
            {
                settings.SceneSettings.MainScene = ((x as SceneAsset)?.GetID() ?? Guid.Empty).ToString();
                return true;
            });

            DrawSceneList("Scenes", ref settings.SceneSettings.Scenes);
        }

        private Guid GetGuidSafe(string str)
        {
            Guid.TryParse(str, out var id);
            return id;
        }

        private void DrawSceneList(string title, ref SceneSettings.SceneBuildInfo[] sceneList)
        {
            ImGui.Text(title);
            ImGui.SameLine();
            EditorGuiFieldsResolver.DrawArrayField(title, ref sceneList, false, ArrayDrawer, OnAdded);

            void OnAdded(SceneSettings.SceneBuildInfo sceneInfo)
            {
                sceneInfo.IsBuildAdded = true;
                sceneInfo.RefId = Guid.Empty.ToString();
            }

            bool ArrayDrawer(int index, float width, object item)
            {
                SceneAsset scene = null;
                var sceneInfo = (SceneSettings.SceneBuildInfo)item;
                if (Guid.TryParse(sceneInfo?.RefId, out var guid))
                {
                    scene = Assets.GetAssetFromGuid(guid) as SceneAsset;
                }
                EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
                var isBuildAdded = sceneInfo.IsBuildAdded;
                EditorGuiFieldsResolver.DrawBoolField($"##IsAddedToTheBuild_{index}", ref isBuildAdded);
                ImGui.SameLine();
                ImGui.PushID($"{title}_SCENES_SETTINGS_{index}");
                EditorGuiFieldsResolver.DrawEObjectSlot(scene, typeof(SceneAsset), x =>
                {
                    scene = x as SceneAsset;
                    return true;
                });
                ImGui.PopID();

                var id = (scene?.GetID() ?? Guid.Empty).ToString();

                if (!sceneInfo.RefId?.Equals(id) ?? true)
                {
                    sceneInfo.RefId = id;
                    return true;
                }

                if (sceneInfo.IsBuildAdded != isBuildAdded)
                {
                    sceneInfo.IsBuildAdded = isBuildAdded;
                    return true;
                }

                return false;
            }
        }
    }
}
