using Engine.IO;
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
                                                 AssetBuildType.All,
                                                 Paths.GetAssetDatabaseFolder(),
                                                 new CookFileOptions()
                                                 {
                                                     CompressAllFiles = false,
                                                     CompressionLevel = 0,
                                                     EncryptAllFiles = false
                                                 })
        {
        }

        public override async Task<BuildStageResult> Execute()
        {
            var result = await base.Execute();

            var assetDatabase = (AssetsDatabaseInfo)result.Data;

            // Update database.
            foreach (var guid in assetDatabase.UpdatedAssets)
            {
               EditorIOLayer.Database?.UpdateReloadAsset(guid);
            }

            return new BuildStageResult()
            {
                IsSuccess = true,
            };
        }
    }
}
