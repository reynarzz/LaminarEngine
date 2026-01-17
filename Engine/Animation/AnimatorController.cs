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

        // Serializer
        internal AnimatorController() : base("Animator Controller", Guid.Empty)
        {
        }

        public AnimatorController(string path, Guid guid) : base(path, guid)
        {
        }

        internal override void UpdateResource(object data, string path, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
