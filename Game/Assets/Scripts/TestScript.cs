using Engine;
using GlmNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game
{   
    public class TestScript : ScriptBehavior 
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
            [SerializedField] public Actor Actor { get; set; }
            [SerializedField] public Component Component { get; set; }
            [SerializedField] public TestClassSub TestClassSub { get; set; }
            [SerializedField] public List<Component> _TestComponentList { get; set; }
            [SerializedField] public List<Actor> _TestActorList { get; set; }


        }

        public struct AStruct
        {
            [SerializedField] public int IntValue;
            [SerializedField] public List<TestClass> _TestCLassList2 { get; set; } = new();
            [SerializedField] public List<Component> _TestComponentList { get; set; }
            [SerializedField] public TestClass TestClassObj = new TestClass();
            [SerializedField] public TestClassSub SubObject { get; set; } = new TestClassSub();

            //[ExposeEditorField] public List<string> StringTest { get; set; } = new();
            //[ExposeEditorField] public List<int> IntTest { get; set; } = new();
            //[ExposeEditorField] public List<Body2DType> EnumList { get; set; } = new();
            //[ExposeEditorField] public mat2 mat2Test { get; set; }
            //[ExposeEditorField] public mat3 mat3Test { get; set; }
            //[ExposeEditorField] public mat4 mat4Test { get; set; }
            private static Sprite sp = new Sprite("Sprite in dictionary", 0, Texture2D.White);
            [SerializedField]
            private Dictionary<int, Actor> _InStructeActorDictionary;
                // = new Dictionary<int, EObject> { { 2, null }, { 346, null } };

            [SerializedField]
            private Dictionary<int, Component> _InStructComponentDictionary;
                //= new Dictionary<int, AssetResourceBase> { { 11, null }, { 71, sp } };



            //[SerializedField]
            //private Dictionary<string, AssetResourceBase> _InStructassetDictionary =
            //   new Dictionary<string, AssetResourceBase> { { "asd", null }, { "adkaj", sp } };



            public AStruct()
            {

            }

            [ShowMethodInEditor]
            public void PrintDebugLog()
            {
                Debug.Log("Called from editor");
            }
        } 
        public class AAA<T, T2>
        {
            [SerializedField] public T TValue1;
            [SerializedField] public T2 TValue2;
              
        } 
        public class AAA<T>
        {
            [SerializedField] public T TValue; 
            [SerializedField] public EnumerablePartitionerOptions Enumn;
            [SerializedField] private int HotReloadThis;
            //[SerializedField] private RigidBody2D Something; 
            [SerializedField] private RigidBody2D _rigid2; 
            //[SerializedField] private Body2DType bod;
            [SerializedField] public AStruct[] AStructTypeArray { get; set; }
            [SerializedField] int x;
        }
        [SerializedField] AAA<int> AAAClass;
        [SerializedField] AAA<int, TestClass> AAAClass2v;
        [SerializedField] List<AAA<int>> AAAIntList;
        [SerializedField] List<AAA<string>> AAAStringList;
        [SerializedField] List<AAA<AStruct>> AAAStructList;

        [SerializedField] private quat Orientation;
        [SerializedField] public AStruct StructType { get; set; }
        [SerializedField] public List<AStruct> AStructTypeList { get; set; }
        [SerializedField] public AStruct[] AStructTypeArray { get; set; }
        [SerializedField][HideFromInspector] public List<TestClass> _TestCLassList { get; set; }
        [SerializedField] public TestClass TestClassObj;
        //[SerializedField] public List<string> StringTest;
        // [SerializedField] public List<int> IntTest { get; set; } = new();
        // [SerializedField] public List<Component> _TestComponentList { get; set; }
        // [SerializedField] public List<Component> _TestComponentArray { get; set; }
        // [SerializedField] public List<Actor> _TestActorList { get; set; }
        [SerializedField] public Actor[] _TestActorArray { get; set; }
        //[SerializedField] public double DoubleField { get; set; }
        //[SerializedField] public Component ComponentTest { get; set; }
        // [SerializedField] public Actor ActorTest { get; set; }
        //[ExposeEditorField] private List<List<int>> _nestedList;
        [PropertyHeader("Important fields")]
        //[SerializedField] public string String { get; set; }
        //[SerializedField] public IComponent ComponentInterface { get; set; }
        //[SerializedField] public IObject ObjectInterface { get; set; }
        //[SerializedField] public Body2DType[] EnumArray { get; set; }
        [SerializedField] public TestClass[] TestClassArray { get; set; }


        //[SerializedField]
        //private Dictionary<int, EObject> _eObjectDictionary =
        //    new Dictionary<int, EObject> { { 1, null }, { 35, null } };

        //[SerializedField]
        //private Dictionary<int, AssetResourceBase> _assetDictionary =
        //   new Dictionary<int, AssetResourceBase> { { 1, null }, { 22, new Sprite() } };


        [SerializedField]
        private Dictionary<int, AStruct> _complexDictionary =
           new Dictionary<int, AStruct> { { 1, default }, { 51, new AStruct() { IntValue = 100 } } };

        [SerializedField]
        private Dictionary<int, string> _intStringDictionary =
           new Dictionary<int, string> { { 3, "first value" }, { 51, "Second value"} };
         
        [SerializedField]
        private Dictionary<int, Component> _compStringDictionary =
           new Dictionary<int, Component> { { 3, default }, { 51, default } };

        [SerializedField]
        private Dictionary<int, quat> _quatDictionary =
          new Dictionary<int, quat> { { 3, default }, { 51, new quat(1, 0,0,0) } };


        [ShowMethodInEditor]
        public void AMethodCallingFromEditor()
        {
            Debug.Log("Called from editor");
        }
    }
}
