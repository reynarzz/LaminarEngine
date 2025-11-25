using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class GameEntityBuilderBase
    {
        public abstract GameEntity Build(vec2 position, FieldInstance[] fields);
    }
}
