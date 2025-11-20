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
        private bool _advanced = false;
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
                if (_currentInstruction != inst || _advanced)
                {
                    _currentInstruction = inst;
                    _currentInstruction.OnBegin();
                }
                _currentInstruction.Update();

                _advanced = false;
            }
            else if (_target.Current is Coroutine childCoroutine && !childCoroutine.IsCompleted)
            {
                return;
            }

            if (_currentInstruction == null || _currentInstruction.IsInstructionCompleted)
            {
                Advance();
            }
        }

        private void Advance()
        {
            _advanced = true;
            IsCompleted = _target.MoveNext() == false;
        }

        internal virtual void Stop()
        {
            _canRun = false;
        }
    }
}
