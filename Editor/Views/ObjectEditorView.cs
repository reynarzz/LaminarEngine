using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class ObjectEditorView : IEditorWindow
    {
        private readonly Dictionary<Type, IDrawerEditor> _drawers;
        private IDrawerEditor _prevSelected;
        private const string NothingSelectedLabel = "Nothing is selected.";
        private bool _isTextSizeCalculated;
        private Vector2 _textSize;

        public ObjectEditorView()
        {
            _drawers = new()
            {
                { typeof(Actor), new ActorInspectorDrawer() }
            };
        }

        public void OnOpen()
        {

        }

        public void OnDraw()
        {
            ImGui.Begin("Object Editor");

            if (Selector.Selected)
            {
                var transform = Selector.SelectedTransform();

                if (transform?.Actor)
                {
                    if (_drawers.TryGetValue(typeof(Actor), out var drawer))
                    {
                        if (_prevSelected != Selector.Selected)
                        {
                            if (_prevSelected != null)
                            {
                                _prevSelected.OnClose();
                            }
                            _prevSelected = drawer;
                            drawer.OnOpen();
                        }
                        drawer.OnDraw();
                    }
                    else
                    {
                        Debug.Error($"Type for editor view: '{Selector.Selected.GetType()}' is not implemented");
                        Clear();
                    }
                }
                else
                {
                    Clear();
                }
            }
            else
            {
                Clear();
            }

            ImGui.End();
        }

        private void Clear()
        {
            if (_prevSelected != null)
            {
                _prevSelected.OnClose();
                _prevSelected = null;
            }

            var size = ImGui.GetContentRegionAvail();

            if (!_isTextSizeCalculated)
            {
                _isTextSizeCalculated = true;
                _textSize = ImGui.CalcTextSize(NothingSelectedLabel);
            }

            ImGui.SetCursorPos(new Vector2(size.X / 2.0f - _textSize.X / 2.0f, size.Y / 2.0f - _textSize.Y / 2.0f));
            ImGui.Text(NothingSelectedLabel);
        }

        public void OnUpdate()
        {

        }

        public void OnClose()
        {

        }
    }
}