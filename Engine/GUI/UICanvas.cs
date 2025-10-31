using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    // Note: Here to remember to build a UI system.
    public class UICanvas : Component, IUpdatableComponent
    {
        private Camera _internalCamera;
        public Camera Camera { get; set; }
        private List<UIElement> _uiElements;
        public float Width { get; internal set; } = 512 * 2;
        public float Height { get; internal set; } = 288 * 2;
        internal override void OnInitialize()
        {
            _uiElements = new();
        }
        void RebuildRecursive(UIElement element, vec2 parentPos)
        {
            if (!element)
                return;

            element.RectTransform.Recalculate(this);
            element.OnRebuild();

            foreach (var child in element.Transform.Children)
            {
                RebuildRecursive(child.GetComponent<UIElement>(), element.RectTransform.AnchoredPosition);
            }

            if (element is UIGraphicsElement graphics)
            {
                graphics.OnCanvasDraw(this);
            }
        }

        public void OnUpdate()
        {
            _uiElements.Clear();
            SceneManager.ActiveScene.FindAll(_uiElements, false, Actor);

            foreach (var elements in _uiElements)
            {
                RebuildRecursive(elements, new vec2(0, 0));
            }
        }

    }
}