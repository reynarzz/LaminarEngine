using GameCooker;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class WindowsAssetsBuildStage : AssetsBuildStage
    {
        public WindowsAssetsBuildStage() : base(CookingPlatform.Windows, 
                                                CookingType.ReleaseMode, 
                                                AssetBuildType.OnlyMatchingFiles,
                                                EditorPaths.Win32ShipGameDataFolderRoot,
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
