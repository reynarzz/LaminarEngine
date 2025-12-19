using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace Game
{
    public interface IDamageReceiver
    {
        bool HitDamage(vec3 aggressorPosition, int amount);
    }
}
