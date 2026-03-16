using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorController : AssetResourceBase
    {
        [SerializedField] internal AnimatorParameters Parameters { get; private set; } = new();
        [SerializedField] internal Dictionary<string, AnimationState> States { get; private set; } = new();

        // Remove
        internal AnimatorController() : base("Animator Controller", Guid.Empty)
        {
        }

        // Serializer
        internal AnimatorController(string path, Guid guid) : base(path, guid)
        {
        }

        protected override void OnUpdateResource(object data, string path, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
