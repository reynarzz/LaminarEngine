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
        private static ProjectCreatedInfo _projectCreateInfo = new();

        public override void OnDraw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 2);
            var viewport = ImGui.GetMainViewport();
            var winSize = new Vector2(viewport.Size.X / 1.1f, viewport.Size.Y / 1.1f);

            float spacing = 4.0f;

            Vector2 basePos = new Vector2(viewport.Pos.X + viewport.Size.X * 0.5f - winSize.X * 0.5f,
                                          viewport.Pos.Y + viewport.Size.Y * 0.5f - winSize.Y * 0.5f);

            float leftWidth = winSize.X / 1.5f;
            float rightWidth = winSize.X - leftWidth - spacing;

            Vector2 leftSize = new Vector2(leftWidth, winSize.Y);
            Vector2 rightSize = new Vector2(rightWidth, winSize.Y);

            Vector2 leftPos = basePos;
            Vector2 rightPos = new Vector2(basePos.X + leftWidth + spacing, basePos.Y);

            ImGui.SetNextWindowPos(leftPos, ImGuiCond.Always);
            ImGui.SetNextWindowSize(leftSize, ImGuiCond.Always);
            ImGui.Begin("__OPEN_PROJECT__", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);


            OpenProjectGUI();
            ImGui.End();

            ImGui.SetNextWindowPos(rightPos, ImGuiCond.Always);
            ImGui.SetNextWindowSize(rightSize, ImGuiCond.Always);
            ImGui.Begin("__CREATE_PROJECT__", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);


            CreateProjectGUI();
            ImGui.End();
            ImGui.PopStyleVar();
        }

        private Vector2 DrawTitleBar(string title)
        {
            float titleBarHeight = 45.0f;

            var drawList = ImGui.GetWindowDrawList();
            var winPos = ImGui.GetWindowPos();
            var winSizeCurrent = ImGui.GetWindowSize();

            uint titleBg = ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.1f, 0.85f));
            drawList.AddRectFilled(winPos, new Vector2(winPos.X + winSizeCurrent.X, winPos.Y + titleBarHeight),
                                   titleBg, ImGui.GetStyle().WindowRounding,
                                   ImDrawFlags.RoundCornersTop);

            var titleCursor = ImGui.GetCursorPos();

            ImGui.SetWindowFontScale(1.6f);
            var textSize = ImGui.CalcTextSize(title);
            float centerX = (winSizeCurrent.X - textSize.X) * 0.5f;

            ImGui.SetCursorPos(new Vector2(centerX, titleCursor.Y));

            ImGui.TextUnformatted(title);
            ImGui.SetWindowFontScale(1.0f);

            var cursor = ImGui.GetCursorPos();
            ImGui.SetCursorPos(cursor.X, cursor.Y + 8.0f);

            return titleCursor;
        }
        private void OpenProjectGUI()
        {
            var titleCursor = DrawTitleBar("Projects");

            float buttonWidth = 120.0f;
            float buttonHeight = 31;
            float availX = ImGui.GetContentRegionAvail().X;
            var startCursor = ImGui.GetCursorPos();
            ImGui.SetCursorPos(titleCursor.X + availX - buttonWidth, titleCursor.Y + 1);
            if (ImGui.Button("Open project", new Vector2(buttonWidth, buttonHeight)))
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

            ImGui.SetCursorPos(startCursor);
            if (ImGui.BeginTable("##ProjectsTable", 3, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.SizingStretchProp))
            {
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.NoReorder, 80.0f);
                ImGui.TableSetupColumn("Modified", ImGuiTableColumnFlags.NoReorder, 20);
                ImGui.TableSetupColumn("Settings", ImGuiTableColumnFlags.NoReorder, 10);
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

                    var cursorPos = ImGui.GetCursorScreenPos();
                    var avail = ImGui.GetContentRegionAvail();

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

                    if (clicked && IsValidProject(projectPath))
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

        private void CreateProjectGUI()
        {
            DrawTitleBar("New Project");

            float labelWidth = 120.0f;
            float fieldSpacing = 8.0f;
            float buttonWidth = 80.0f;

            // Project Name
            ImGui.BeginGroup();
            ImGui.Text("Project Name");
          //  ImGui.SameLine(labelWidth + fieldSpacing);
            EditorGuiFieldsResolver.DrawStringField("##ProjectName", ref _projectCreateInfo.ProjectName);
            ImGui.EndGroup();

            ImGui.Spacing();

            // Project Directory
            ImGui.BeginGroup();
            ImGui.Text("Directory");
            ImGui.SameLine(labelWidth + fieldSpacing);
            ImGui.TextWrapped(_projectCreateInfo.ProjectRootDirectory);
          //  ImGui.SameLine();
            if (ImGui.Button("Select", new Vector2(buttonWidth, 0)))
            {
                if (EditorFileDialog.PickFolder(_projectCreateInfo.ProjectRootDirectory, out var selected))
                {
                    _projectCreateInfo.ProjectRootDirectory = selected;
                }
            }
            ImGui.EndGroup();

            ImGui.Spacing();

            ImGui.BeginGroup();
            ImGui.Text("intermediary directory");
            ImGui.SameLine(labelWidth + fieldSpacing);
            EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
            EditorGuiFieldsResolver.DrawBoolField("##Intermediary", ref _projectCreateInfo.UseIntermediaryDirectory);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("If enabled, the build will use an intermediate directory for the project.");
            }
            ImGui.EndGroup();

            ImGui.Spacing();

            ImGui.BeginDisabled(!_projectCreateInfo.IsValidProjectData());
            if (ImGui.Button("Create Project", new Vector2(ImGui.GetContentRegionAvail().X, 35)))
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