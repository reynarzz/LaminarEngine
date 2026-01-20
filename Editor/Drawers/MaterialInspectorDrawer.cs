using Engine;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class MaterialInspectorDrawer : IDrawerEditor<Material>
    {
        private readonly static Type[] _visibilityAttributes = [typeof(SerializedFieldAttribute), typeof(ShowFieldNoSerialize)];
        public bool AutoDrawTitle => true;

        public void OnOpen()
        {
        }

        public void OnClose()
        {
        }

        public void OnDraw(Material target)
        {
            var members = ReflectionUtils.GetAllMembersWithAttributes(target.GetType(), _visibilityAttributes, true, true);
            int index = 0;
            foreach (var member in members)
            {
                PropertyDrawer.DrawVars(target.GetID().ToString(), target, member, 0, index, 0, true);
                index++;
            }

            // PropertyDrawer.DrawMethods(target, target.GetID().ToString());
        }
    }
}
