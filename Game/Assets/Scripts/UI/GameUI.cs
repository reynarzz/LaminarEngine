using Engine;
using Engine.GUI;
using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(UICanvas))]
    public class GameUI : ScriptBehavior
    {
        [RequiredProperty] public UICanvas Canvas { get; private set; }
    }
}
