using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class EngineInfo
    {
        public class Renderer
        {
            public static int WBatches { get; internal set; }
            public static int GrabScreenPass { get; internal set; }
            public static int UIBatches { get; internal set; }
            public static int UIGrabScreenPass { get; internal set; }
            public static int UIDrawCalls => UIBatches * (UIGrabScreenPass + 1);
            public static int WDrawCalls => WBatches * (GrabScreenPass + 1);
            public static int PostProcessingPasses { get; internal set; }
            public static int TotalDrawCalls => WDrawCalls + UIDrawCalls + PostProcessingPasses;
            public static int TotalBatches => WBatches + UIBatches;

            public static int SavedByBatching { get; internal set; }

            internal static void Clear()
            {
                WBatches = 0;
                GrabScreenPass = 0;
                UIBatches = 0;
            }
        }

        public static string RendererInfoToString()
        {
            return $"WBatches:{Renderer.WBatches} | WScreenGrabs:{Renderer.GrabScreenPass} | W_DC:{Renderer.WDrawCalls} " +
                $"| UIBatches:{Renderer.UIBatches} | UIScreenGrabs:{Renderer.UIGrabScreenPass} | UI_DC:{Renderer.UIDrawCalls} " +
                $"| AllBatches:{Renderer.TotalBatches} | All_DCs:{Renderer.TotalDrawCalls} | W_DC_BatchSaved:{Renderer.SavedByBatching}";
        }
    }
}