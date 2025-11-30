using Engine;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PortalEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(vec2 position, FieldInstance[] fields, Func<vec2, bool, vec2> positionConverter)
        {
            var portal = new Actor<SpriteRenderer>("Portal").AddComponent< Portal>();
            var renderer = portal.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 14;
            renderer.Material = MaterialUtils.PortalMaterial;
            portal.Transform.LocalScale = new vec3(6, 6);
            portal.Transform.WorldPosition = position;
            return portal;
        }
    }
}
