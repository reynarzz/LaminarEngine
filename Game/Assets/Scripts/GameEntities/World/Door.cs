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
    [RequiredComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class Door : InteractableEntityBase
    {
        private Animator _animator;
        private Predicate<Player> _conditionToOpen;
        private bool _isOpen = false;

        public override void OnAwake()
        {
            base.OnAwake();

            _animator = GetComponent<Animator>();
            BoxCollider.Size = new vec2(1.9f, 3);

            var idleAnim = AnimatorUtils.AddState(_animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(_animator, "Open", false);
            var closeAnim = AnimatorUtils.AddState(_animator, "Close", false);

            var doorAtlas = Assets.GetTexture("KingsAndPigsSprites/11-Door/Opening (46x56).png");
            var spritesList = TextureAtlasUtils.SliceSprites(doorAtlas, 46, 56).ToList();
            spritesList.RemoveAt(0);

            var sprites = spritesList.ToArray();
            const int fps = 11;

            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, sprites[0]));
            openAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, sprites));
            closeAnim.Clip.AddCurve("Sprite", new SpriteCurve(fps, sprites.Reverse().ToArray()));

            // When player enters: Audio/Gameplay/Win_2.wav
            _animator.OnUpdate += animator =>
            {
                SpriteRenderer.Sprite = animator.GetSprite("Sprite");
            };

            var a = Actor;

        }

        public void SetConditionToOpen(Predicate<Player> condition)
        {
            _conditionToOpen = condition;
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                _animator.Play(_isOpen ? "Close" : "Open");
                _isOpen = !_isOpen;
            }
        }

        protected override void OnPlayerInteractZone(bool enter, Player player)
        {
            Debug.Log("Player enter: " + enter);
        }

        public override bool CanInteract(Player player)
        {
            return _conditionToOpen?.Invoke(player) ?? true;
        }

        public override void TryInteract()
        {
            if (!_isOpen)
            {
                _isOpen = true;
                _animator.Play("Open");
            }
        }

        public void Close()
        {
            _isOpen = false;
            _animator.Play("Close");
        }
    }
}
