using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data
{
    internal class EngineDataService : IEngineService
    {
        private ProjectSettings _projectData;
        public void Initialize(ProjectSettings projectData)
        {
            _projectData = projectData;
        }

        public ProjectSettings GetProjectData()
        {
            return _projectData;
        }
    }
}
