using Engine;
using Engine.IO;
using Engine.Layers;
using GameCooker;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class EditorAssetsBuildStage : AssetsBuildStage
    {
        internal EditorAssetsBuildStage() : base(CookingPlatform.Windows,
                                                 CookingType.DevMode,
                                                 AssetsBuildType.All)
        {
        }

        protected override CookData OnBeforeBuild()
        {
            return new CookData()
            {
                ExportFolderPath = Paths.GetAssetDatabaseFolder(),
                FileOptions = new CookFileOptions()
                {
                    CompressAllFiles = false,
                    CompressionLevel = 0,
                    EncryptAllFiles = false
                }
            };
        }

        public override async Task<BuildStageResult> Execute()
        {
            var result = await base.Execute();
            Debug.Log("Editor asset update");
            var assetDatabase = (AssetsDatabaseInfo)result.Data;

            await MainThreadDispatcher.EnqueueAsync(() =>
            {
                EditorIOLayer.Instance.ReloadDisk(assetDatabase);
            });

            return new BuildStageResult()
            {
                IsSuccess = true,
            };
        }

    }
}
