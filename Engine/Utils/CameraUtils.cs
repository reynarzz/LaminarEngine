using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Plane
    {
        public vec3 Normal;
        public float D;

        public float Distance(vec3 p)
        {
            return glm.dot(Normal, p) + D;
        }

        public static Plane Normalize(Plane plane)
        {
            float invLen = 1.0f / plane.Normal.length();
            plane.Normal *= invLen;
            plane.D *= invLen;
            return plane;
        }
    }

    public struct Frustum
    {
        public Plane[] Planes;
    }

    public class CameraUtils
    {
        public static Frustum ExtractFrustum(mat4 viewProj)
        {
            vec4 c0 = viewProj.c0;
            vec4 c1 = viewProj.c1;
            vec4 c2 = viewProj.c2;
            vec4 c3 = viewProj.c3;

            var planes = new Plane[6];

            // Left = c3 + c0
            planes[0] = Plane.Normalize(new Plane
            {
                Normal = (c3 + c0).xyz,
                D = (c3 + c0).w
            });

            // Right = c3 - c0
            planes[1] = Plane.Normalize(new Plane
            {
                Normal = (c3 - c0).xyz,
                D = (c3 - c0).w
            });

            // Bottom = c3 + c1
            planes[2] = Plane.Normalize(new Plane
            {
                Normal = (c3 + c1).xyz,
                D = (c3 + c1).w
            });

            // Top = c3 - c1
            planes[3] = Plane.Normalize(new Plane
            {
                Normal = (c3 - c1).xyz,
                D = (c3 - c1).w
            });

            // Near (LH, z >= 0)
            planes[4] = Plane.Normalize(new Plane
            {
                Normal = c2.xyz,
                D = c2.w
            });

            // Far = c3 - c2
            planes[5] = Plane.Normalize(new Plane
            {
                Normal = (c3 - c2).xyz,
                D = (c3 - c2).w
            });

            return new Frustum { Planes = planes };
        }

        public static bool IsSphereInFrustum(in Frustum frustum, vec3 center, float radius)
        {
            foreach (var plane in frustum.Planes)
            {
                if (plane.Distance(center) < -radius)
                {
                    return false;
                }
            }

            return true;
        }


        public static bool IsAABBInFrustum(in Frustum frustum, vec3 min, vec3 max)
        {
            foreach (var plane in frustum.Planes)
            {
                vec3 v = new vec3(plane.Normal.x >= 0 ? max.x : min.x,
                                  plane.Normal.y >= 0 ? max.y : min.y,
                                  plane.Normal.z >= 0 ? max.z : min.z);

                if (plane.Distance(v) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsConvexMeshInFrustum(in Frustum frustum, ReadOnlySpan<vec3> vertices)
        {
            foreach (var plane in frustum.Planes)
            {
                bool allOutside = true;

                foreach (var v in vertices)
                {
                    if (plane.Distance(v) >= 0)
                    {
                        allOutside = false;
                        break;
                    }
                }

                if (allOutside)
                {
                    return false;
                }
            }
            return true;
        }


    }
}
