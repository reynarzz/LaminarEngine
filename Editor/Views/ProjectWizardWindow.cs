using Editor.Cooker;
using Editor.Data;
using Editor.Utils;
using Engine;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ProjectWizardWindow : EditorWindow
    {
        public override void OnDraw()
        {

            var viewport = ImGui.GetMainViewport();
            var winSize = new Vector2(viewport.Size.X / 1.2f, 400);

            ImGui.SetNextWindowSize(winSize, ImGuiCond.Always);
            ImGui.SetNextWindowPos(viewport.Pos.X + viewport.Size.X * 0.5f - winSize.X * 0.5f,
                                   viewport.Pos.Y + viewport.Size.Y * 0.5f - winSize.Y * 0.5f);
            ImGui.Begin("Project",  ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);
            var projWinSize = ImGui.GetContentRegionAvail();
            ImGui.BeginChild("__OPEN_PROJECT__", new Vector2(projWinSize.X / 1.5f, projWinSize.Y));
            OpenProjectGUI();
            ImGui.EndChild();
            
            ImGui.SameLine();

            ImGui.BeginChild("__CREATE_PROJECT__");
            CreateProjectGUI();
            ImGui.EndChild();

            ImGui.End();
        }


        private void OpenProjectGUI()
        {
            if (ImGui.Button("Open project"))
            {
                if (EditorFileDialog.PickFolder("C:/", out var rootFolderSelected))
                {
                    if (IsValidProject(rootFolderSelected))
                    {
                        LoadProject(rootFolderSelected);
                        Debug.Log(rootFolderSelected + "Is Valid: ");
                    }
                }
            }

            ImGui.Separator();

            if (ImGui.BeginTable("##ProjectsTable", 3, ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.SizingStretchProp))
            {
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn("Modified");
                ImGui.TableSetupColumn("Settings");
                ImGui.TableHeadersRow();

                var path = "C:/User/Games/Project";
                float padding = 5.0f;

                for (int i = 0; i < 3; i++)
                {
                    ImGui.TableNextRow();

                    ImGui.TableSetColumnIndex(0);

                    string projectName = $"Project {i}";
                    string projectPath = path;

                    float titleHeight = ImGui.GetTextLineHeight() * 1.2f;
                    float pathHeight = ImGui.GetTextLineHeight();
                    float rowHeight = padding + titleHeight + pathHeight + padding;

                    Vector2 cursorPos = ImGui.GetCursorScreenPos();
                    Vector2 avail = ImGui.GetContentRegionAvail();

                    ImGui.InvisibleButton($"##ProjectSelect{i}", new Vector2(avail.X, rowHeight));

                    bool hovered = ImGui.IsItemHovered();
                    bool held = ImGui.IsItemActive();
                    bool clicked = ImGui.IsItemClicked();

                    uint bgColor = 0;

                    if (held)
                    {
                        bgColor = ImGui.GetColorU32(ImGuiCol.HeaderActive);
                    }
                    else if (hovered)
                    {
                        bgColor = ImGui.GetColorU32(ImGuiCol.HeaderHovered);
                    }

                    if (bgColor != 0)
                    {
                        var drawList = ImGui.GetWindowDrawList();
                        Vector2 min = cursorPos;
                        Vector2 max = new Vector2(cursorPos.X + avail.X, cursorPos.Y + rowHeight);

                        drawList.AddRectFilled(min, max, bgColor, ImGui.GetStyle().FrameRounding);
                    }

                    ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + padding, cursorPos.Y + padding));

                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1f));
                    ImGui.SetWindowFontScale(1.2f);
                    ImGui.TextUnformatted(projectName);
                    ImGui.SetWindowFontScale(1.0f);
                    ImGui.PopStyleColor();

                    cursorPos = ImGui.GetCursorScreenPos();
                    ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + padding, cursorPos.Y - padding));

                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 1f));
                    ImGui.TextUnformatted(projectPath);
                    ImGui.PopStyleColor();

                    if (clicked)
                    {
                        // LoadProject(projectPath);
                    }

                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text("2026-03-30");

                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.SmallButton($"...##Settings{i}"))
                    {
                    }
                }

                ImGui.EndTable();
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
                LoadProject(_projectCreateInfo.ProjectRootDirectory);
            }
            ImGui.EndDisabled();
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

            var projectsFiles = Directory.EnumerateFiles(root, "*.csproj");
            var csProjectFullPath = string.Empty;

            foreach (var item in projectsFiles)
            {
                var path = Paths.ClearPathSeparation(item);
                if (Path.GetFileName(path).Equals(EditorPaths.GAME_PROJECT_FULL_NAME))
                {
                    csProjectFullPath = path;
                    break;
                }
            }

            try
            {
                if (string.IsNullOrEmpty(csProjectFullPath))
                {
                    Debug.Warn("Not valid .csproj was found. Creating default one now");
                    var projFullPath = Paths.ClearPathSeparation(Path.Combine(root, EditorPaths.GAME_PROJECT_FULL_NAME));

                    GameProject.CreateDefaultProject(new ProjectCreatedInfo() { ProjectName = "Game", ProjectRootDirectory = projFullPath });
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }

            return true;
        }

        private void LoadProject(string projectDirRootAbsolutePath)
        {
            if (!Directory.Exists(projectDirRootAbsolutePath))
            {
                Debug.Warn($"Project: '{projectDirRootAbsolutePath}' doesn't exists.");
                return;
            }
            projectDirRootAbsolutePath = Paths.ClearPathSeparation(projectDirRootAbsolutePath);

            var gameCsprojAbsolutePath = Path.Combine(projectDirRootAbsolutePath, EditorPaths.GAME_PROJECT_FULL_NAME);
            var assemblyName = Path.GetFileName(gameCsprojAbsolutePath);
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