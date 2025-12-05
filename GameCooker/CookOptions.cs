using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    public class CookFileOptions
    {
        public bool CompressAllFiles { get; set; }
        /// <summary>
        /// From 0 (Fast) to 12 (Max compression)
        /// </summary>
        public int CompressionLevel { get; set; }
        public bool EncryptAllFiles { get; set; }

        public HashSet<AssetType> FilesToCompress { get; private set; } = new();
        public HashSet<AssetType> FilesToEncrypt { get; private set; } = new();
        public bool EncryptFilesPath { get; set; }

        public CookFileOptions() { }
    }

    public class CookOptions
    {
        public CookingType Type { get; set; }
        public string AssetsFolderPath { get; set; }
        public string ExportFolderPath { get; set; }
        public CookFileOptions FileOptions { get; set; }

        /// <summary>
        /// Only cook these files, path should be relative to the 'Assets' folder
        /// </summary>
        public string[] MatchingFiles { get; set; }
    }
}