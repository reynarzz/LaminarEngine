using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    public struct BuildStageResult
    {
        public bool IsSuccess; 
        public string Message;
        public object Data;
    }

    internal abstract class BuildStage
    {
        public abstract Task<BuildStageResult> Execute();
        public virtual bool ShouldExecute() { return true; }
    }
}
