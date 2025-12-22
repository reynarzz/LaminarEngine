using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Game
{
    internal class TestScript : ScriptBehavior
    {
        public List<string> StringTest { get; set; } = new();
        public List<int> IntTest { get; set; } = new();
        public List<Body2DType> EnumList { get; set; } = new();
        public Body2DType[] EnumArray { get; set; } = new Body2DType[5];
    }
}
