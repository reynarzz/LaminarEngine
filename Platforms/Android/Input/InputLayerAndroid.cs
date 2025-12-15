using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Android
{
    internal class InputLayerAndroid : InputLayerBase
    {
        public override TouchInput Touch => new TouchInput();

        public override vec2 MousePosition => default;


        public override void Initialize()
        {
        }

        internal override void UpdateLayer()
        {
            base.UpdateLayer();

        }

        public override bool GetKey(KeyCode key)
        {
            return false;
        }

        public override bool GetKeyDown(KeyCode key)
        {
            return false;
        }

        public override bool GetKeyUp(KeyCode key)
        {
            return false;
        }

        public override bool GetMouse(MouseButton button)
        {
            return false;
        }

        public override bool GetMouseDown(MouseButton button)
        {
            return false;
        }

        public override bool GetMouseUp(MouseButton button)
        {
            return false;
        }

        public override void Close()
        {

        }
    }
}
