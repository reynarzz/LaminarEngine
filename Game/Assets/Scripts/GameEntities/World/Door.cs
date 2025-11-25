using Engine;
using Engine.Types;
using Engine.Utils;
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
        private SpriteRenderer _spriteRenderer;

        public override void OnAwake()
        {
            base.OnAwake();

            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.Material = MaterialUtils.SpriteMaterial;

            var idleAnim = AnimatorUtils.AddState(_animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(_animator, "Open", false);
            var closeAnim = AnimatorUtils.AddState(_animator, "Close", false);

            var doorAtlas = Assets.GetTexture("KingsAndPigsSprites\\11-Door\\Opening (46x56).png");
            var spritesList = TextureAtlasUtils.SliceSprites(doorAtlas, 46, 56).ToList();
            spritesList.RemoveAt(0);

            var sprites = spritesList.ToArray();
            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, sprites[0]));
            openAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, sprites));
            closeAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, sprites.Reverse().ToArray()));
            _animator.OnUpdate += animator =>
            {
                _spriteRenderer.Sprite = animator.GetSprite("Sprite");
            };
        }

        private bool _isOpen = false;
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

        }
    }
}
