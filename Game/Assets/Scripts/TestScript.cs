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
            [ExposeEditorField] public int ValueTest { get; set; }
            [ExposeEditorField] public string StringTest { get; set; }
            [ExposeEditorField] public TypeCode EnumTest { get; set; }
            [ExposeEditorField] public vec2 vec2Test { get; set; }
            [ExposeEditorField] public vec3 vec3Test { get; set; }
            [ExposeEditorField] public vec4 vec4Test { get; set; }
            [ExposeEditorField] public mat2 mat2Test { get; set; }
            [ExposeEditorField] public mat3 mat3Test { get; set; }
            [ExposeEditorField] public mat4 mat4Test { get; set; }
            [ExposeEditorField] public List<vec2> Vec2List { get; set; } = new List<vec2>();
            [ExposeEditorField] public List<Body2DType> EnumList { get; set; } = new();

            [ExposeEditorField] public EObject AnyEObjectTest { get; set; }
            [ExposeEditorField] public Actor ActorTest { get; set; }
            [ExposeEditorField] public SpriteRenderer RendererTest { get; set; }
            [ExposeEditorField] public RigidBody2D RigidBodyTest { get; set; }
            [ExposeEditorField] public Component AnyComponentTest { get; set; }
        }
        public class TestClass
        {
            [ExposeEditorField] public int ValueTest { get; set; }
            [ExposeEditorField] public string StringTest { get; set; }
            [ExposeEditorField] public TypeCode EnumTest { get; set; }
            [ExposeEditorField] public vec2 vec2Test { get; set; }
            [ExposeEditorField] public vec3 vec3Test { get; set; }
            [ExposeEditorField] public vec4 vec4Test { get; set; }
            [ExposeEditorField] public mat2 mat2Test { get; set; }
            [ExposeEditorField] public mat3 mat3Test { get; set; }
            [ExposeEditorField] public mat4 mat4Test { get; set; }
            [ExposeEditorField] public TestClassSub SubObject { get; set; } = new TestClassSub();
        }

        public struct AStruct
        {
            [ExposeEditorField] public int Value;
            [ExposeEditorField] public List<TestClass> _TestCLassList { get; set; } = new();
            [ExposeEditorField] public TestClass TestClassObj = new TestClass();
            //[ExposeEditorField] public List<string> StringTest { get; set; } = new();
            //[ExposeEditorField] public List<int> IntTest { get; set; } = new();
            //[ExposeEditorField] public List<Body2DType> EnumList { get; set; } = new();
            //[ExposeEditorField] public mat2 mat2Test { get; set; }
            //[ExposeEditorField] public mat3 mat3Test { get; set; }
            //[ExposeEditorField] public mat4 mat4Test { get; set; }
            public AStruct()
            {
                
            }
        }
        //[ExposeEditorField] public AStruct StructType { get; set; }
        [ExposeEditorField] public List<TestClass> _TestCLassList { get; set; }
        [ExposeEditorField] public TestClass TestClassObj;
        [ExposeEditorField] public List<string> StringTest;
        //[ExposeEditorField] public List<int> IntTest { get; set; } = new();
        //[ExposeEditorField] public List<Body2DType> EnumList { get; set; } = new();
        //[ExposeEditorField] public mat2 mat2Test { get; set; }
        //[ExposeEditorField] public mat3 mat3Test { get; set; }
        //[ExposeEditorField] public mat4 mat4Test { get; set; }
        //[ExposeEditorField] private List<List<int>> _nestedList;

        [ExposeEditorField] public Body2DType[] EnumArray { get; set; }
        [ExposeEditorField] public TestClass[] ClassArray { get; set; } 
    }
}
