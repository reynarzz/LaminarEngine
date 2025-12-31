using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal interface IDrawerEditor
    {
        void OnOpen();
        void OnClose();
        void OnDraw();
    }
}
