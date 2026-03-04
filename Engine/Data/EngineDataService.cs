using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data
{
    internal class EngineDataService : IEngineService
    {
        private ProjectSettingsData _projectData;
        public void Initialize(ProjectSettingsData projectData)
        {
            _projectData = projectData;
        }

        public ProjectSettingsData GetProjectSettings()
        {
            return _projectData;
        }
    }
}
