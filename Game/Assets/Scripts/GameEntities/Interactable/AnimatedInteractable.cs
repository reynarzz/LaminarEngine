using Engine;
using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(Animator))]
    public abstract class AnimatedInteractable<T> : InteractableEntityBase<T> where T: InteractableData
    {
        protected Animator Animator { get; set; }
        public override void OnAwake()
        {
            base.OnAwake();
            Animator = GetComponent<Animator>();
        }
    }
}
