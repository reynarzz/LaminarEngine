using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal interface ExecutableEntry
    {
        private static ApplicationLayer _appLayer;
        internal static ApplicationLayer GetApplicationLayer()
        {
#if SHIP_BUILD
            if (_appLayer == null)
            {
                _appLayer = (ApplicationLayer)ReflectionUtils.GetDefaultValueInstance(TypeResolver.GetApplicationLayerTypeShip());
            }

            return _appLayer;
#else
            throw new InvalidOperationException($"The method '{nameof(GetApplicationLayer)}' should only be called in ship builds.");
#endif
        }
    }
}
