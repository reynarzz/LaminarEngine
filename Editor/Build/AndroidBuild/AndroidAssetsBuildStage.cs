using GameCooker;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class AndroidAssetsBuildStage : AssetsBuildStage
    {
        public AndroidAssetsBuildStage() : base(CookingPlatform.Android, 
                                                CookingType.ReleaseMode, 
                                                AssetBuildType.OnlyMatchingFiles,
                                                EditorPaths.AndroidProjectAssetsFolderRoot,
                                                new CookFileOptions()
                                                {
                                                    CompressAllFiles = false,
                                                    CompressionLevel = 12,
                                                    EncryptAllFiles = false
                                                })
        {
        }
    }
}
