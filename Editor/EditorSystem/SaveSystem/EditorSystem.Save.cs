using Engine;
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
                // Save all to disk
                foreach (var (refId, dirtyObject) in _dirtyObjectsRefId)
                {
                    if(_fileSavers.TryGetValue(dirtyObject.Type, out var saver))
                    {
                        var info = EditorIOLayer.Database.GetAssetInfo(refId);

                        saver.Write(refId, info.Path);
                    }
                }

                // Not longer dirty, clear all.
                _dirtyObjectsRefId.Clear();
            }

            public static void MarkDirty(EObject obj)
            {
                if (obj)
                {
                    var assetType = AssetType.Invalid;
                    var refId = Guid.Empty;

                    var isScene = MarkSceneDirty(obj, ref refId, ref assetType);

                    if (!isScene)
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
            }

            private static bool MarkSceneDirty(EObject obj, ref Guid refId, ref AssetType assetType)
            {
                if (obj is Actor or Component)
                {
                    refId = obj switch
                    {
                        Actor actor => actor.Scene.GetID(),
                        Component component => component.Actor.Scene.GetID(),
                        _ => Guid.Empty
                    };

                    assetType = AssetType.Scene;

                    return true;
                }

                return false;
            }
        }

        private struct DirtyEObject
        {
            public AssetType Type;
        }
    }
}
