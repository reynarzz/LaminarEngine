using Engine.IO;
using SharedTypes;

namespace Engine
{
    internal class MaterialAssetBuilder : AssetBuilderBase
    {
        internal override AssetResourceBase BuildAsset(AssetInfo info, AssetMetaFileBase meta, Guid guid, BinaryReader reader)
        {
            // TODO: read material correctly
           // throw new NotImplementedException("Implement Material Asset Builder");

            return new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text, 
                "Invalid builder", "invalid builder"));
        }

        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
        }
    }
}