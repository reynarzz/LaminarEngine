using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Coroutine
    {
        private YieldInstruction _currentInstruction;
        private bool _canRun = true;
        private readonly IEnumerator _target;
        internal bool IsCompleted { get; private set; }
        internal Coroutine(IEnumerator target)
        {
            _target = target;
        }

        internal void Update()
        {
            if (!_canRun)
                return;

            if (_target.Current is YieldInstruction inst)
            {
                if (_currentInstruction != inst)
                {
                    _currentInstruction = inst;
                    _currentInstruction.OnBegin();
                }
                _currentInstruction.Update();

            }
            else if (_target.Current is Coroutine childCoroutine && !childCoroutine.IsCompleted)
            {
                // TODO: update children coroutine here.
                // childCoroutine.Update();
                return;
            }

            if (_currentInstruction == null || _currentInstruction.IsInstructionCompleted)
            {
                _currentInstruction = null;
                IsCompleted = _target.MoveNext() == false;
            }
        }

        internal virtual void Stop()
        {
            _canRun = false;
        }
    }
}
