using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class EventCurveData
    {
        public bool Raised { get; set; }
        public Action Callback { get; set; }
    }

    public class EventCurve : ConstantCurve<Action>
    {
        
    }
}