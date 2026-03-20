using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Serialization;
using GlmNet;
using ImGuiNET;
using System;
using System.Numerics;

namespace Editor.Views
{
    internal class SceneGraphWindow : EditorWindow
    {
        private Guid? _pressedActorId;
        private Guid _prevSelectedActorIdGraph;
        private Guid _prevDragActorId;
        private readonly HashSet<Guid> _expandParents = new();
        private Guid _firstTimeSelectedActorId = default;
        private readonly WeakReference<Actor> _currentDraggingActor = new WeakReference<Actor>(null);
        private bool _isPendingToDropActor = false;
        private const string WINDOW_NAME = "Scene graph###Scene graph";
        private const string WINDOW_DIRTY_NAME = "Scene graph*###Scene graph";
        private bool _wasDragging = false;

        private struct SelectionInfo
        {
            public bool Expanded { get; set; }
        }

        private readonly Dictionary<Guid, SelectionInfo> _currentSelectionGraph = new();
        public SceneGraphWindow() : base("Window/Scene Graph")
        {
        }

        private string GetWindowName()
        {
            return EditorSystem.Save.IsAnyAssetDirty(AssetType.Scene) ? WINDOW_DIRTY_NAME : WINDOW_NAME;
        }
        public override void OnDraw()
        {
            if (OnBeginWindow(GetWindowName(), ImGuiWindowFlags.None, true, new GlmNet.vec2()))
            {
                var screenCursor = ImGui.GetCursorScreenPos();

                var isMouseDrag = ImGui.IsMouseDragging(ImGuiMouseButton.Left);
                if (isMouseDrag || _wasDragging)
                {
                    _wasDragging = isMouseDrag;
                    ImGui.InvisibleButton("SCENE_GRAPH_DropArea", ImGui.GetContentRegionAvail());
                    DropActorHandle(null, dropActor =>
                    {
                        dropActor.Transform.Parent = null;
                    });

                }
                else if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    Debug.Log("Window clicked");
                }

                ImGui.SetCursorScreenPos(screenCursor);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2());

                PopulateDragGraph();
                if (IsWindowFocused && ImGui.IsKeyPressed(ImGuiKey.Escape))
                {
                    Selector.Selected = null;
                }
                for (int i = SceneManager.Scenes.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && (!Application.IsInPlayMode || SceneManager.Scenes[0].RootActors.Count == 0))
                    {
                        continue;
                    }
                    var scene = SceneManager.Scenes[i];
                    ImGui.PushID(scene.GetID().ToString());
                    ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.DefaultOpen;

                    bool open = ImGui.TreeNodeEx("##scene_node", flags);
                    DropActorHandle(null, dropActor =>
                    {
                        dropActor.Transform.Parent = null; // Remove current parent, this makes it root of scene
                        dropActor.Scene.UnregisterRootActor(dropActor); // Remove from the old scene's root.
                        scene.RegisterRootActor(dropActor); // add actor to the new scene's root.
                    });

                    bool isClicked = ImGui.IsItemClicked();

                    ImGui.SameLine();
                    var iconCursorX = ImGui.GetCursorPosX();
                    var iconOffset = 8;
                    ImGui.SetCursorPosX(iconCursorX - iconOffset);
                    EditorImGui.ImageFromIcon(EditorIcon.Scene, new GlmNet.vec2(18, 18));

                    ImGui.SameLine();
                    iconCursorX = ImGui.GetCursorPosX();
                    ImGui.SetCursorPosX(iconCursorX + iconOffset - 4);
                    ImGui.TextUnformatted(scene.Name);

