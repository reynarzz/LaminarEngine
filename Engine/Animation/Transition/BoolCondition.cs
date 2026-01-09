using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class BoolCondition : TransitionCondition
    {
        [SerializedField, HideFromInspector, JsonProperty] private bool _compare;
        public BoolCondition(string property, bool compare) : base(property)
        {
            _compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            return parameters.GetBool(Property) == _compare;
        }
    }
}
