using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorParameters
    {
        [SerializedField, HideFromInspector, JsonProperty] private Dictionary<string, float> _floats = new();
        [SerializedField, HideFromInspector, JsonProperty] private Dictionary<string, int> _ints = new();
        [SerializedField, HideFromInspector, JsonProperty] private Dictionary<string, bool> _bools = new();
        [SerializedField, HideFromInspector, JsonProperty] private HashSet<string> _triggers = new();

        public void SetInt(string name, int value) { _ints[name] = value; }
        public int GetInt(string name) => _ints.TryGetValue(name, out var v) ? v : 0;


        public void SetFloat(string name, float value) { _floats[name] = value; }
        public float GetFloat(string name) => _floats.TryGetValue(name, out var v) ? v : 0f;
        
        public void SetBool(string name, bool value) { _bools[name] = value; }
        public bool GetBool(string name) { return _bools.TryGetValue(name, out var v) && v; }

        public void SetTrigger(string name) { _triggers.Add(name); }
        public bool GetTrigger(string name)
        {
            if (HasTrigger(name))
            {
                _triggers.Remove(name);
                return true;
            }
            return false;
        }

        public bool HasTrigger(string name)
        {
            return _triggers.Contains(name);
        }
    }
}
