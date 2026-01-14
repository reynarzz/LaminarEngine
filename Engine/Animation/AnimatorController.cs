using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorController : AssetResourceBase
    {
        [SerializedField] private Dictionary<string, AnimationState> _states = new();
        internal IDictionary<string, AnimationState> States => _states;

        // Serializer
        internal AnimatorController() : base(string.Empty, Guid.Empty)
        {
        }

        public AnimatorController(string path, Guid guid) : base(path, guid)
        {
        }
    }
}
