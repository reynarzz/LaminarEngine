using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class YieldInstruction
    {
        protected internal bool IsInstructionCompleted { get; protected set; } = false;

        internal virtual void OnBegin()
        {
        }

        internal virtual void Update() { }
    }
}
