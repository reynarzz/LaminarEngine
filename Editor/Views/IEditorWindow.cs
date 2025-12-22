using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal interface IEditorWindow
    {
        void OnOpen();
        void OnClose();
        void OnUpdate();
        void OnRender();
    }
}
