using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public enum UniformType
    {
        Invalid,
        Bool,
        Int,
        Float,
        Double,
        Uint,
        Mat2,
        Mat3,
        Mat4,
        Vec2,
        Vec3,
        Vec4,
        IntArr,
        Texture2D,
        Texture2DArray,
        Texture3D,
        Texture3DArray,
        TextureCubeMap,
        TextureCubeMapArray,
        Sampler,
    }
}
