using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Chest : AnimatedInteractable
    {
        public override void OnAwake()
        {
            base.OnAwake();

            var idleAnim = AnimatorUtils.AddState(Animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(Animator, "Open", false);

            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(1, GameTextureAtlases.GetAtlas("chest_normal_idle")));
            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(1, GameTextureAtlases.GetAtlas("chest_normal_idle")));
        }
        public override bool TryInteract(Player player)
        {
            return false;
        }
    }
}