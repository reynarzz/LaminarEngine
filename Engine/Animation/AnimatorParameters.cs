using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorParameters
    {
        [SerializedField] private Dictionary<string, float> _floats = new();
        [SerializedField] private Dictionary<string, int> _ints = new();
        [SerializedField] private Dictionary<string, bool> _bools = new();
        [SerializedField] private Dictionary<string, bool> _triggers = new();

        public void SetInt(string name, int value) { _ints[name] = value; }
        public int GetInt(string name) { return _ints.TryGetValue(name, out var v) ? v : 0; }
        public void SetFloat(string name, float value) { _floats[name] = value; }
        public float GetFloat(string name) { return _floats.TryGetValue(name, out var v) ? v : 0f; }
        public void SetBool(string name, bool value) { _bools[name] = value; }
        public bool GetBool(string name) { return _bools.TryGetValue(name, out var v) && v; }
        public void SetTrigger(string name) { _triggers[name] = true; }

        public bool GetTrigger(string name)
        {
            if (HasTrigger(name))
            {
                _triggers[name] = false;
                return true;
            }
            return false;
        }

        public bool HasTrigger(string name)
        {
            if (_triggers.TryGetValue(name, out var val))
            {
                return val;
            }
            return false;
        }
    }
}