                    if (EditorImGui.BeginPopupContextItem("SceneContext"))
                    {
                        var isValidScene = i > 0;
                        ImGui.BeginDisabled(!isValidScene);
                        if (ImGui.MenuItem("Reload Scene"))
                        {
                            void ReloadScene()
                            {
                                Guid? selectedActor = null;
                                if (Selector.Transform)
                                {
                                    selectedActor = Selector.Transform.Actor.GetID();
                                }
                                SceneManager.ReloadScene(scene.GetID());
                                if (selectedActor != null)
                                {
                                    Selector.Selected = SceneManager.FindActorByID(selectedActor.Value);
                                }
                            }

                            if (!EditorSystem.Save.IsDirty(scene.GetID()))
                            {
                                ReloadScene();
                            }
                            else
                            {
                                string messageTitle = "Reload Scene";
                                string messageText = $"Scene '{scene.Name}' contains unsaved changes, still reload?";
                                var result = EditorFileDialog.MessageBox(messageTitle, messageText, MessageBoxChoice.Yes_No, MessageBoxIcon.Warning);
                                if (result == MessageBoxButton.Yes)
                                {
                                    EditorSystem.Save.RemoveDirty(scene.GetID());
                                    ReloadScene();
                                }
                            }
                        }
                        ImGui.BeginDisabled(i <= 1);

                        if (ImGui.MenuItem("Unload Scene"))
                        {
                            if (!EditorSystem.Save.IsDirty(scene.GetID()))
                            {
                                SceneManager.UnloadScene(scene);
                            }
                            else
                            {
                                string messageTitle = "Unload Scene";
                                string messageText = $"Scene '{scene.Name}' contains unsaved changes, still unload?";
                                var result = EditorFileDialog.MessageBox(messageTitle, messageText, MessageBoxChoice.Yes_No, MessageBoxIcon.Warning);

                                if (result == MessageBoxButton.Yes)
                                {
                                    SceneManager.UnloadScene(scene);
                                }
                            }
                            ImGui.EndPopup();

                            if (open)
                            {
                                ImGui.TreePop();
                            }
                            ImGui.PopID();
                            ImGui.EndDisabled();
                            ImGui.EndDisabled();
                            break;
                        }
                        ImGui.EndDisabled();

                        ImGui.EndDisabled();

                        if (ImGui.MenuItem("Create Actor"))
                        {
                            var newActor = new Actor("Actor", Guid.NewGuid(), scene);
                            EditorSystem.Save.MarkDirty(newActor);
                            SetSelectedActorParentGraph(newActor);
                        }


                        ImGui.EndPopup();
                    }

                    if (open)
                    {
                        if (scene.RootActors.Count > 0)
                        {
                            ImGui.Dummy(0, 4);
                            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 15.5f);

                            SetSelectedActorParentGraph(Selector.Selected as Actor);

                            for (int j = 0; j < scene.RootActors.Count; j++)
                            {
                                DrawActor(scene.RootActors[j]);
                            }
                            ImGui.PopStyleVar();
                        }

                        ImGui.TreePop();
                    }

