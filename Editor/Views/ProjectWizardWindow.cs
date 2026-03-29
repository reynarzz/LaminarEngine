using Editor.Cooker;
using Editor.Data;
using Editor.Utils;
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

            OpenProjectGUI();
            ImGui.Separator();
            CreateProjectGUI();
            ImGui.End();
        }

        private void OpenProjectGUI()
        {
            if (ImGui.Button("Open project"))
            {
                if (EditorFileDialog.PickFolder("C:/", out var selected))
                {
                    if (IsValidProject(selected, out var csProjFullPath))
                    {
                        LoadProject(selected, csProjFullPath);
                        Debug.Log(selected + "Is Valid: ");
                    }
                }
            }
        }

        private static ProjectCreatedInfo _projectCreateInfo = new();

        private void CreateProjectGUI()
        {
            ImGui.Text("Project Name");
            ImGui.SameLine();
            EditorGuiFieldsResolver.DrawStringField("", ref _projectCreateInfo.ProjectName);

            ImGui.Text("Directory");
            ImGui.SameLine();
            ImGui.TextWrapped(_projectCreateInfo.ProjectRootDirectory);
            ImGui.SameLine();
            if (ImGui.Button("Select"))
            {
                if (EditorFileDialog.PickFolder(_projectCreateInfo.ProjectRootDirectory, out var selected))
                {
                    _projectCreateInfo.ProjectRootDirectory = selected;
                }
            }

            ImGui.Text("Use intermediary directory");
            ImGui.SameLine();
            EditorGuiFieldsResolver.DrawBoolField("###_Intermediary_", ref _projectCreateInfo.UseIntermediaryDirectory);

            ImGui.BeginDisabled(!_projectCreateInfo.IsValidProjectData());
            if (ImGui.Button("Create Project"))
            {
                GameProject.CreateDefaultProject(_projectCreateInfo);
            }
            ImGui.EndDisabled();
        }
        private bool IsValidProject(string root, out string csProjectFullPath)
        {
            csProjectFullPath = string.Empty;
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

            var projectsFiles = Directory.EnumerateFiles(root, "*.csproj");

            foreach (var item in projectsFiles)
            {
                csProjectFullPath = Paths.ClearPathSeparation(item);
                break;
            }

            if (string.IsNullOrEmpty(csProjectFullPath))
            {
                // TODO: ask the user to pick the project or select one.
                return false;
            }

            return true;
        }

        private void LoadProject(string projectDirRootAbsolutePath, string gameCsprojAbsolutePath)
        {
            var assemblyName = Path.GetFileName(gameCsprojAbsolutePath);
            projectDirRootAbsolutePath = Paths.ClearPathSeparation(projectDirRootAbsolutePath); ;
            EditorPaths.GameRoot = projectDirRootAbsolutePath;
            EditorPaths.GameCsProjName = Path.GetFileNameWithoutExtension(assemblyName); // derive from project settings, and if is not found ask the user to pick it or create it.
            GameProject.Initialize(new ProjectConfig() { ProjectFolderRoot = EditorPaths.GameRoot });

            var projectInfo = new EditorLoadedProjectData()
            {
                ProjectName = "Game",
                ProjectRootPath = projectDirRootAbsolutePath,
                AssemblyName = assemblyName,
                AssemblyAbsolutePath = Paths.ClearPathSeparation(gameCsprojAbsolutePath),
            };

            EditorConfigManager.SetLoadedProject(projectInfo);
        }

    }
}