using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class GameEntityManager
    {
        private readonly Dictionary<string, GameEntityBuilderBase> _entityBuilders;
        public GameEntityManager()
        {
            _entityBuilders = new Dictionary<string, GameEntityBuilderBase>()
            {
                { "Door", new DoorEntityBuilder() }
            };
        }

        public bool BuildEntity(string name, vec2 position, FieldInstance[] fields)
        {
            if(_entityBuilders.TryGetValue(name, out var builder))
            {
                return builder.Build(position, fields);
            }

            return false;
        }
    }
}
