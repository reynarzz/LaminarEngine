using Editor.Build;
using Engine;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class WindowsBuildDrawer : PlatformBuildSettingsDrawer
    {
        private readonly MemberInfo[] _properties;
        public WindowsBuildDrawer()
        {
            var type = typeof(WindowsBuildSettings);
            _properties = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(type, true, true).ToArray();
        }
        public override void OnDraw(PlatformBuildSettings settings)
        {
            foreach (var member in _properties)
            {
                PropertiesGUIDrawEditor.DrawVars("WindowsBuildSettings", settings, member);
            }
        }
    }
}
