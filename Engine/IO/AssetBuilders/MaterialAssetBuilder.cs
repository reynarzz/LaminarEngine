using Engine.IO;
using Engine;
using Engine.Serialization;

namespace Engine
{
    internal class MaterialAssetBuilder : IAssetBuilder<Material, AssetMeta>
    {
        public Material BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            var materialIR = BinaryIRDeserializer.DeserializeMaterial(reader);

            var material = new Material(info.Path, meta.GUID);

            Deserializer.Deserialize(material, materialIR.Properties);

            return material;


            //new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text,
            //"Invalid builder", "invalid builder"));
        }

        public void UpdateAsset(ref readonly AssetInfo info, Material asset, AssetMeta meta, BinaryReader reader)
        {
        }
    }
}