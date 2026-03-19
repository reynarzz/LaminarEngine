using Editor.Build;
using Editor.Cooker;
using Editor.Utils;
using Engine;
using Engine.Layers;
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
        private float _leftPaneWidth = 200f;
        private const float SplitterWidth = 6f;
        private float _splitterStartWidth;
        private float _splitterAccumulatedDelta;
        private List<AssetViewFileInfo> _assetsDirectories;
        private AssetViewFileInfo _selectedFile;
        private AssetViewFileInfo _currentDirFile;
        private AssetViewFileInfo _selectedFileRename;
        private AssetViewFileInfo _prevSelectedFileRename;

        private EObject _selectedAsset;
        private int _globalIndex = 0;
        public AssetsViewWindow() : base("Window/Assets View") { }
        private const string WINDOW_NAME = "Assets";
        protected override void OnOpen()
        {
            base.OnOpen();

            if (_assetsDirectories == null)
            {
                LoadDirectories();

                SelectFile(_assetsDirectories[0]);
            }

            EditorIOLayer.OnAssetDatabaseUpdated += LoadDirectories;
        }

        protected override void OnClose()
        {
            base.OnClose();
            EditorIOLayer.OnAssetDatabaseUpdated -= LoadDirectories;
        }

        private void LoadDirectories()
        {
            if (_assetsDirectories == null)
            {
                _assetsDirectories = new();
            }
            else
            {
                _assetsDirectories.Clear();
            }

            _globalIndex = 0;

            _clickedFile = null;
            DeselectFileRename();

            var selectedRelPath = string.Empty;
            var parentOfSelectedRelPath = string.Empty;

            if (_selectedFile != null)
            {
                selectedRelPath = _selectedFile.RelativePath;
                parentOfSelectedRelPath = _selectedFile.Parent?.RelativePath ?? string.Empty;
                _selectedFile = null;
            }
            _assetsDirectories.Add(EnumerateDirectoryGraphRecursive(Paths.GetAssetsFolderPath()));
            _assetsDirectories.Add(EnumerateDirectoryGraphRecursive(EditorPaths.CookerPaths.InternalAssetsPath));

            bool TrySelectFile(string relativePath)
            {
                if (!Selector.Transform)
                {
                    return SelectFile(relativePath);
                }
                return false;

            }
            TrySelectFile(selectedRelPath);
            if (!SelectDirectory(selectedRelPath))
            {
                TrySelectFile(parentOfSelectedRelPath);
                if (!SelectDirectory(parentOfSelectedRelPath))
                {
                    SelectDirectory(_assetsDirectories[0]);
                }
            }
        }

        public override void OnDraw()
        {
            if (!_selectedAsset)
            {
                _selectedAsset = null;
            }

            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            if (OnBeginWindow(WINDOW_NAME, ImGuiWindowFlags.Modal, true, new GlmNet.vec2(0, 5)))
            {
                var contentAvail = ImGui.GetContentRegionAvail();
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2());

                LeftPane(contentAvail);
                ImGui.PopStyleVar();

                ImGui.SameLine();
                Splitter(contentAvail);
                ImGui.SameLine();

                RightPane(contentAvail);
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
            DrawDirectoryNavigator();
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 2));

            var avail = ImGui.GetContentRegionAvail();
            ImGui.BeginChild("Files View", new Vector2(avail.X, avail.Y - 30), ImGuiChildFlags.AlwaysUseWindowPadding);
            ImGui.PopStyleVar();
            DrawWindowPopup();
            DrawRightFolderView(_currentDirFile);
            ImGui.EndChild();
            DrawRightFooter();


            ImGui.EndChild();
        }

        private void DrawWindowPopup()
        {
            //if (IsWindowHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            //{
            //    ImGui.OpenPopup("SceneViewWindowRight");
            //}
            //ImGui.BeginDisabled(_selectedFile.RelativePath.StartsWith(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME));
            //if (EditorImGui.BeginPopupContextItem("SceneViewWindowRight"))
            //{
            //    if (ImGui.MenuItem("Create Scene"))
            //    {
            //        var relativePathDir = GetFileRelativeFolderClean(_selectedFile);
            //        var defaultSceneName = "Scene";
            //        EditorAssetUtils.CreateScene(relativePathDir, defaultSceneName);

            //    }

            //    ImGui.EndPopup();
            //}
            //ImGui.EndDisabled();
        }
        private string GetFileRelativeFolderClean(AssetViewFileInfo file)
        {
            var dirName = "";
            if (File.Exists(file.AbsolutePath))
            {
                dirName = Path.GetDirectoryName(file.RelativePath);
            }
            else
            {
                dirName = file.RelativePath;
            }
            return Paths.ClearPathSeparation(dirName.Substring(dirName.IndexOf('/') + 1));
        }


        private AssetViewFileInfo GetSelectedFileParent(AssetViewFileInfo file, int index)
        {
            return GetParent(file, index);
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
        private void DrawDirectoryNavigator()
        {
            var path = _currentDirFile?.RelativePath ?? string.Empty;

            var split = path.Split('/');
            var length = _currentDirFile.Type == FileType.Asset ? split.Length - 1 : split.Length;
            for (int i = 0; i < length; i++)
            {
                var dir = split[i];
                var isLast = (i + 1 >= length);
                if (ImGui.Button(dir + $"##__Path__{i}") && !isLast)
                {
                    var parent = GetSelectedFileParent(_currentDirFile, i);
                    if (parent != null)
                    {
                        SelectDirectory(parent);
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
                ImGui.Text($"{_selectedFile.Filename}{_selectedFile.Extension} ({_selectedFile.AssetType})");
            }
            ImGui.EndChild();
        }

        private AssetViewFileInfo _clickedFile = null;
        private bool _isDoubleClick = false;

        private void DrawFileItemPopup(AssetViewFileInfo file)
        {
            const string popupId = "__FileItempopup__";
            if (ImGui.IsWindowHovered() && ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                DeselectFileRename();
                ImGui.SetWindowFocus(WINDOW_NAME);
                ImGui.OpenPopup(popupId);
            }
            ImGui.BeginDisabled(_currentDirFile.RelativePath.StartsWith(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME));

            if (ImGui.BeginPopup(popupId))
            {
                SelectFile(file);

                if (ImGui.MenuItem("Show in Explorer"))
                {
                    EditorFileDialog.ShowInExplorer(file.AbsolutePath);
                }
                if (ImGui.MenuItem("Create Scene"))
                {
                    var relativePathDir = GetFileRelativeFolderClean(file);
                    var writePath = EditorAssetUtils.RemoveRootAssetFolder(relativePathDir);
                    if (!string.IsNullOrEmpty(writePath))
                    {
                        writePath += "/";
                    }

                    var defaultSceneName = "Scene";
                    writePath += defaultSceneName + EditorPaths.SCENE_FILE_EXTENSION;
                    var relativeScenePath = Paths.ASSETS_FOLDER_NAME + "/" + writePath;
                    CreateFile(relativeScenePath, _ => EditorAssetUtils.CreateScene(writePath));
                }
                if (ImGui.MenuItem("Rename"))
                {
                    SelectRename(file);
                }
                if (ImGui.MenuItem("Create Folder"))
                {
                    var folder = GetFileRelativeFolderClean(file);
                    var relativePathDir = EditorAssetUtils.RemoveRootAssetFolder(folder);

                    if (!string.IsNullOrEmpty(relativePathDir))
                    {
                        relativePathDir += "/";
                    }

                    var tempDirName = "NewFolder";
                    relativePathDir += tempDirName;

                    CreateFile(relativePathDir, EditorAssetUtils.CreateDirectory);
                }
                if (ImGui.MenuItem("Delete"))
                {
                    string messageTitle = file.Type == FileType.Asset ? $"Delete selected asset?" : $"Delete Directory?";
                    var assetType = file.Type == FileType.Asset ? $"({file.AssetType})" : string.Empty;

                    string messageText = $"{file.RelativePath} {assetType}\n\nCannot undo.";
                    var result = EditorFileDialog.MessageBox(messageTitle, messageText, MessageBoxChoice.Yes_No, MessageBoxIcon.Warning);

                    if (result == MessageBoxButton.Yes)
                    {
                        var relativePathDir = EditorAssetUtils.RemoveRootAssetFolder(file.RelativePath);

                        if (!string.IsNullOrEmpty(relativePathDir))
                        {
                            EditorAssetUtils.DeleAsset(relativePathDir);
                            LoadDirectories();

                            if (file.Type == FileType.Asset)
                            {
                                Assets.DestroyAsset(file.RefId);
                            }

                            if (file.Type == FileType.Directory && _currentDirFile == file)
                            {
                                _currentDirFile = file?.Parent ?? _assetsDirectories[0];
                            }
                        }
                    }

                }
                ImGui.EndPopup();
            }
            ImGui.EndDisabled();
        }

        private void CreateFile(string relativePath, Action<string> createCallback)
        {
            createCallback?.Invoke(relativePath);

            LoadDirectories();

            // After this point the old file was destroyed by the update so we need to find the new file with the same path.
            var newFile = FindFile(Paths.ASSETS_FOLDER_NAME + "/" + relativePath);
            SelectRename(newFile);
        }
        private void SelectRename(AssetViewFileInfo file)
        {
            DeselectFileRename();
            _selectedFileRename = file;
            ImGui.SetWindowFocus(WINDOW_NAME);
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
                    var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.DefaultOpen;
                    if (file.Type == FileType.Directory || (file.Type == FileType.Asset && file.FilesCount == 0))
                    {
                        flags |= ImGuiTreeNodeFlags.Leaf;
                    }

                    if (file != _selectedFileRename)
                    {
                        flags |= ImGuiTreeNodeFlags.SpanFullWidth;
                    }
                    var cursorPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPosX(cursorPos.X - 10);
                    var open = ImGui.TreeNodeEx($"##_PREVIEW_{file.AbsolutePath}_{i}", flags);
                    DrawFileItemPopup(file);

                    var isHover = ImGui.IsItemHovered();
                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        _clickedFile = file;

                        if (_selectedFileRename != file)
                        {
                            DeselectFileRename();
                        }
                    }

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
                    EditorImGui.DragAndDrop.ItemDragReference(file.Filename, image, EditorImGui.DragAndDrop.PAYLOAD_ID_EOBJECT, null, file.AssetType, file.RefId);

                    EditorImGui.Image(image, new vec2(16, 16));
                    ImGui.SameLine();
                    cursorPos = ImGui.GetCursorPos();

                    if (_selectedFileRename == file)
                    {
                        bool firstFrameRename = false;
                        if (_prevSelectedFileRename != _selectedFileRename)
                        {
                            _prevSelectedFileRename = _selectedFileRename;
                            firstFrameRename = true;
                        }

                        var changedFilename = file.Filename;
                        const string renameId = "Rename_FILE";
                        ImGui.SetCursorPosX(cursorPos.X - 8);

                        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 0));
                        var renameChange = EditorGuiFieldsResolver.DrawStringField($"##{renameId}", ref changedFilename, 0, true, firstFrameRename);
                        var isContinueKeyPressed = (IsWindowFocused || IsWindowHovered || ImGui.IsItemFocused()) && (ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.Escape));

                        // Cannot update, user only added white spaces.
                        if ((renameChange || isContinueKeyPressed) && changedFilename.Trim().Equals(file.Filename))
                        {
                            renameChange = false;
                            isContinueKeyPressed = false;
                            DeselectFileRename();
                        }
                        ImGui.PopStyleVar();
                        if (renameChange || isContinueKeyPressed)
                        {
                            if (renameChange && !string.IsNullOrEmpty(changedFilename))
                            {
                                changedFilename = changedFilename.Trim();
                                var newRelativePath = string.Empty;

                                var relFolder = GetFileRelativeFolderClean(file);
                                if (file.Type == FileType.Asset)
                                {
                                    newRelativePath = EditorAssetUtils.RemoveRootAssetFolder(relFolder + "/" + changedFilename + file.Extension);
                                }
                                else
                                {
                                    var index = relFolder.LastIndexOf('/');

                                    if (index < 0)
                                    {
                                        newRelativePath = changedFilename;
                                    }
                                    else
                                    {
                                        newRelativePath = relFolder.Substring(0, index) + "/" + changedFilename;
                                    }
                                }

                                if (EditorAssetUtils.MoveAsset(EditorAssetUtils.RemoveRootAssetFolder(file.RelativePath), newRelativePath, overwrite: false))
                                {
                                    var oldFilename = file.Filename;
                                    file.Filename = changedFilename;
                                    file.AbsolutePath = Paths.ClearPathSeparation(Path.GetDirectoryName(file.AbsolutePath)) + "/" + changedFilename + file.Extension;
                                    file.RelativePath = newRelativePath;
                                }
                                else
                                {
                                    // Show popup with this message and a 'ok' button, renaming will not be cancelled.
                                    Debug.Error("There is already a file with this name in this directory.");
                                    DeselectFileRename();
                                }

                                LoadDirectories();
                                ImGui.SetScrollHereY();
                            }
                            else
                            {
                                DeselectFileRename();
                            }
                        }
                        //else if (!IsWindowFocused)
                        //{
                        //    _selectedFileRename = null;
                        //}
                    }
                    else
                    {
                        ImGui.SetCursorPosX(cursorPos.X - 4);
                        ImGui.Text(file.Filename);
                    }

                    var isDoubleClick = ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);
                    var isMouseUp = ImGui.IsMouseReleased(ImGuiMouseButton.Left);
                    if (isDoubleClick && isHover && _clickedFile == file)
                    {
                        _isDoubleClick = true;
                    }

                    // Select an item.
                    if (isHover && isMouseUp && _clickedFile == file)
                    {
                        SelectFile(file);

                        if (_isDoubleClick)
                        {
                            if (file.Type == FileType.Asset)
                            {
                                OpenAsset(file);
                            }
                            else
                            {
                                SelectDirectory(file);
                            }
                        }

                        _isDoubleClick = false;
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

        private void DeselectFileRename()
        {
            _selectedFileRename = null;
            _prevSelectedFileRename = null;
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
                case AssetType.AnimatorController:
                    break;
                case AssetType.Material:
                    break;
                case AssetType.Scene:
                    {
                        if (!EditorSystem.Save.IsAnyAssetDirty(AssetType.Scene))
                        {
                            SceneManager.LoadScene(file.RefId);
                        }
                        else
                        {
                            var dirtyScenes = new List<Scene>();
                            int index = 0;
                            var dirtySceneNames = new StringBuilder();
                            for (int i = 1; i < SceneManager.Scenes.Count; i++)
                            {
                                var scene = SceneManager.Scenes[i];

                                if (EditorSystem.Save.IsDirty(scene.GetID()))
                                {
                                    dirtyScenes.Add(scene);
                                    dirtySceneNames.AppendLine(scene.Name + EditorPaths.SCENE_FILE_EXTENSION);
                                }
                            }

                            string messageTitle = "Open scene.";
                            string messageText = $"Current opened scenes have unsaved changes:\n\n{dirtySceneNames.ToString()}\nSave current, and then load '{file.Filename}{EditorPaths.SCENE_FILE_EXTENSION}'?";
                            var result = EditorFileDialog.MessageBox(messageTitle, messageText, MessageBoxChoice.Yes_No_Cancel, MessageBoxIcon.Warning);

                            if (result == MessageBoxButton.Yes)
                            {
                                EditorSystem.Save.SaveAll();
                                SceneManager.LoadScene(file.RefId);
                            }
                            else if (result == MessageBoxButton.No)
                            {
                                foreach (var dirtyScene in dirtyScenes)
                                {
                                    EditorSystem.Save.RemoveDirty(dirtyScene.GetID());
                                }
                                SceneManager.LoadScene(file.RefId);
                            }
                        }
                    }
                    break;
                case AssetType.Tilemap:
                    break;
            }
        }
        private bool SelectDirectory(string relativePath)
        {
            return SelectFileInfo(relativePath, SelectDirectory);
        }

        private void SelectDirectory(AssetViewFileInfo directory)
        {
            _currentDirFile = directory.Type == FileType.Asset ? directory?.Parent : directory;
        }

        private bool SelectFile(string relativePath)
        {
            return SelectFileInfo(relativePath, SelectFile);
        }

        private AssetViewFileInfo FindFile(string relativePath)
        {
            return FindFile(relativePath, _assetsDirectories[0]);
        }

        private AssetViewFileInfo FindFile(string relativePath, AssetViewFileInfo rootFile)
        {
            if (rootFile.RelativePath.Equals(relativePath, StringComparison.OrdinalIgnoreCase))
            {
                return rootFile;
            }
            else if (rootFile.Children != null)
            {
                for (int i = 0; i < rootFile.Children.Count; i++)
                {
                    var result = FindFile(relativePath, rootFile.Children[i]);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
        private bool SelectFileInfo(string relativePath, Action<AssetViewFileInfo> selectCallBack)
        {
            foreach (var dir in _assetsDirectories)
            {
                foreach (var file in dir.Children)
                {
                    var fileFound = FindFile(relativePath, file);

                    if (fileFound != null)
                    {
                        selectCallBack(fileFound);
                        return true;
                    }
                }
            }

            return false;
        }

        private void SelectFile(AssetViewFileInfo file)
        {
            _selectedFile = file;
            DeselectFileRename();
            if (file != null && file.Type == FileType.Asset)
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
                SelectFile(default(AssetViewFileInfo));
                SelectDirectory(fileRoot);
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
            directoryInfo.GlobalIndex = _globalIndex++;
            //directoryInfo.RefId = Guid.NewGuid(); // TODO: folders guids should persist.

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
                    GlobalIndex = _globalIndex++,
                    RefId = refId,
                };
                fileInfo.Children = GetAssetChildren(fileInfo, assetType, file, ref refId);

                directoryInfo.Children.Add(fileInfo);
            }

            directoryInfo.FilesCount = filesCount;

            return directoryInfo;
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
                case AssetType.AnimatorController:
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
            public int GlobalIndex { get; set; }
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