using Editor.Build;
using Editor.Cooker;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class AssetsViewWindow : EditorWindow
    {
        private float _leftPaneWidth = 150f;
        private const float SplitterWidth = 6f;
        private float _splitterStartWidth;
        private float _splitterAccumulatedDelta;

        public AssetsViewWindow() : base("Window/Assets View")
        {
        }

        public override void OnDraw()
        {
            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            if (OnBeginWindow("Assets View", ImGuiWindowFlags.Modal, true, new GlmNet.vec2()))
            {
                var contentAvail = ImGui.GetContentRegionAvail();
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2());

                LeftPane(contentAvail);

                ImGui.SameLine();
                Splitter(contentAvail);
                ImGui.SameLine();

                RightPane(contentAvail);
                ImGui.PopStyleVar();
            }

            OnEndWindow();
            ImGui.EndDisabled();

        }

        private void Splitter(Vector2 contentAvail)
        {
            var splitterSize = new Vector2(SplitterWidth, Mathf.Clamp(contentAvail.Y, 1, contentAvail.Y + 1));
            ImGui.InvisibleButton("##Splitter", splitterSize);

            bool hovered = ImGui.IsItemHovered();
            bool active = ImGui.IsItemActive();

            if (hovered || active)
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
            }

            if (ImGui.IsItemActivated())
            {
                _splitterStartWidth = _leftPaneWidth;
                _splitterAccumulatedDelta = 0.0f;
            }

            if (active)
            {
                _splitterAccumulatedDelta += ImGui.GetIO().MouseDelta.X;

                float newWidth = _splitterStartWidth + _splitterAccumulatedDelta;
                _leftPaneWidth = Math.Clamp(newWidth, 80f, contentAvail.X - 80f);
            }

            var drawList = ImGui.GetWindowDrawList();
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();

            uint splitterColor = ImGui.GetColorU32(ImGuiCol.Separator);
            if (hovered)
            {
                splitterColor = ImGui.GetColorU32(ImGuiCol.SeparatorHovered);
            }
            if (active)
            {
                splitterColor = ImGui.GetColorU32(ImGuiCol.SeparatorActive);
            }

            float centerX = (min.X + max.X) * 0.5f;
            drawList.AddLine(new Vector2(centerX, min.Y), new Vector2(centerX, max.Y), splitterColor, 1.0f);
        }

        private void LeftPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("Left Pane", new Vector2(_leftPaneWidth, contentAvail.Y), ImGuiChildFlags.None);

            foreach (var assetDirRoot in _assetsDirectories)
            {
                DrawFolderPicker(assetDirRoot);
            }

            ImGui.EndChild();
        }


        private void RightPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("Right Pane", new Vector2(0, contentAvail.Y), ImGuiChildFlags.None, ImGuiWindowFlags.NoMove |
                                                                                                 ImGuiWindowFlags.NoDocking);

            ImGui.EndChild();
        }

        private enum FileType
        {
            None,
            Directory,
            Asset
        }

        private class AssetViewFileInfo
        {
            public FileType Type { get; set; } = FileType.None;
            public AssetType AssetType { get; set; } = AssetType.Invalid;
            public Guid RefId { get; set; }
            public string RelativePath { get; set; }
            public string AbsolutePath { get; set; }
            public string Filename { get; set; }
            public string Ext { get; set; }
            public int FilesCount { get; set; }
            public int DirectoriesCount { get; set; }
            public AssetViewFileInfo Parent { get; set; }
            public List<AssetViewFileInfo> Children { get; set; } = new();
        }

        private List<AssetViewFileInfo> EnumerateDirectoryGraphRecursive(string root)
        {
            var files = new List<AssetViewFileInfo>();

            var rootDirectories = Directory.GetDirectories(root);

            for (int i = 0; i < rootDirectories.Length; i++)
            {
                files.Add(Enumerate(null, rootDirectories[i]));
            }

            AssetViewFileInfo Enumerate(AssetViewFileInfo parent, string rootDirectory)
            {
                var directoryInfo = new AssetViewFileInfo();
                rootDirectory = Paths.ClearPathSeparation(rootDirectory);

                var folderName = rootDirectory.Substring(rootDirectory.LastIndexOf('/') + 1);
                directoryInfo.AbsolutePath = rootDirectory;
                directoryInfo.RelativePath = EditorPaths.GetRelativeAssetPathSafe(rootDirectory);
                directoryInfo.Type = FileType.Directory;
                directoryInfo.Filename = folderName;

                if (parent != null)
                {
                    directoryInfo.Parent = parent;
                }

                var folders = Directory.GetDirectories(rootDirectory);
                var files = Directory.GetFiles(rootDirectory);

                directoryInfo.DirectoriesCount = folders.Length;
                // Enumerate the sub directories.
                for (int i = 0; i < folders.Length; i++)
                {
                    var folder = Paths.ClearPathSeparation(folders[i]);
                    directoryInfo.Children.Add(Enumerate(directoryInfo, folder));
                }

                // Enumerate the files.
                int filesCount = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    var file = Paths.ClearPathSeparation(files[i]);

                    var ext = Path.GetExtension(file);
                    var assetType = AssetsCooker.GetAssetTypeFromExtension(ext);
                    if (assetType == AssetType.Invalid)
                        continue;

                    filesCount++;

                    EditorAssetUtils.GetMetaGuid(file, out var refId);

                    var fileInfo = new AssetViewFileInfo()
                    {
                        Type = FileType.Asset,
                        AbsolutePath = file,
                        RelativePath = EditorPaths.GetRelativeAssetPathSafe(file),
                        Parent = directoryInfo,
                        Filename = Path.GetFileNameWithoutExtension(file),
                        Ext = ext,
                        AssetType = assetType,
                        RefId = refId,
                    };
                    fileInfo.Children = GetAssetChildren(fileInfo, assetType, file, ref refId);

                    directoryInfo.Children.Add(fileInfo);
                }

                directoryInfo.FilesCount = filesCount;

                return directoryInfo;
            }
            return files;

        }

        private List<List<AssetViewFileInfo>> _assetsDirectories;
        protected override void OnOpen()
        {
            base.OnOpen();

            if (_assetsDirectories == null)
            {
                _assetsDirectories = new();

                _assetsDirectories.Clear();
                _assetsDirectories.Add(EnumerateDirectoryGraphRecursive(Paths.GetAssetsFolderPath()));
            }
        }

        private List<AssetViewFileInfo> GetAssetChildren(AssetViewFileInfo parent, AssetType assetType, string absoluteFilePath, ref Guid refId)
        {
            var children = new List<AssetViewFileInfo>();
            switch (assetType)
            {
                case AssetType.Texture:
                    {
                        // Check if the texture has atlas, if so, enumerate the sprites.
                        var textureMeta = EditorAssetUtils.GetMetaFromAbsolutePath(absoluteFilePath, assetType) as TextureMetaFile;

                        if (textureMeta.Config.IsAtlas)
                        {
                            for (int i = 0; i < textureMeta.AtlasData.ChunksCount; i++)
                            {

                                //children.Add(new AssetViewFileInfo() {  RefId = });
                            }
                        }
                    }
                    break;
                case AssetType.Audio:
                    break;
                case AssetType.Text:
                    break;
                case AssetType.Shader:
                    break;
                case AssetType.Font:
                    break;
                case AssetType.AnimationClip:
                    break;
                case AssetType.AnimationController:
                    break;
                case AssetType.Material:
                    break;
                case AssetType.ShaderV2:
                    break;
                case AssetType.Scene:
                    break;
                case AssetType.Tilemap:
                    break;
                default:
                    break;
            }

            return children;
        }

        private void DrawFolderPicker(List<AssetViewFileInfo> filesRoot)
        {
            for (int i = 0; i < filesRoot.Count; i++)
            {
                var file = filesRoot[i];

                if (file.Type != FileType.Directory)
                {
                    continue;
                }

                var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;
                if (file.Children == null || file.DirectoriesCount == 0)
                {
                    flags |= ImGuiTreeNodeFlags.Leaf;
                }
                var open = ImGui.TreeNodeEx($"{file.Filename}##{file.AbsolutePath}", flags);

                if (open)
                {
                    DrawFolderPicker(file.Children);
                    ImGui.TreePop();
                }
            }
        }
    }
}