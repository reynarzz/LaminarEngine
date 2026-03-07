using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
using Box2D.NET;
using Engine.Types;
using Engine.Utils;
using System.Numerics;
using Engine;

namespace Engine
{
    [UniqueComponent, RequireComponent(typeof(TilemapRenderer))]
    public class TilemapCollider2D : Collider2D
    {
        [RequiredProperty] private TilemapRenderer _renderer;

        protected override B2ShapeId[] CreateShape(B2BodyId bodyId)
        {
            var polygons = GetPolygons();

            if (polygons == null || polygons.Length == 0)
                return null;

            var shapesid = new B2ShapeId[polygons.Length];

            for (int i = 0; i < polygons.Length; i++)
            {
                shapesid[i] = B2Shapes.b2CreatePolygonShape(bodyId, ref ShapeDef, ref polygons[i]);
            }

            return shapesid;
        }

        private B2Polygon[] GetPolygons()
        {
            var layer = _renderer.GetLayer();
            var boxes = layer.CollisionBoxes;

            if(boxes != null)
            {
                var polygons = new B2Polygon[boxes.Length];
                for (int i = 0; i < boxes.Length; i++)
                {
                    polygons[i] = B2Geometries.b2MakeOffsetBox(boxes[i].Size.x / 2.0f, boxes[i].Size.y / 2.0f, (boxes[i].Position + Offset).ToB2Vec2(), glm.radians(RotationOffset).ToB2Rot());
                }
             
                return polygons;
            }
            return null;
        }

        protected override void UpdateShape()
        {
            var polygons = GetPolygons();

            if (polygons == null)
                return;

            for (int i = 0; i < polygons.Length; i++)
            {
                B2Shapes.b2Shape_SetPolygon(ShapesId[i], ref polygons[i]);
            }
        }

        protected internal override void OnDestroy()
        {
            base.OnDestroy();

            _renderer = null;
        }
    }
}
