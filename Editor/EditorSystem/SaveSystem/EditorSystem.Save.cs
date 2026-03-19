using Editor.Utils;
using Engine;
using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal partial class EditorSystem
    {
        public static class Save
        {
            private readonly static Dictionary<Guid, DirtyEObject> _dirtyObjectsRefId = new();
            private readonly static Dictionary<AssetType, IFileSaver> _fileSavers = new()
            {
                { AssetType.Scene, new EditorSceneSaver() }
            };

            public static void SaveAll()
            {
                if (Application.IsInPlayMode)
                {
                    Debug.Warn("Cannot save in playmode.");
                    return;
                }

                if (_dirtyObjectsRefId.Count == 0)
                {
                    Debug.Log("Nothing to save.");
                    return;
                }
                bool isAnySaved = false;
                // Save all to disk
                foreach (var (refId, dirtyObject) in _dirtyObjectsRefId)
                {
                    if (_fileSavers.TryGetValue(dirtyObject.Type, out var saver))
                    {
                        if (EditorIOLayer.Database.ExistsAsset(refId))
                        {
                            var info = EditorIOLayer.Database.GetAssetInfo(refId);
                            Debug.Log("Saving asset: " + info.Path);
                            saver.Write(refId, info.Path);

                            isAnySaved = true;
                        }
                        else
                        {
                            Debug.Error($"Can't save asset '{refId}' it doesn't exists.");
                        }

                    }
                }

                if (isAnySaved)
                {
                    EditorAssetUtils.RefreshAssetDatabase();
                }

                // Not longer dirty, clear all.
                _dirtyObjectsRefId.Clear();
            }

            public static void MarkDirty(EObject obj)
            {
                if (!obj)
                {
                    Debug.Error($"Can't mark dirty null '{nameof(EObject)}'");
                    return;
                }

                var assetType = AssetType.Invalid;
                var refId = Guid.Empty;

                var isSceneDirty = TryGetSceneDirty(obj, ref refId, ref assetType, out bool isInvalidScene);

                if (isInvalidScene)
                {
                    return;
                }

                if (Application.IsInPlayMode && isSceneDirty)
                {
                    return;
                }
                if (!isSceneDirty)
                {
                    var info = EditorIOLayer.Database.GetAssetInfo(obj.GetID());
                    refId = obj.GetID();
                    assetType = info.Type;
                }

                if (!_dirtyObjectsRefId.ContainsKey(refId))
                {
                    _dirtyObjectsRefId.Add(refId, new DirtyEObject()
                    {
                        Type = assetType
                    });
                }
            }

            private static bool TryGetSceneDirty(EObject obj, ref Guid refId, ref AssetType assetType, out bool isInvalidScene)
            {
                isInvalidScene = false;

                if (Application.IsInPlayMode)
                {
                    isInvalidScene = true;
                    return false;
                }
                if (obj is Actor or Component)
                {
                    Scene scene = null;
                    scene = obj switch
                    {
                        Actor actor => actor.Scene,
                        Component component => component.Actor.Scene,
                        _ => null
                    };

                    if (scene && scene != SceneManager.DontDestroyOnLoadScene)
                    {
                        assetType = AssetType.Scene;
                        refId = scene.GetID();
                        return true;
                    }
                    else
                    {
                        isInvalidScene = true;
                    }
                }

                return false;
            }

            internal static bool IsAnyDirty()
            {
                return _dirtyObjectsRefId.Count > 0;
            }
            internal static bool IsAnyAssetDirty(AssetType type)
            {
                foreach (var (refId, assetType) in _dirtyObjectsRefId)
                {
                    if (assetType.Type == type)
                    {
                        return true;
                    }
                }

                return false;
            }

            internal static bool IsDirty(Guid guid)
            {
                return _dirtyObjectsRefId.ContainsKey(guid);
            }

            internal static void RemoveDirty(Guid refId)
            {
                _dirtyObjectsRefId.Remove(refId);
            }
        }

        private struct DirtyEObject
        {
            public AssetType Type;
        }
    }
}
