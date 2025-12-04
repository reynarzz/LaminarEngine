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
    public class Door : AnimatedInteractable<DoorData>
    {
        public event Action<bool> OnDoorStateChanged;
        const int fps = 11;

        protected override void OnAwake()
        {
            base.OnAwake();

            BoxCollider.Size = new vec2(1.9f, 3);

            var idleAnim = AnimatorUtils.AddState(Animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(Animator, "Open", false);
            var closeAnim = AnimatorUtils.AddState(Animator, "Close", false);

            var openingSprites = GameTextures.GetAtlas("door_opening");
            var closingSprites = GameTextures.GetAtlas("door_closing");

            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, openingSprites[0]));
            openAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, openingSprites));
            closeAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, closingSprites));

            openAnim.Clip.AddEvent(openAnim.Clip.Duration, () =>
            {
                CameraShake.Instance.BurstShake(20, 0.1f, 0.1f);
                OnDoorStateChanged?.Invoke(true);
            });
            closeAnim.Clip.AddEvent(closeAnim.Clip.Duration, () =>
            {
                OnDoorStateChanged?.Invoke(false);
                CameraShake.Instance.BurstShake(20, 0.2f, 0.15f);
                if (Data.CurrentLevel != Data.TargetLevelIndex)
                {
                    FadeInOutManager.Instance.FadeIn(1.5f, () =>
                    {
                        GameManager.Instance.BuildLevel(Data.TargetLevelIndex);
                        FadeInOutManager.Instance.FadeOut(1.45f);

                    });
                }
            });

            // When player enters: Audio/Gameplay/Win_2.wav

            SpriteRenderer.Sprite = openingSprites[0];
            InteractableRenderer.Transform.LocalPosition += vec3.Up * 2.5f;
            Animator.OnUpdate += animator =>
            {
                SpriteRenderer.Sprite = animator.GetSprite("Sprite");
            };
        }

        public override bool TryInteract(Player player)
        {
            if (CanInteract(player))
            {
                if (Data.LockedBy == ItemId.none || player.Inventory.Use(Data.LockedBy))
                {
                    Open();
                    InteractableRenderVisible(false);
                    return true;
                }
            }
            return false;
        }

        public void Close()
        {
            Animator.Play("Close");

            // TODO: fade in/ fade out.

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
