using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorController : Asset
    {
        [SerializedField] internal AnimatorParameters Parameters { get; private set; } = new();
        [SerializedField] internal Dictionary<string, AnimationState> States { get; private set; } = new();

        // Remove
        internal AnimatorController() : base(Guid.Empty)
        {
            Name = "Animator Controller";
        }

        // Serializer
        internal AnimatorController(Guid guid) : base(guid)
        {
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
