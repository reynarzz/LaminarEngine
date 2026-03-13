using Editor.Build;
using Editor.Cooker;
using Editor.Utils;
using Engine;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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
        private List<AssetViewFileInfo> _assetsDirectories;
        private AssetViewFileInfo _selectedFile;
        private EObject _selectedAsset;

        public AssetsViewWindow() : base("Window/Assets View") { }

        protected override void OnOpen()
        {
            base.OnOpen();

            if (_assetsDirectories == null)
            {
                _assetsDirectories = new();

                _assetsDirectories.Clear();
                _assetsDirectories.Add(EnumerateDirectoryGraphRecursive(Paths.GetAssetsFolderPath()));
                _assetsDirectories.Add(EnumerateDirectoryGraphRecursive(EditorPaths.CookerPaths.InternalAssetsPath));

                SelectFile(_assetsDirectories[0]);
            }
        }

        public override void OnDraw()
        {
            if (!_selectedAsset)
            {
                _selectedAsset = null;
            }

            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            if (OnBeginWindow("Assets", ImGuiWindowFlags.Modal, true, new GlmNet.vec2(0, 5)))
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
                DrawFolderPicker(assetDirRoot, true);
            }

            ImGui.EndChild();
        }


        private void RightPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("Right Pane", new Vector2(0, contentAvail.Y), ImGuiChildFlags.None, ImGuiWindowFlags.NoMove |
                                                                                                 ImGuiWindowFlags.NoDocking);
            DrawRightDirectoryPath();
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 2));

            var avail = ImGui.GetContentRegionAvail();
            ImGui.BeginChild("Files View", new Vector2(avail.X, avail.Y - 30), ImGuiChildFlags.AlwaysUseWindowPadding);
            DrawRightFolderView(_selectedFile);
            ImGui.EndChild();
            ImGui.PopStyleVar();
            DrawRightFooter();


            ImGui.EndChild();
        }

        private AssetViewFileInfo GetSelectedFileParent(int index)
        {
            return GetParent(_selectedFile, index);
        }

        private AssetViewFileInfo GetParent(AssetViewFileInfo file, int index)
        {
            var parent = file.Parent;

            if (parent == null)
            {
                return null;
            }

            var st = new List<AssetViewFileInfo>();
            while (parent != null)
            {
                st.Add(parent);
                parent = parent.Parent;
            }

            if (st.Count == 1)
            {
                return st[0];
            }

            return st[^(index + 1)];
        }

        private void DrawRightDirectoryPath()
        {
            var path = _selectedFile?.RelativePath ?? string.Empty;

            var split = path.Split('/');
            var length = _selectedFile.Type == FileType.Asset ? split.Length - 1 : split.Length;
            for (int i = 0; i < length; i++)
            {
                var dir = split[i];
                var isLast = (i + 1 >= length);
                if (ImGui.Button(dir + $"##__Path__{i}") && !isLast)
                {
                    var parent = GetSelectedFileParent(i);
                    if (parent != null)
                    {
                        SelectFile(parent);
                    }
                }

                if (!isLast)
                {
                    ImGui.SameLine();
                    ImGui.Text(">");
                    ImGui.SameLine();
                }
            }

        }

        private void DrawRightFooter()
        {
            ImGui.BeginChild("Right footer");

            if (_selectedFile != null && _selectedFile.Type == FileType.Asset)
            {
                EditorImGui.Image(EditorTextureDatabase.GetIconImGui(_selectedFile.AssetType), new vec2(16, 16));
                ImGui.SameLine();
                ImGui.Text($"{_selectedFile.Filename}{_selectedFile.Extension}");
            }
            ImGui.EndChild();
        }

        private void DrawRightFolderView(AssetViewFileInfo fileRoot)
        {
            if (fileRoot != null)
            {
                if (fileRoot.Type == FileType.Asset)
                {
                    fileRoot = fileRoot.Parent;
                }
                for (int i = 0; i < fileRoot.Children.Count; i++)
                {
                    var file = fileRoot.Children[i];
                    var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.DefaultOpen;
                    if (file.Type == FileType.Directory || (file.Type == FileType.Asset && file.FilesCount == 0))
                    {
                        flags |= ImGuiTreeNodeFlags.Leaf;
                    }
                    var cursorPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(cursorPos.X - 10);
                    var open = ImGui.TreeNodeEx($"##_PREVIEW_{file.AbsolutePath}_{i}", flags);
                    var isClicked = ImGui.IsItemClicked();
                    ImGui.SameLine();
                    cursorPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(cursorPos.X - 14);
                    nint image = 0;
                    if (file.Type == FileType.Directory)
                    {
                        image = EditorTextureDatabase.GetIconImGui(GetFolderIcon(file, false));
                    }
                    else if (file.Type == FileType.Asset)
                    {
                        image = EditorTextureDatabase.GetIconImGui(file.AssetType);
                    }
                    EditorImGui.Image(image, new GlmNet.vec2(16, 16));
                    ImGui.SameLine();
                    cursorPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(cursorPos.X - 4);
                    ImGui.Text(file.Filename);

                    if (isClicked)
                    {
                        var isDoubleClick = ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);

                        if (file.Type == FileType.Asset || isDoubleClick)
                        {
                            SelectFile(file);

                            if (isDoubleClick)
                            {
                                OpenAsset(file);
                            }
                        }
                        else
                        {
                            SelectFile(file.Parent);
                        }
                    }

                    if (open)
                    {
                        if (file.Type == FileType.Asset)
                        {
                            for (int j = 0; j < file.Children.Count; j++)
                            {
                                var child = file.Children[j];
                                ImGui.Text(child.Filename);
                            }
                        }

                        ImGui.TreePop();
                    }
                }
            }
        }

        private void OpenAsset(AssetViewFileInfo file)
        {
            // TODO: Move this to another class

            switch (file.AssetType)
            {
                case AssetType.Invalid:
                    break;
                case AssetType.Texture:
                case AssetType.Audio:
                case AssetType.Text:
                case AssetType.ShaderV2:
                case AssetType.Shader:
                case AssetType.Script:
                    EditorFileUtils.OpenFile(file.AbsolutePath);
                    break;
                case AssetType.Font:
                    break;
                case AssetType.AnimationClip:
                    break;
                case AssetType.AnimationController:
                    break;
                case AssetType.Material:
                    break;
                case AssetType.Scene:
                    SceneManager.LoadScene(file.RefId);
                    break;
                case AssetType.Tilemap:
                    break;
            }
        }

        private void SelectFile(AssetViewFileInfo file)
        {
            _selectedFile = file;

            if (file.Type == FileType.Asset)
            {
                // TODO: improve selector so we don't have to load the entire asset.
                if (file.AssetType == AssetType.Texture)
                {
                    _selectedAsset = (Assets.GetAssetFromGuid(file.RefId) as TextureAsset).Texture;
                }
                else
                {
                    _selectedAsset = Assets.GetAssetFromGuid(file.RefId);
                }
            }
            else
            {
                _selectedAsset = null;
            }

            Selector.Selected = _selectedAsset;
        }

        private void DrawFolderPicker(AssetViewFileInfo fileRoot, bool isDefaultOpened = false)
        {
            var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth | (isDefaultOpened ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None);
            if (fileRoot.Children == null || fileRoot.DirectoriesCount == 0)
            {
                flags |= ImGuiTreeNodeFlags.Leaf;
            }
            var open = ImGui.TreeNodeEx($"##{fileRoot.AbsolutePath}", flags);
            var isClicked = ImGui.IsItemClicked();

            ImGui.SameLine();
            var cursorPos = ImGui.GetCursorPos();
            ImGui.SetCursorPosX(cursorPos.X - 14);
            EditorImGui.Image(EditorTextureDatabase.GetIconImGui(GetFolderIcon(fileRoot, open)), new GlmNet.vec2(16, 16));
            ImGui.SameLine();
            cursorPos = ImGui.GetCursorPos();
            ImGui.SetCursorPosX(cursorPos.X - 4);
            ImGui.Text(fileRoot.Filename);

            if (isClicked)
            {
                SelectFile(fileRoot);
            }
            if (open)
            {
                for (int i = 0; i < fileRoot.Children.Count; i++)
                {
                    if (fileRoot.Children[i].Type != FileType.Directory)
                    {
                        continue;
                    }
                    DrawFolderPicker(fileRoot.Children[i]);
                }
                ImGui.TreePop();
            }
        }

        private EditorIcon GetFolderIcon(AssetViewFileInfo file, bool isOpen)
        {
            var folderIcon = EditorIcon.FolderClosedEmpty;

            if (isOpen)
            {
                folderIcon = file.DirectoriesCount > 0 ? EditorIcon.FolderOpenFilled : EditorIcon.FolderOpenEmpty;
            }
            else
            {
                folderIcon = file.Children.Count > 0 ? EditorIcon.FolderClosedFilled : EditorIcon.FolderClosedEmpty;
            }
            return folderIcon;
        }

        private bool IsFileSelected(AssetViewFileInfo file)
        {
            return file == _selectedFile;
        }

        private AssetViewFileInfo EnumerateDirectoryGraphRecursive(string root)
        {
            return EnumerateDirectoryGraphRecursive(null, root);
        }
        private AssetViewFileInfo EnumerateDirectoryGraphRecursive(AssetViewFileInfo parent, string rootDirectory)
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
                directoryInfo.Children.Add(EnumerateDirectoryGraphRecursive(directoryInfo, folder));
            }

            // Enumerate the files.
            int filesCount = 0;
            for (int i = 0; i < files.Length; i++)
            {
                var file = Paths.ClearPathSeparation(files[i]);

                var ext = Path.GetExtension(file);
                var assetType = AssetType.Invalid;

                if (ext.EndsWith(".cs"))
                {
                    assetType = AssetType.Script;
                }
                else
                {
                    assetType = AssetsCooker.GetAssetTypeFromExtension(ext);
                }

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
                    Extension = ext,
                    AssetType = assetType,
                    RefId = refId,
                };
                fileInfo.Children = GetAssetChildren(fileInfo, assetType, file, ref refId);

                directoryInfo.Children.Add(fileInfo);
            }

            directoryInfo.FilesCount = filesCount;

            return directoryInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ReferenceDragAndDropPayload
        {
            public ulong A;
            public ulong B;
            public Guid RefId
            {
                get
                {
                    Span<ulong> parts = stackalloc ulong[2];
                    parts[0] = A;
                    parts[1] = B;

                    return new Guid(MemoryMarshal.AsBytes(parts));
                }
            }
        }
        private ReferenceDragAndDropPayload _dragAndDropPayload;
        private void DragAndDrop()
        {
            unsafe
            {
                int dragValue = 10;

                ImGui.Button("Drag Source");

                if (ImGui.BeginDragDropSource())
                {
                    Guid refId = Guid.NewGuid();

                    Span<byte> guidBytes = stackalloc byte[sizeof(Guid)];
                    refId.TryWriteBytes(guidBytes);

                    _dragAndDropPayload = new ReferenceDragAndDropPayload()
                    {
                        A = BitConverter.ToUInt64(guidBytes[..8]),
                        B = BitConverter.ToUInt64(guidBytes[8..])
                    };

                    fixed(void* ptr = &_dragAndDropPayload)
                    {
                        ImGui.SetDragDropPayload("ASSETS_VIEW_VALUE", new IntPtr(ptr), (uint)sizeof(ReferenceDragAndDropPayload));
                    }
                    ImGui.Text($"Value: {dragValue}");
                    ImGui.EndDragDropSource();
                }

                ImGui.Button("Drop Target");

                if (ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload("ASSETS_VIEW_VALUE");

                    if (payload.NativePtr != null)
                    {
                        int dropped = *(int*)payload.Data;
                        dragValue = dropped;
                    }

                    ImGui.EndDragDropTarget();
                }
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
            public string Extension { get; set; }
            public int FilesCount { get; set; }
            public int DirectoriesCount { get; set; }
            public AssetViewFileInfo Parent { get; set; }
            public List<AssetViewFileInfo> Children { get; set; } = new();
        }
    }
}