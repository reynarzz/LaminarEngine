using Editor.Build;
using Editor.Drawers;
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
    internal class iOSBuildDrawer : PlatformBuildSettingsDrawer
    {
        private readonly MemberInfo[] _properties;
        public iOSBuildDrawer()
        {
            var type = typeof(iOSBuildSettings);
            _properties = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(type, true, true).ToArray();
        }
        public override void OnDraw(PlatformBuildSettings settings)
        {
            foreach (var member in _properties)
            {
                PropertiesGUIDrawEditor.DrawVars("iOSBuildSettings", settings, member);
            }
        }
    }
}
