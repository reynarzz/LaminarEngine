using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class AnimationWindow : EditorWindow
    {
        public AnimationWindow() : base("Window/Animation") 
        {
            
        }

        private List<MultiTrackTimelineEditor.Track> _tracks = new List<MultiTrackTimelineEditor.Track>()
        {
            new MultiTrackTimelineEditor.Track
            {
                Name = "Position",
                Keys = new List<MultiTrackTimelineEditor.Keyframe>
                {
                    new MultiTrackTimelineEditor.Keyframe { Time = 0.5f },
                    new MultiTrackTimelineEditor.Keyframe { Time = 1.5f },
                }
            },
            new MultiTrackTimelineEditor.Track
            {
                Name = "Rotation",
                Keys = new List<MultiTrackTimelineEditor.Keyframe>
                {
                    new MultiTrackTimelineEditor.Keyframe { Time = 0.3f },
                    new MultiTrackTimelineEditor.Keyframe { Time = 2.0f },
                }
            }
        };

        public override void OnDraw()
        {
            if (OnBeginWindow("Animation", ImGuiNET.ImGuiWindowFlags.None, true, new GlmNet.vec2()))
            {
                MultiTrackTimelineEditor.Draw(ref _tracks);
            }

            OnEndWindow();
        }
    }
}
