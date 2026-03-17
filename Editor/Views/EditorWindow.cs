using Engine;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal abstract class EditorWindow
    {
        private bool _isOpened = true;
        private bool _needsToCallEndWindow;
        public string WindowName { get; protected set; }
        public bool IsWindowFocused { get; private set; }
        public bool IsWindowHovered { get; private set; }

        private readonly string _menuItemPath;
        private bool _isInMenuItem = false;
        private bool _onOpen = false;

        protected EditorWindow()
        {
        }

        protected EditorWindow(string menuItemPath)
        {
            _menuItemPath = menuItemPath;
            _isInMenuItem = true;
            EditorMenu.PushMenu(_menuItemPath, OnMenuChanged/*, true*/);
            EditorMenu.Toggle(_menuItemPath, _isOpened);
        }

        private void OnMenuChanged()
        {
            Open();

            //if (_isOpened)
            //{
            //    Close();
            //}
            //else
            //{
            //    Open();
            //}
        }

        public void Open()
        {
            _isOpened = true;
            if (_isInMenuItem)
            {
                EditorMenu.Toggle(_menuItemPath, true);
            }

            OnOpen();
        }

        public void Close()
        {
            _isOpened = false;
            _onOpen = false;
            if (_isInMenuItem)
            {
                EditorMenu.Toggle(_menuItemPath, false);
            }
            OnClose();
        }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        public virtual void OnUpdate() { }
        public abstract void OnDraw();

        protected bool OnBeginWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool canClose = true, vec2? windowsPadding = default)
        {
            if (!_isOpened)
                return false;

            if (!_onOpen)
            {
                _onOpen = true;
                OnOpen();
            }
            if (!_needsToCallEndWindow)
            {
                _needsToCallEndWindow = true;
            }
            else
            {
                Debug.Error($"Window error: Needs to call {nameof(OnEndWindow)}");
            }
            if (windowsPadding != null)
            {
                var val = windowsPadding.GetValueOrDefault();
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(val.x, val.y));
            }
            if (canClose)
            {
                if (flags == ImGuiWindowFlags.None)
                {
                    ImGui.Begin(name, ref _isOpened);
                }
                else
                {
                    ImGui.Begin(name, ref _isOpened, flags);
                }

                IsWindowHovered = ImGui.IsWindowHovered();

                if (ImGui.IsAnyMouseClicked() && IsWindowHovered)
                {
                    ImGui.SetWindowFocus();
                }


                if (!_isOpened)
                {
                    Close();
                }

                if (windowsPadding != null)
                {
                    ImGui.PopStyleVar();
                }
                IsWindowFocused = ImGui.IsWindowFocused();

                return _isOpened;
            }

            if (flags == ImGuiWindowFlags.None)
            {
                ImGui.Begin(name);
            }
            else
            {
                ImGui.Begin(name, flags);
            }

            IsWindowHovered = ImGui.IsWindowHovered();

            if (ImGui.IsAnyMouseClicked() && IsWindowHovered)
            {
                ImGui.SetWindowFocus();
            }

            IsWindowFocused = ImGui.IsWindowFocused();

            if (windowsPadding != null)
            {
                ImGui.PopStyleVar();
            }
            return true;
        }

        protected void OnEndWindow()
        {
            if (_needsToCallEndWindow)
            {
                ImGui.End();
            }


            _needsToCallEndWindow = false;
        }
    }
}
