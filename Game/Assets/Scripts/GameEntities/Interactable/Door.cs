using Engine;
using Engine.Types;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(Animator))]
    public class Door : AnimatedInteractable<DoorData>
    {
        public event Action<bool> OnDoorStateChanged;
        const int fps = 11;

        public override void OnAwake()
        {
            base.OnAwake();

            BoxCollider.Size = new vec2(1.9f, 3);

            var idleAnim = AnimatorUtils.AddState(Animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(Animator, "Open", false);
            var closeAnim = AnimatorUtils.AddState(Animator, "Close", false);

            var openingSprites = GameTextureAtlases.GetAtlas("door_opening");
            var closingSprites = GameTextureAtlases.GetAtlas("door_closing");

            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, openingSprites[0]));
            openAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, openingSprites));
            closeAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, closingSprites));

            openAnim.Clip.AddEvent(openAnim.Clip.Duration, () => OnDoorStateChanged?.Invoke(true));
            closeAnim.Clip.AddEvent(closeAnim.Clip.Duration, () => OnDoorStateChanged?.Invoke(false));

            // When player enters: Audio/Gameplay/Win_2.wav
            Animator.OnUpdate += animator =>
            {
                SpriteRenderer.Sprite = animator.GetSprite("Sprite");
            };
        }

        public override bool TryInteract(Player player)
        {
            if (CanInteract(player))
            {
                Open();
                return true;
            }
            return false;
        }

        public void Close()
        {
            Animator.Play("Close");
        }

        public void Open()
        {
            Animator.Play("Open");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnDoorStateChanged = null;
        }
    }
}
