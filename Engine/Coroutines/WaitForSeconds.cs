using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class WaitForSeconds : YieldInstruction
    {
        private readonly float _seconds;
        private float _currentSeconds;
        public WaitForSeconds(float seconds)
        {
            _seconds = seconds;
            IsInstructionCompleted = false;
        }

        internal override void OnBegin()
        {
            _currentSeconds = _seconds;
            IsInstructionCompleted = false;
        }
        internal override void Update()
        {
            if (!IsInstructionCompleted)
            {
                _currentSeconds -= Time.DeltaTime;
                if (_currentSeconds <= 0)
                {
                    IsInstructionCompleted = true;
                }
            }
        }
    }
}
