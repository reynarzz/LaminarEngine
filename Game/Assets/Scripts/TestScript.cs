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
            [SerializedField] public int ValueTest { get; set; }
            [SerializedField] public string StringTest { get; set; }
            [SerializedField] public TypeCode EnumTest { get; set; }
            [SerializedField] public vec2 vec2Test { get; set; }
            [SerializedField] public vec3 vec3Test { get; set; }
            [SerializedField] public vec4 vec4Test { get; set; }
            [SerializedField] public mat2 mat2Test { get; set; }
            [SerializedField] public mat3 mat3Test { get; set; }
            [SerializedField] public mat4 mat4Test { get; set; }
            [SerializedField] public List<vec2> Vec2List { get; set; } = new List<vec2>();
            [SerializedField] public List<Body2DType> EnumList { get; set; } = new();

            [SerializedField] public EObject AnyEObjectTest { get; set; }
            [SerializedField] public Actor ActorTest { get; set; }
            [SerializedField] public SpriteRenderer RendererTest { get; set; }
            [SerializedField] public RigidBody2D RigidBodyTest { get; set; }
            [SerializedField] public Component AnyComponentTest { get; set; }
        }
        public class TestClass
        {
            [SerializedField] public int IntTest { get; set; }
            [SerializedField] public uint UIntTest { get; set; }
            [SerializedField] private long LongTest = 987654321012345678;
            [SerializedField] private ulong ULongTest = 18446744073709551615UL;
            [SerializedField] private double DoubleTest = 0;
            [SerializedField] public string StringTest { get; set; }
            [SerializedField] public TypeCode EnumTest { get; set; }
            [SerializedField] public vec2 vec2Test { get; set; }
            [SerializedField] public vec3 vec3Test { get; set; }
            [SerializedField] public vec4 vec4Test { get; set; }
            [SerializedField] public mat2 mat2Test { get; set; }
            [SerializedField] public mat3 mat3Test { get; set; }
            [SerializedField] public mat4 mat4Test { get; set; }
        }

        public struct AStruct
        {
            [SerializedField] public int Value;
            [SerializedField] public List<TestClass> _TestCLassList { get; set; } = new();
            [SerializedField] public List<Component> _TestComponentList { get; set; } 
            [SerializedField] public TestClass TestClassObj = new TestClass();
            [SerializedField] public TestClassSub SubObject { get; set; } = new TestClassSub();

            //[ExposeEditorField] public List<string> StringTest { get; set; } = new();
            //[ExposeEditorField] public List<int> IntTest { get; set; } = new();
            //[ExposeEditorField] public List<Body2DType> EnumList { get; set; } = new();
            //[ExposeEditorField] public mat2 mat2Test { get; set; }
            //[ExposeEditorField] public mat3 mat3Test { get; set; }
            //[ExposeEditorField] public mat4 mat4Test { get; set; }
            public AStruct()
            {
                
            }

            [ShowMethodInEditor]
            public void PrintDebugLog()
            {
                Debug.Log("Called from editor");
            }
        }
        [SerializedField] public AStruct StructType { get; set; }
        [SerializedField] public List<AStruct> AStructTypeList { get; set; }
        [SerializedField] [HideFromInspector] public List<TestClass> _TestCLassList { get; set; }
        [SerializedField] public TestClass TestClassObj;
        [SerializedField] public List<string> StringTest;
        [SerializedField] public List<int> IntTest { get; set; } = new();
        [SerializedField] public List<Component> _TestComponentList { get; set; }
        [SerializedField] public List<Component> _TestComponentArray { get; set; }
        [SerializedField] public List<Actor> _TestActorList { get; set; }
        [SerializedField] public Actor[] _TestActorArray { get; set; }
        [SerializedField] public double DoubleField { get; set; }
        [SerializedField] public Component ComponentTest { get; set; }
        [SerializedField] public Actor ActorTest { get; set; }
        //[ExposeEditorField] public List<Body2DType> EnumList { get; set; } = new();
        //[ExposeEditorField] public mat2 mat2Test { get; set; }
        //[ExposeEditorField] public mat3 mat3Test { get; set; }
        //[ExposeEditorField] public mat4 mat4Test { get; set; }
        //[ExposeEditorField] private List<List<int>> _nestedList;
        [PropertyHeader("Important fields")]
        [SerializedField] public string String { get; set; }
        [SerializedField] public IComponent ComponentInterface { get; set; }
        [SerializedField] public IObject ObjectInterface { get; set; }
        [SerializedField] public Body2DType[] EnumArray { get; set; }
        [SerializedField] public TestClass[] TestClassArray { get; set; }


        [ShowMethodInEditor]
        public void AMethodCallingFromEditor()
        {
            Debug.Log("Called from editor");
        }
    }
}
