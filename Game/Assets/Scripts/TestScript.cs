using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class TestScript : ScriptBehavior
    {
        public class TestClassSub
        {
            public int ValueTest { get; set; }
            public string StringTest { get; set; }
            public TypeCode EnumTest { get; set; }
            public vec2 vec2Test { get; set; }
            public vec3 vec3Test { get; set; }
            public vec4 vec4Test { get; set; }
            public mat4 mat4Test { get; set; }
            public List<vec2> Vec2List { get; set; } = new List<vec2>();
            public EObject AnyEObjectTest { get; set; }
            public Actor ActorTest { get; set; }
            public SpriteRenderer RendererTest { get; set; }
            public RigidBody2D RigidBodyTest { get; set; }
            public Component AnyComponentTest { get; set; }
        }
        public class TestClass
        {
            public int ValueTest { get; set; }
            public string StringTest { get; set; }
            public TypeCode EnumTest { get; set; }
            public vec2 vec2Test { get; set; }
            public vec3 vec3Test { get; set; }
            public vec4 vec4Test { get; set; }
            public TestClassSub SubObject { get; set; } = new TestClassSub();
        }
        public TestClass TestClassObj { get; set; } = new TestClass();
        public List<string> StringTest { get; set; } = new();
        public List<int> IntTest { get; set; } = new();
        public List<Body2DType> EnumList { get; set; } = new();
        public Body2DType[] EnumArray { get; set; } = new Body2DType[5];
    }
}
