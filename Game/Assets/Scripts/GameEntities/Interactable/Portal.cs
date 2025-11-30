using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Portal : AnimatedInteractable<PortalData>
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            AddComponent<Rotate>();
        }
        public override bool TryInteract(Player player)
        {
            return false;
        }
    }
}
