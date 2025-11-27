
using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Move : ScriptBehavior
    {
        private vec3 _startPos;

        protected override void OnStart()
        {
            _startPos = Transform.WorldPosition;
        }

        protected override void OnUpdate()
        {
            var freq = Time.TimeCurrent * 3;

            Transform.WorldPosition = new vec3(_startPos.x + glm.cos(freq) * 3, _startPos.y);
        }
    }
}