using Engine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal abstract class ProjectMenuDrawer
    {
        protected ProjectMenuDrawer()
        {
        }

        public void OnDraw()
        {
            OnDraw(EngineServices.GetService<EngineDataService>().GetProjectData());
        }

        protected abstract void OnDraw(ProjectSettings settings);
    }
}
