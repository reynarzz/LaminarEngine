using Editor.Cooker;
using Editor.Data;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ProjectWizardWindow : EditorWindow
    {
        public override void OnDraw()
        {
            ImGui.Begin("Project");
            if (ImGui.Button("Open project"))
            {
                if (EditorFileDialog.PickFolder("C:/", out var selected))
                {
                    var isValid = IsValidProject(selected);
                    Debug.Log(selected + "Is Valid: " + isValid);
                }
            }

            ImGui.End();
        }

        private bool IsValidProject(string root)
        {
            // Check if is has project settings folder
            var projectSettingsPath = Path.Combine(root, Paths.PROJECT_SETTINGS_FOLDER_NAME);
            if (!Directory.Exists(projectSettingsPath))
            {
                return false;
            }

            if (!File.Exists(Path.Combine(projectSettingsPath, EditorPaths.PROJECT_SETTINGS_DAT_FULL_NAME)))
            {
                return false;
            }

            // EditorProjectDataManager.SaveProjectSettings

            var projectsFiles = Directory.EnumerateFiles(root, "*.csproj");

            var gameCsProjFullPath = string.Empty;
            foreach (var item in projectsFiles)
            {
                gameCsProjFullPath = Paths.ClearPathSeparation(item);
                break;
            }

            if (string.IsNullOrEmpty(gameCsProjFullPath))
            {
                // TODO: ask user to pick the project or select one.
            }

            var assemblyName = Path.GetFileName(gameCsProjFullPath);
            EditorPaths.GameRoot = Paths.ClearPathSeparation(root);
            EditorPaths.GameCsProjName = Path.GetFileNameWithoutExtension(assemblyName); // derive from project settings, and if is not found ask the user to pick it or create it.
            GameProject.Initialize(new ProjectConfig() { ProjectFolderRoot = EditorPaths.GameRoot });

            var projectInfo = new EditorLoadedProjectData()
            {
                AssemblyName = assemblyName,
                AssemblyAbsolutePath = gameCsProjFullPath,
                ProjectRootPath = root,
                ProjectName = "Game"
            };
            EditorConfigManager.SetLoadedProject(projectInfo);

            return true;
        }
    }
}