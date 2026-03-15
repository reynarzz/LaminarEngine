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
		public static void DrawEObjectSlot(IObject eObject, Type valueType, Func<EObject, bool> setValue)
		{
			ImGui.SameLine();
			ImGui.SetCursorPosX(MathF.Max(XPosOffset, ImGui.GetCursorPosX()) + 5);
			var hasObject = eObject != null;
			string label = hasObject ? $"{eObject.Name}" : $"None";

			if (hasObject)
			{
				if (eObject is AssetResourceBase res)
				{
					ImGui.SetItemTooltip($"{res.Path}");
				}
				else
				{
					ImGui.SetItemTooltip(eObject.GetID().ToString());
				}
			}
			else
			{

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
                    
                    EditorImGui.ImageQuad(imagePtr, new vec2(16,16), cell.Uvs.BottomLeftUV, cell.Uvs.TopLeftUV, cell.Uvs.TopRightUV, cell.Uvs.BottomRightUV);
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
			ImGui.SetCursorPos(preRectCursor.X + width-24, preRectCursor.Y + 3);

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0f, 0f, 0f, 0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0f, 0f, 0f, 0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0f, 0f, 0f, 0f));

			if(EditorImGui.ImageButtonFromIcon("_PICKER_BUTTON_", EditorIcon.CirclePicker, new vec2(13, 13)))
			{
				_openPopup = true;
				_selectedValue = eObject;
				_selectedSetter = setValue;
			}
			ImGui.PopStyleColor(3);

			ImGui.Dummy(new Vector2(0, ImGui.GetStyle().ItemSpacing.Y - 2));
			PickObjectPopup(valueType, setValue);
		}

		private static void PickObjectPopup(Type valueType, Func<EObject, bool> setValue, int columnCount = 4)
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

			void RenderItemsInColumns(IEnumerable<(string label, Action action, string path)> items)
			{
				if (!items.Any())
					return;

				int count = 0;
				ImGui.BeginTable("PopupTable", columnCount, ImGuiTableFlags.None);

				foreach (var item in items)
				{
					if (count % columnCount == 0)
						ImGui.TableNextRow();

					ImGui.TableNextColumn();
					if (ImGui.Selectable(item.label))
					{
						item.action();
						ImGui.CloseCurrentPopup();
					}
					ImGui.SetItemTooltip(item.path);

					count++;
				}

				ImGui.EndTable();
			}


			// TODO: refactor the asset picker list behavior.
			if (typeof(AssetResourceBase).IsAssignableFrom(valueType))
			{
				// Asset picking
				if (valueType == typeof(Material))
				{
					//var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Material);
					//foreach (var (id, info) in assets)
					//{
					//    if (ImGui.Selectable($"{Path.GetFileName(info.Path)}##{id}{info.Path}"))
					//    {
					//        setValue(Assets.GetMaterial(info.Path));
					//    }
					//}

					var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Material);
					var items = assets.Select(a =>
					{
						var (id, info) = a;
						string label = $"{Path.GetFileName(info.Path)}##{id}{info.Path}";
						return (label, (Action)(() => setValue(Assets.GetMaterial(info.Path))), info.Path);
					});
					RenderItemsInColumns(items);
				}
				else if (valueType == typeof(AudioClip))
				{
					//var audios = IOLayer.Database.Disk.GetAssetsInfo(SharedTypes.AssetType.Audio);

					//foreach (var asset in audios)
					//{
					//    if (ImGui.Selectable($"{Path.GetFileName(asset.Value.Path)}##{asset.Key}"))
					//    {
					//        setValue(Assets.GetAudioClip(asset.Value.Path));
					//        ImGui.CloseCurrentPopup();
					//    }
					//}

					var assets = IOLayer.Database.Disk.GetAssetsInfo(Engine.AssetType.Audio);
					var items = assets.Select(a =>
					{
						var (id, info) = a;
						string label = $"{Path.GetFileName(info.Path)}##{id}";
						return (label, (Action)(() => setValue(Assets.GetAssetFromGuid(id))), info.Path);
					});
					RenderItemsInColumns(items);
				}
				else if (valueType == typeof(RenderTexture))
				{

				}
				else if (valueType.IsAssignableTo(typeof(Texture)))
				{
					//foreach (var guid in Assets.GetGuids(AssetType.Texture))
					//{
					//    var path = Assets.ResolvePath(guid);
					//    if (ImGui.Selectable($"{System.IO.Path.GetFileName(path)}##{guid}"))
					//    {
					//        setValue(Assets.GetTexture(path));
					//        ImGui.CloseCurrentPopup();
					//    }
					//}

					var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Texture);
					var items = assets.Select(a =>
					{
						var (id, info) = a;
						string label = $"{Path.GetFileName(info.Path)}##{id}";
						return (label, (Action)(() => setValue(Assets.GetTexture(info.Path))), info.Path);
					});
					RenderItemsInColumns(items);
				}
				else if (valueType == typeof(TilemapAsset))
				{
					var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Tilemap);
					var path = string.Empty;

					var items = assets.Select(a =>
					{
						var (id, info) = a;
						string label = $"{Path.GetFileName(info.Path)}##{id}";

						return (label, (Action)(() => setValue(Assets.GetAssetFromGuid(id))), info.Path);
					});
					RenderItemsInColumns(items);

				}
				else if (valueType == typeof(SceneAsset))
				{
					var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Scene);
					var path = string.Empty;

					var items = assets.Select(a =>
					{
						var (id, info) = a;
						string label = $"{Path.GetFileName(info.Path)}##{id}";

						return (label, (Action)(() => setValue(Assets.GetAssetFromGuid(id))), info.Path);
					});
					RenderItemsInColumns(items);

				}
			}
			else if (valueType == typeof(Sprite))
			{
				//var assets = IOLayer.Database.Disk.GetAssetsInfo(SharedTypes.AssetType.Texture);
				//foreach (var (id, info) in assets)
				//{
				//    // TODO: this is very very slow, I have to cache all the sprites names on load.
				//    var meta = EditorAssetUtils.GetAssetMeta(info.Path, AssetType.Texture) as TextureMetaFile;
				//    // var texturesInfo = EditorIOLayer.Database.GetAssetsInfoByType(AssetType.Texture);

				//    if (meta?.AtlasData == null || meta.AtlasData.ChunksCount == 0)
				//        continue;

				//    // TODO: use a tree node for multi sprites
				//    //if (ImGui.TreeNode())
				//    //{

				//    //}

				//    for (int i = 0; i < meta.AtlasData.ChunksCount; i++)
				//    {
				//        var name = Sprite.CreateSpriteName(Path.GetFileName(info.Path), i);
				//        if (ImGui.Selectable($"{name}##{meta.AtlasData.GetCell(i).ID}{i}{info.Path}"))
				//        {
				//            setValue(Assets.GetSpriteAtlas(info.Path).GetSprite(i));
				//        }
				//    }
				//}

				var assets = IOLayer.Database.Disk.GetAssetsInfo(Engine.AssetType.Texture);
				var spriteItems = new List<(string label, Action action, string path)>();

				foreach (var (id, info) in assets)
				{
					var meta = EditorAssetUtils.GetAssetMeta(info.Path, AssetType.Texture) as TextureMetaFile;
					if (meta?.AtlasData == null || meta.AtlasData.ChunksCount == 0)
						continue;

					var atlas = Assets.GetSpriteAtlas(info.Path);

					for (int i = 0; i < meta.AtlasData.ChunksCount; i++)
					{
						var name = Sprite.CreateSpriteName(Path.GetFileName(info.Path), i);
						var cellId = meta.AtlasData.GetCell(i).ID;
						var label = $"{name}##{cellId}{i}{info.Path}";

						// This copy is needed since I'm passing it to a lambda.
						int iCopy = i;

						spriteItems.Add((label, () =>
						{
							setValue(atlas.GetSprite(iCopy));
						}, info.Path));
					}
				}

				// Render all sprites in columns (choose number of columns here)
				RenderItemsInColumns(spriteItems);
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
	}
}
