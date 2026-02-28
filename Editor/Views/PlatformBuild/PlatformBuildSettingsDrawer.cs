using Editor.Build;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal abstract class PlatformBuildSettingsDrawer
    {
        public abstract void OnDraw(PlatformBuildSettings settings);
    }
}
