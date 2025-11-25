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
            var closeAnim = AnimatorUtils.AddState(Animator, "Close", false);

            var doorAtlas = Assets.GetTexture("KingsAndPigsSprites/11-Door/Opening (46x56).png");
        }
        public override bool TryInteract(Player player)
        {
            return false;
        }
    }
}