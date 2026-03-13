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
			private struct DirtyEObject
			{
				public AssetType Type;
			}

			private static Dictionary<Guid, DirtyEObject> _dirtyObjectsRefId;

			public static void SaveAll()
			{
				// TODO: Save everything marked as dirty: 

				// Save all to disk
				foreach (var (refId, dirtyObject) in _dirtyObjectsRefId)
				{

				}

				// Flush all
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
	}
}
