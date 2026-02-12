using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Test
{
    internal class Test_MaterialBinary
    {

        public static void RunTest_Windows()
        {
            var processor = new Cooker.MaterialProcessorRelease();

            var path = Path.Combine(EditorPaths.CookerPaths.InternalAssetsPath, "Materials", "SpriteDefault.material");

            using var stream = File.OpenRead(path);
            using var fileReader = new BinaryReader(stream);
            var meta = new DefaultMetaFile() { GUID = Guid.NewGuid() };
            var result = processor.Process(fileReader, meta, Cooker.CookingPlatform.Windows);

            var builder = new MaterialAssetBuilder();
            using var mem = new MemoryStream(result.Data);
            using var reader = new BinaryReader(mem);
            var material = builder.BuildAsset(new AssetInfo() { Type = AssetType.Material, Path = path }, meta, reader);
        }
    }
}
