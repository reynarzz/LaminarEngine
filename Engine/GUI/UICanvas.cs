using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    // Note: Here to remember to build a UI system.
    public class UICanvas : UIElement, IUpdatableComponent
    {
        private Camera _internalCamera;
        public Camera Camera { get; set; }
        private List<UIElement> _elements;
        public UIDrawList DrawList = new();
        public List<UIElement> RootElements = new();
        public vec2 CanvasSize;

        public float Width { get; internal set; }
        public float Height { get; internal set; }
        public void Render()
        {
            DrawList.Clear();
            foreach (var e in RootElements)
            {
                RebuildRecursive(e, new vec2(0, 0), CanvasSize);
            }
        }

        void RebuildRecursive(UIElement element, vec2 parentPos, vec2 parentSize)
        {
            element.RectTransform.Recalculate(); 
            element.OnRebuild();
            element.Draw(this, DrawList);
            //foreach (var child in element.Children)
            //{
            //    RebuildRecursive(child, element.RectTransform.ComputedRect.Position, element.RectTransform.ComputedRect.Size);
            //}
        }

        private static void GetQuadVertices(RectTransform rt, ref vec2 bottomLeft,
                                                              ref vec2 topLeft,
                                                              ref vec2 topRight,
                                                              ref vec2 bottomRight)
        {
            var rect = rt.ComputedRect;
            var size = rect.Size;
            var pivot = rt.Pivot;

            vec2 pivotOffset = new vec2(size.x * pivot.x, size.y * pivot.y);

            bottomLeft = new vec2(-pivotOffset.x, -pivotOffset.y);
            topLeft = new vec2(-pivotOffset.x, size.y - pivotOffset.y);
            topRight = new vec2(size.x - pivotOffset.x, size.y - pivotOffset.y);
            bottomRight = new vec2(size.x - pivotOffset.x, -pivotOffset.y);

            bottomLeft += rect.Min;
            topLeft += rect.Min;
            topRight += rect.Min;
            bottomRight += rect.Min;
        }

        public void OnUpdate()
        {
        }
    }
}