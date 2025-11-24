using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public class PostProcessingStack
    {
        private static List<PostProcessingPass> _passes = new();
        public static IReadOnlyList<PostProcessingPass> Passes => _passes;

        public static void Push(PostProcessingPass pass)
        {
            _passes.Add(pass);
        }
        public static void Insert(PostProcessingPass pass, int index)
        {
            _passes.Insert(index, pass);
        }

        public static void Pop(PostProcessingPass pass)
        {
            _passes.Remove(pass);
        }

        public static void Clear()
        {
            _passes.Clear();
        }
    }
}