                    ImGui.PopID();
                }

                ImGui.PopStyleVar();
            }

            if (Selector.Selected is Actor actor && ImGui.IsKeyDown(ImGuiKey.Delete))
            {
                EditorSystem.Save.MarkDirty(actor);
                Actor.Destroy(actor);
            }

            OnEndWindow();
        }

        private void PopulateDragGraph()
        {
            var hasTarget = _currentDraggingActor.TryGetTarget(out var draggedActor);

            if (!hasTarget)
            {
                _currentSelectionGraph.Clear();
            }
            if (hasTarget && draggedActor.GetID() != _prevDragActorId)
            {
                _currentSelectionGraph.Clear();
                _prevDragActorId = draggedActor.GetID();

                void PopulateSelectionGraph(Actor current)
                {
                    _currentSelectionGraph.Add(current.GetID(), default);
                    for (int i = 0; i < current.Transform.Children.Count; i++)
                    {
                        PopulateSelectionGraph(current.Transform.Children[i].Actor);
                    }
                }
                PopulateSelectionGraph(draggedActor);
            }

            if (_isPendingToDropActor)
            {
                _prevDragActorId = Guid.Empty;
                _currentDraggingActor.SetTarget(null);
            }
        }
        private void SetSelectedActorParentGraph(Actor actor, bool force = false, bool includeActor = false)
        {
            if (actor)
            {
                Selector.Selected = actor;
            }

            if (actor && (force || _prevSelectedActorIdGraph != actor.GetID()))
            {
                if (includeActor)
                {
                    _expandParents.Add(actor.GetID());
                }
                _prevSelectedActorIdGraph = actor.GetID();
                _firstTimeSelectedActorId = actor.GetID();

                var parent = actor.Transform.Parent;
                while (parent != null)
                {
                    _expandParents.Add(parent.Actor.GetID());

                    parent = parent.Parent;

                    if (!parent)
                    {
                        break;
                    }
                }
            }
        }
        private void DrawActor(Actor actor)
        {
            ImGui.PushID(actor.GetID().ToString());

            bool hasChildren = actor.Transform.Children.Count > 0;

            var tintColor = new vec4(1, 1, 1, 1);
            if (!actor.IsActiveInHierarchy)
            {
                const float disabledAlpha = 0.6f;
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(tintColor.x, tintColor.y, tintColor.z, disabledAlpha));
                tintColor.w = disabledAlpha;
            }

            var greenSelected = EditorColors.MainColor.ToVector4();
            greenSelected.W = 0.6f;
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, greenSelected);
            int popColors = 1;

            bool isSelected = Selector.Selected && Selector.Selected == actor;
            var headerColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];

            var anotherIsDrag = (_pressedActorId != null && _pressedActorId != actor.GetID());

            if (!isSelected || anotherIsDrag)
            {
                ImGui.PushStyleColor(ImGuiCol.Header, headerColor);
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerColor);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Header, greenSelected);
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, greenSelected);
            }

            popColors += 2;

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1f));
            popColors += 1;

            if ((_currentSelectionGraph.TryGetValue(actor.GetID(), out var selectedInfo) && selectedInfo.Expanded) ||
                _expandParents.Contains(actor.GetID()))
            {
                _expandParents.Remove(actor.GetID());
                ImGui.SetNextItemOpen(true);
            }

            var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;

            if (!hasChildren)
                flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

            if (Selector.Selected && Selector.Selected == actor)
                flags |= ImGuiTreeNodeFlags.Selected;

            bool open = ImGui.TreeNodeEx("##node", flags);
            var isItemVisible = ImGui.IsItemVisible();

            if (_currentSelectionGraph.TryGetValue(actor.GetID(), out var selectionInfo))
            {
                selectionInfo.Expanded = open || (flags & ImGuiTreeNodeFlags.Leaf) != 0;
                _currentSelectionGraph[actor.GetID()] = selectionInfo;
            }

            DropActorHandle(actor, dropActor =>
            {
                if (!IsChildren(dropActor, actor))
                {
                    dropActor.Transform.Parent = actor.Transform;
                }
            });


            var isDrag = EditorImGui.DragAndDrop.ItemDragReference(actor.Name, EditorImGui.DragAndDrop.PAYLOAD_ID_EOBJECT, actor, actor.GetType(), actor.GetID());

            if (isDrag)
            {
                _isPendingToDropActor = false;
                _currentDraggingActor.SetTarget(actor);
            }
            else if (!ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                _isPendingToDropActor = true;
            }

            var id = actor.GetID();

            if (ImGui.IsItemActivated())
            {
                _pressedActorId = id;
            }

            if (ImGui.IsItemDeactivated())
            {
                if (_pressedActorId.HasValue && _pressedActorId.Value == id && ImGui.IsItemHovered())
                {
                    Selector.Selected = actor;
                }

                _pressedActorId = null;
            }


            if (!isItemVisible && _firstTimeSelectedActorId == actor.GetID())
            {
                _firstTimeSelectedActorId = Guid.Empty;
                ImGui.SetScrollHereY();
            }

            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                EditorCamera.Focus(actor.Transform.WorldPosition);
            }
            if (EditorImGui.BeginPopupContextItem("ActorContext"))
            {
                Selector.Selected = actor;

                if (ImGui.MenuItem("Create Actor"))
                {
                    var newActor = new Actor("Actor");
                    newActor.Transform.Parent = actor.Transform;
                    newActor.Transform.LocalPosition = default;
                    newActor.Transform.LocalRotation = quat.Identity;
                    newActor.Transform.LocalScale = vec3.One;

                    SetSelectedActorParentGraph(newActor);
                    EditorSystem.Save.MarkDirty(actor);
                }

                if (ImGui.MenuItem("Create Actor Parent"))
                {
                    var newActor = new Actor("Actor");
                    var oldParent = actor.Transform.Parent;
                    newActor.Scene.UnregisterRootActor(newActor);
                    newActor.Transform.Parent = oldParent;
                    if (!oldParent)
                    {
                        actor.Scene.RegisterRootActor(newActor);
                    }
                    else
                    {
                        newActor.Transform.LocalPosition = default;
                        newActor.Transform.LocalRotation = quat.Identity;
                        newActor.Transform.LocalScale = vec3.One;

                        newActor.Scene = actor.Scene;
                    }
                    actor.Transform.Parent = newActor.Transform;
                    SetSelectedActorParentGraph(newActor, true, true);
                    EditorSystem.Save.MarkDirty(actor);
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Rename"))
                {
                }
                ImGui.BeginDisabled();
                if (ImGui.MenuItem("Duplicate"))
                {
                    // TODO: create and re-assign the new instance ids of actors and components.

                    var actorsTree = SceneSerializer.SerializeActorsTree(actor);
                    var actorsRootInstances = SceneDeserializer.DeserializeScene(actorsTree, actor.Scene, true);

                    if (actorsRootInstances != null)
                    {
                        foreach (var actorRoot in actorsRootInstances)
                        {
                            actorRoot.Transform.Parent = actor.Transform.Parent;
                        }
                    }

                    EditorSystem.Save.MarkDirty(actor);
                }
                ImGui.EndDisabled();
                ImGui.Separator();
                ImGui.BeginDisabled(!actor.Transform.Parent);

                if (ImGui.MenuItem("Clear parent"))
                {
                    actor.Transform.Parent = null;
                    EditorSystem.Save.MarkDirty(actor);
                }

                if (ImGui.MenuItem("Unparent to previous"))
                {
                    if (actor.Transform.Parent.Parent)
                    {
                        actor.Transform.Parent = actor.Transform.Parent.Parent;
                    }
                    else
                    {
                        actor.Transform.Parent = null;
                    }
                    EditorSystem.Save.MarkDirty(actor);
                }

                ImGui.EndDisabled();
                ImGui.BeginDisabled(!actor.Transform.CanMoveSiblingDown());
                if (ImGui.MenuItem("Set as first sibling"))
                {
                    actor.Transform.SetAsFirstSibling();
                    EditorSystem.Save.MarkDirty(actor);
                }
                ImGui.EndDisabled();
                ImGui.BeginDisabled(!actor.Transform.CanMoveSiblingUp());
                if (ImGui.MenuItem("Set as last sibling"))
                {
                    actor.Transform.SetAsLastSibling();
                    EditorSystem.Save.MarkDirty(actor);
                }
                ImGui.EndDisabled();

                ImGui.BeginDisabled(!actor.Transform.CanMoveSiblingDown());
                if (ImGui.MenuItem("Move up"))
                {
                    actor.Transform.MoveSiblingDown();
                    EditorSystem.Save.MarkDirty(actor);
                }
                ImGui.EndDisabled();
                ImGui.BeginDisabled(!actor.Transform.CanMoveSiblingUp());
                if (ImGui.MenuItem("Move Down"))
                {
                    actor.Transform.MoveSiblingUp();
                    EditorSystem.Save.MarkDirty(actor);
                }
                ImGui.EndDisabled();
                ImGui.Separator();
                if (ImGui.MenuItem("Delete"))
                {
                    ImGui.EndPopup();

                    if (open && hasChildren)
                    {
                        ImGui.TreePop();
                    }

                    ImGui.PopID();
                    EditorSystem.Save.MarkDirty(actor);
                    Actor.Destroy(actor);
                    ImGui.PopStyleColor(popColors);
                    return;
                }

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor(popColors);

            ImGui.SameLine();
            var iconCursorX = ImGui.GetCursorPosX();
            var iconOffset = 8;
            ImGui.SetCursorPosX(iconCursorX - iconOffset);
            EditorImGui.ImageFromIcon(EditorIcon.Actor2, new GlmNet.vec2(18, 18), tintColor);

            ImGui.SameLine();
            iconCursorX = ImGui.GetCursorPosX();
            ImGui.SetCursorPosX(iconCursorX + iconOffset - 4);
            ImGui.TextUnformatted(actor.Name);

            if (!actor.IsActiveInHierarchy)
                ImGui.PopStyleColor();

            if (open && hasChildren)
            {
                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    var child = actor.Transform.Children[i];
                    DrawActor(child.Actor);
                }

                ImGui.TreePop();
            }

            ImGui.PopID();
        }

        private void DropActorHandle(Actor target, Action<Actor> callback)
        {
            if (EditorImGui.DragAndDrop.ItemDropReference(EditorImGui.DragAndDrop.PAYLOAD_ID_EOBJECT, out var payload))
            {
                if (payload.Value is Actor dropActor)
                {
                    // _dragActorId = Guid.Empty;
                    callback(dropActor);
                    _pressedActorId = null;
                    EditorSystem.Save.MarkDirty(dropActor);
                    SetSelectedActorParentGraph(dropActor, true);
                }
                else if (payload.Type == typeof(Texture2D) || payload.Type == typeof(Texture))
                {
                    var texAsset = Assets.GetAssetFromGuid(payload.RefId) as TextureAsset;
                    var sprite = texAsset.Atlas.GetSprite(0);
                    SetSprite(target, sprite);
                }
                else if (payload.Type == typeof(Sprite))
                {
                    var texAsset = Assets.GetAssetFromGuid(payload.RefId) as TextureAsset;
                    var sprite = texAsset.Atlas.GetSprite(payload.Index);
                    SetSprite(target, sprite);
                }
                else if (payload.Type == typeof(Material) && target)
                {
                    var renderer = target.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.Material = Assets.GetAssetFromGuid(payload.RefId) as Material;
                    }
                }
            }
        }

        private void SetSprite(Actor target, Sprite sprite)
        {
            var renderer = new Actor(sprite.Name).AddComponent<SpriteRenderer>();
            renderer.Material = Assets.GetMaterial(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME + "/Materials/SpriteDefault.material");
            renderer.Sprite = sprite;

            if (target)
            {
                renderer.Transform.Parent = target.Transform;
                renderer.Transform.LocalPosition = default;
                renderer.Transform.LocalRotation = quat.Identity;
            }

            Selector.Selected = renderer.Actor;
            EditorSystem.Save.MarkDirty(renderer);
        }
        /// <summary>
        /// Checks if 'to' is a children of 'from'
        /// </summary>
        private bool IsChildren(Actor from, Actor to)
        {
            var parent = to.Transform.Parent;
            while (parent)
            {
                if (parent == from.Transform)
                {
                    return true;
                }
                parent = parent.Parent;
            }

            return false;
        }
    }
}