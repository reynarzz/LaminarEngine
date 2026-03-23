using Engine;
using Engine.Layers;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    // EObject property drawer
    public partial class EditorGuiFieldsResolver
    {
        private const int ASSETS_COLUMNS_COUNT_POPUP = 4;
        public static bool DrawEObjectSlot<T>(in T lazy, Type valueType, Func<EObject, bool> setValue) where T : ILazyRef
        {
            // TODO: do not load the asset like this, this defeats the purpose, but for testing is fine for now.
            return DrawEObjectSlot(Assets.GetAssetFromGuid(lazy.GetRefId()), valueType, setValue);
        }

        public static bool DrawEObjectSlot(IObject eObject, Type valueType, Func<EObject, bool> setValue)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(MathF.Max(XPosOffset, ImGui.GetCursorPosX()) + 5);
            var hasObject = eObject != null && eObject.IsValid();
            bool isAsset = false;
            bool isAssetMissingReference = false;

            if (hasObject && eObject is Asset asset)
            {
                isAsset = true;
                // TODO: Show a 'Missing reference' text in the
                //--isAssetMissingReference = !asset.IsPhysicallyAvailable;
                //--hasObject = asset.IsPhysicallyAvailable;
            }

            string label = null;

            if (hasObject)
            {
                if (isAssetMissingReference)
                {
                    label = $"Missing Ref: {eObject.Name}";
                }
                else
                {
                    label = $"{eObject.Name}";
                }
            }
            else
            {
                label = "None";
            }


            var drawList = ImGui.GetWindowDrawList();
            var pos = ImGui.GetCursorScreenPos();
            var size = ImGui.CalcTextSize(label);

            var width = ImGui.GetContentRegionAvail().X - 5;
            var min = new Vector2(pos.X - 5, pos.Y);
            var max = new Vector2(pos.X + width, pos.Y + size.Y + 7);

            var preRectCursor = ImGui.GetCursorPos();

            ImGui.SetCursorPos(preRectCursor);


            drawList.AddRectFilled(min, max, ImGui.ColorConvertFloat4ToU32(new(0.1f, 0.1f, 0.1f, 1f)), ImGui.GetStyle().FrameRounding);
            if (hasObject)
            {
                nint imagePtr = 0;
                ImGui.SetCursorPos(preRectCursor + new Vector2(-2, 5));

                if (eObject is RenderTexture rendTex)
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(rendTex);
                    EditorImGui.Image(imagePtr, new vec2(16, 16));
                }
                else if (eObject is Texture tex)
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(tex);
                    EditorImGui.Image(imagePtr, new vec2(16, 16));
                }
                else if (eObject is Sprite sprite)
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(sprite.Texture);
                    var cell = sprite.GetAtlasCell();

                    EditorImGui.Image(imagePtr, new vec2(16, 16), cell.Uvs.BottomLeftUV, cell.Uvs.TopLeftUV, cell.Uvs.TopRightUV, cell.Uvs.BottomRightUV);
                }
                else
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(eObject.GetType());
                    EditorImGui.Image(imagePtr, new vec2(16, 16));
                }

                ImGui.SetCursorPos(preRectCursor + new Vector2(16, 0));
            }


            string suffix = $"({ReflectionUtils.GetFriendlyTypeName(valueType)})";
            float suffixWidth = ImGui.CalcTextSize(suffix).X;

            const float offset = 10;
            var length = (max.X - min.X) - offset;
            float availableLabelWidth = length - suffixWidth;
            if (availableLabelWidth < 0)
                availableLabelWidth = 0;

            string displayLabel = label;

            float labelWidth = ImGui.CalcTextSize(label).X;
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);

            if (hasObject)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(new(1.0f, 1.0f, 1.0f, 1f)));
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(new(0.7f, 0.7f, 0.7f, 1f)));
            }

            if (labelWidth > availableLabelWidth)
            {
                const string ellipsis = "...";
                float ellipsisWidth = ImGui.CalcTextSize(ellipsis).X;

                int count = 0;
                float wwidth = 0f;

                foreach (char c in label)
                {
                    float w = ImGui.CalcTextSize(c.ToString()).X;
                    if (wwidth + w + ellipsisWidth > availableLabelWidth)
                        break;

                    wwidth += w;
                    count++;
                }

                displayLabel = label.Substring(0, count) + ellipsis;

                ImGui.Text($"{displayLabel}{suffix}");
            }
            else
            {
                ImGui.Text($"{displayLabel} {suffix}");
            }
            ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.SetCursorPos(preRectCursor.X + width - 24, preRectCursor.Y + 3);

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0f, 0f, 0f, 0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0f, 0f, 0f, 0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0f, 0f, 0f, 0f));

            var afterTextCursorPos = ImGui.GetCursorPos();

            ImGui.SetCursorPos(preRectCursor);
            var invisibleButtonSize = new Vector2(max.X - min.X, max.Y - min.Y);
            invisibleButtonSize.X -= 30;
            ImGui.InvisibleButton($"DropRect##_DROP_RECT_{valueType.Name}", invisibleButtonSize);

            if (hasObject)
            {
                if (eObject is Asset res)
                {
                    ImGui.SetItemTooltip($"{res.Path}");
                }
                else
                {
                    ImGui.SetItemTooltip(eObject.GetID().ToString());
                }
            }
            if (EditorImGui.DragAndDrop.ItemDropReference(EditorImGui.DragAndDrop.PAYLOAD_ID_EOBJECT, out var result))
            {
                DropValue(valueType, result, setValue);
            }

            ImGui.SetCursorPos(afterTextCursorPos);
            if (EditorImGui.ImageButtonFromIcon("_PICKER_BUTTON_", EditorIcon.CirclePicker, new vec2(13, 13)))
            {
                _openPopup = true;
            }
            ImGui.PopStyleColor(3);

            ImGui.Dummy(new Vector2(0, ImGui.GetStyle().ItemSpacing.Y - 2));
            PickObjectPopup(valueType, setValue);

            return false;
        }

        private static void PickObjectPopup(Type valueType, Func<EObject, bool> setValue)
        {
            if (_openPopup)
            {
                _openPopup = false;
                ImGui.CloseCurrentPopup();
                ImGui.OpenPopup("ObjectPickPopup");
            }

            //var winSize = ImGui.GetWindowSize();

            //ImGui.SetNextWindowSizeConstraints(new Vector2(400, 100), new Vector2(1400, 500));
            //var viewPortPos = ImGui.GetWindowViewport().Pos;

            //ImGui.SetNextWindowPos(new Vector2(viewPortPos.X + winSize.X / 2, viewPortPos.Y + winSize.Y / 2 - 250));
            if (!ImGui.BeginPopup("ObjectPickPopup"))
                return;

            if (ImGui.Selectable("None"))
            {
                setValue(null);
                ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
                return;
            }

            if (typeof(Asset).IsAssignableFrom(valueType))
            {
                // Asset picking
                if (valueType == typeof(Material))
                {
                    DrawAssetColumns(AssetType.Material, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType == typeof(AudioClip))
                {
                    DrawAssetColumns(AssetType.Audio, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType == typeof(RenderTexture))
                {
                    DrawAssetColumns(AssetType.RenderTexture, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType.IsAssignableTo(typeof(Texture2D)))
                {
                    DrawAssetColumns(AssetType.Texture, refId =>
                    {
                        var texture = (Assets.GetAssetFromGuid(refId) as TextureAsset)?.Texture;
                        return texture as Texture2D;
                    }, setValue);

                }
                else if (valueType.IsAssignableTo(typeof(Texture)))
                {
                    DrawAssetColumns(AssetType.Texture, refId => (Assets.GetAssetFromGuid(refId) as TextureAsset)?.Texture, setValue);
                }
                else if (valueType == typeof(TilemapAsset))
                {
                    DrawAssetColumns(AssetType.Tilemap, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType == typeof(SceneAsset))
                {
                    DrawAssetColumns(AssetType.Scene, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType == typeof(AnimationClip))
                {
                    DrawAssetColumns(AssetType.AnimationClip, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType == typeof(AnimatorController))
                {
                    DrawAssetColumns(AssetType.AnimatorController, Assets.GetAssetFromGuid, setValue);
                }
                else if (valueType == typeof(Sprite))
                {
                    var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Texture);
                    var spriteItems = new List<AssetPickedInfo>();

                    foreach (var (id, info) in assets)
                    {
                        var meta = EditorAssetUtils.GetAssetMeta(info.Path, AssetType.Texture) as TextureMetaFile;
                        if (meta?.AtlasData == null || meta.AtlasData.ChunksCount == 0)
                            continue;

                        var atlas = Assets.GetSpriteAtlas(info.Path);

                        for (int i = 0; i < meta.AtlasData.ChunksCount; i++)
                        {
                            var name = Sprite.CreateSpriteName(Path.GetFileName(info.Path), i);
                            var label = $"{name}###{i}__{info.Path}";

                            // This copy is needed since I'm passing it to a lambda.
                            int iCopy = i;

                            spriteItems.Add(new AssetPickedInfo()
                            {
                                Name = label,
                                Path = info.Path,
                                SetValueCallback = () => setValue(atlas.GetSprite(iCopy)),
                            });
                        }
                    }

                    // Render all sprites in columns (choose number of columns here)
                    RenderItemsInColumns(spriteItems);
                }
            }
            else
            {
                foreach (var scene in SceneManager.Scenes)
                {
                    var root = scene.RootActors;
                    for (int i = 0; i < root.Count; i++)
                    {
                        DrawSceneObjectPropertyPicker(root[i].Transform, valueType, setValue);
                    }
                }
            }

            ImGui.EndPopup();
        }

        private static bool CanBeAssigned(Type payloadType, Type valueType)
        {
            if (payloadType == null)
                return false;

            if (ReflectionUtils.IsLazy(valueType))
            {
                ReflectionUtils.TryGetLazyType(valueType, out valueType);
            }
            if (payloadType.IsAssignableTo(valueType))
            {
                return true;
            }
            else if (valueType == typeof(Sprite) && payloadType.IsAssignableFrom(typeof(Texture2D)))
            {
                return true;
            }
            else if (payloadType == typeof(Actor) && valueType.IsAssignableTo(typeof(Component)))
            {
                return true;
            }
            return false;
        }

        private static void DropValue(Type valueType, EditorImGui.DragAndDrop.ReferenceDragAndDropPayload payload, Func<EObject, bool> setValue)
        {
            if (!CanBeAssigned(payload.Type, valueType))
            {
                return;
            }

            // If 'valueType' is the type 'EObject' then we would select whatever type is in the payload.
            if (valueType == typeof(EObject))
            {
                valueType = payload.Type;
            }

            if (typeof(Asset).IsAssignableFrom(valueType))
            {
                if (valueType.IsAssignableTo(typeof(Texture)))
                {
                    var texture = Assets.GetAssetFromGuid(payload.RefId) as TextureAsset;
                    setValue(texture?.Texture);
                }
                else
                {
                    setValue(Assets.GetAssetFromGuid(payload.RefId));
                }
            }
            else if (valueType == typeof(Sprite))
            {
                var texture = Assets.GetAssetFromGuid(payload.RefId) as TextureAsset;
                if (texture)
                {
                    var atlas = texture?.Atlas.GetSprite(Mathf.Max(0, payload.Index));
                    setValue(atlas);
                }
            }
            else if (valueType == typeof(Actor))
            {
                setValue(payload.Value);
            }
            else if (valueType.IsAssignableTo(typeof(Component)))
            {
                var actor = payload.Value as Actor;
                if (actor)
                {
                    foreach (var component in actor.Components)
                    {
                        if (component.GetType().IsAssignableTo(valueType))
                        {
                            setValue(component);
                            break;
                        }
                    }
                }
            }
            else
            {
                setValue(payload.Value);
            }
        }

        private static void DrawSceneObjectPropertyPicker(Transform root, Type targetType, Func<EObject, bool> setValue)
        {
            if (typeof(Actor).IsAssignableFrom(targetType))
            {
                if (ImGui.Selectable($"{root.Name}##{root.GetID()}"))
                {
                    setValue(root.Actor);
                    ImGui.CloseCurrentPopup();
                }
            }
            else if (typeof(IComponent).IsAssignableFrom(targetType))
            {
                // TODO: this is slow, it should be cached.
                var components = root.Actor.Components.Where(x => x.GetType().IsAssignableTo(targetType)).ToArray();

                if (components.Length > 0 && ImGui.Selectable($"{root.Name}##{root.GetID()}"))
                {
                    foreach (var comp in components)
                    {
                        if (targetType.IsAssignableFrom(comp.GetType()))
                        {
                            if (setValue(comp))
                                break;
                        }
                    }
                    ImGui.CloseCurrentPopup();
                }
            }
            //else if (typeof(EObject).IsAssignableFrom(targetType))
            //{
            //    if (ImGui.Selectable($"{root.Name}##{root.GetID()}"))
            //    {
            //        setValue(root.Actor);
            //        ImGui.CloseCurrentPopup();
            //    }
            //}
            for (int i = 0; i < root.Children.Count; i++)
            {
                DrawSceneObjectPropertyPicker(root.Children[i], targetType, setValue);
            }
        }

        private static void DrawAssetColumns(AssetType type, Func<Guid, EObject> getValue, Func<EObject, bool> setValue)
        {
            var assets = IOLayer.Database.Disk.GetAssetsInfo(type);
            var items = assets.Select(idAssetInfo =>
            {
                string label = $"{Path.GetFileName(idAssetInfo.Value.Path)}##{idAssetInfo.Key}";

                return new AssetPickedInfo()
                {
                    Name = label,
                    SetValueCallback = () => setValue(getValue(idAssetInfo.Key)),
                    Path = idAssetInfo.Value.Path
                };
            });
            RenderItemsInColumns(items);
        }

        private static void RenderItemsInColumns(IEnumerable<AssetPickedInfo> items)
        {
            if (!items.Any())
                return;

            int count = 0;
            ImGui.BeginTable("PopupTable", ASSETS_COLUMNS_COUNT_POPUP, ImGuiTableFlags.None);

            foreach (var item in items)
            {
                if (count % ASSETS_COLUMNS_COUNT_POPUP == 0)
                    ImGui.TableNextRow();

                ImGui.TableNextColumn();
                if (ImGui.Selectable(item.Name))
                {
                    item.SetValueCallback();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemTooltip(item.Path);

                count++;
            }

            ImGui.EndTable();
        }


        private struct AssetPickedInfo
        {
            public string Name { get; set; }
            public Action SetValueCallback { get; set; }
            public string Path { get; set; }

        }
    }
}
