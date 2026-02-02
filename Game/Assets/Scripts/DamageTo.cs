using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using GlmNet;

namespace Game
{
    public class DamageTo : ScriptBehavior
    {
        public int DamageAmount { get; set; }
        [SerializedField] public ulong Mask { get; set; }
         
        protected override void OnTriggerStay2D(Collider2D collider)
        {
            if (LayerMask.AreValid(collider.Actor.Layer, Mask))
            {
                collider.GetComponent<IDamageReceiver>()?.HitDamage(vec3.Zero, DamageAmount);

            }
        }
    }
}
