using Engine;
using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public abstract class AnimatedInteractable<T> : InteractableEntityBase<T> where T: InteractableData
    {
        [RequiredProperty] protected Animator Animator { get; set; }
    }
}
