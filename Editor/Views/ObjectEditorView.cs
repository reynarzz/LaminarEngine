using Editor.Drawers;
using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using GlmNet;
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
    internal class ObjectEditorView : EditorWindow
    {
        private readonly Dictionary<Type, EditorDrawerBase> _drawers;
        private EditorDrawerBase _prevSelected;
        private const string NothingSelectedLabel = "Nothing is selected.";
        private bool _isTextSizeCalculated;
        private Vector2 _textSize;
        private readonly DefaultInspectorDrawer _defaultDrawer;

        public ObjectEditorView() : base("Window/Object Editor")
        {
            _drawers = new()
            {
                { typeof(Actor), new ActorInspectorDrawer() },
                { typeof(Material), new MaterialInspectorDrawer() },
                { typeof(Texture2D), new TextureInspectorDrawer() },
                { typeof(TilemapAsset), new TilemapInspectorDrawer() },
                { typeof(TextAsset), new TextInspectorDrawer() },
                { typeof(Sprite), new SpriteInspectorDrawer() },
                
            };

            _defaultDrawer = new();
        }

        public override void OnDraw()
        {
            var padding = GetDrawerWindowsPadding();
            if (OnBeginWindow("Object Editor", ImGuiWindowFlags.None, true, padding))
            {
                if (Selector.Selected)
                {
                    var transform = Selector.Transform;

                    if (transform?.Actor)
                    {
                        if (_drawers.TryGetValue(typeof(Actor), out var drawer))
                        {
                            InitDrawer(drawer);
                            drawer.OnDraw(transform?.Actor);
                        }
                        else
                        {
                            Debug.Error($"Type for editor view: '{Selector.Selected.GetType()}' is not implemented");
                            Clear();
                        }
                    }
                    else if (_drawers.TryGetValue(Selector.Selected.GetType(), out var drawer))
                    {
                        InitDrawer(drawer);
                        drawer.OnDraw(Selector.Selected);
                    }
                    else
                    {
                        _defaultDrawer.OnDraw(Selector.Selected);
                    }
                }
                else
                {
                    Clear();
                }
            }

            OnEndWindow();
        }

        private vec2? GetDrawerWindowsPadding()
        {
            if (Selector.Selected)
            {
                var transform = Selector.Transform;

                var type = transform?.Actor ? typeof(Actor) : Selector.Selected.GetType();
                if (_drawers.TryGetValue(type, out var drawer))
                {
                    return drawer.WindowsPadding;
                }
            }
            return null;
        }
        private void InitDrawer(EditorDrawerBase drawer)
        {
            if (_prevSelected != drawer)
            {
                if (_prevSelected != null)
                {
                    _prevSelected.OnClose();
                }
                _prevSelected = drawer;
                drawer.OnOpen(Selector.Selected);
            }
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
    }
}