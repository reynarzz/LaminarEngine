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
    internal class AnimationTest : ScriptBehavior
    {
        private Animator _controller;
        private SpriteRenderer _targetRenderer;

        public override void OnStart()
        {
            _controller = AddComponent<Animator>();
            _targetRenderer = AddComponent<SpriteRenderer>();
            _targetRenderer.Material = new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text));
            _targetRenderer.SortOrder = 7;


            PositionAnimation();
        }


        private void PositionAnimation()
        {
            //_targetRenderer.Transform.WorldPosition = new vec3(5, 1, 0) + Transform.WorldPosition;

            var animClip = new AnimationClip("Position anim");

            var posCurve = new Vec2EasingCurve();
            posCurve.AddKeyFrame(0, Transform.WorldPosition + new GlmNet.vec3());
            posCurve.AddKeyFrame(1, Transform.WorldPosition + new GlmNet.vec3(0, 1));
            posCurve.AddKeyFrame(2, Transform.WorldPosition + new GlmNet.vec3(1, 1));
            posCurve.AddKeyFrame(3, Transform.WorldPosition + new GlmNet.vec3(1, -1));
            posCurve.AddKeyFrame(4, Transform.WorldPosition + new GlmNet.vec3());
            //posCurve.AutoSmoothTangents();
            posCurve.EasingType = EasingType.EaseOutBounce;

            var colorCurve = new ColorHermiteCurve();
            colorCurve.AddKeyFrame(0, Color.Red);
            colorCurve.AddKeyFrame(2, Color.Cyan);
            colorCurve.AddKeyFrame(4, Color.Red);
            colorCurve.AutoSmoothTangents();


            var basePath = "KingsAndPigsSprites/03-Pig/";
            var pTexture2 = Assets.GetTexture(basePath + "Attack (34x28).png");
            var sprites = TextureAtlasUtils.SliceSprites(pTexture2, 34, 28, new vec2(0.4f, 0.4f));
            pTexture2.PixelPerUnit = 16;

            var spriteCurve = new SpriteCurve();
            float fps = 11;

            for (int i = 0; i < sprites.Length; i++)
            {
                spriteCurve.AddKeyFrame((1.0f / fps) * (float)i, sprites[i]);
            }


            animClip.AddCurve("Position", posCurve);
            //animClip.AddCurve("Color", colorCurve);
            animClip.AddCurve("Sprite", spriteCurve);
            Debug.Log("Anim clip duration: " + animClip.Duration);
            var state = new AnimationState("Main state", animClip);
            _controller.SetState(state);
        }

        private void AnotherTest()
        {
            var walkClip = new AnimationClip("Walk", true);

            SpriteCurve curve = new SpriteCurve();
            curve.AddKeyFrame(0, new Sprite());
            var hermite = new Vec2HermiteCurve();
            hermite.AutoSmoothTangents();

            walkClip.AddCurve("Sprite", curve);
            walkClip.AddCurve("Sprite", hermite);

            var a = new EventCurve();
            a.AddKeyFrame(5, () => { });
            walkClip.AddEvent(a);

            var runClip = new AnimationClip("Run", true);

            var walkState = new AnimationState("Walk", walkClip);
            var runState = new AnimationState("Run", runClip);

            walkState.AddTransition(new AnimatorTransition("Run", x => x.GetFloat("Speed") > 1.0f));

            runState.AddTransition(new AnimatorTransition("Walk", x => x.GetFloat("Speed") <= 1.0f));

            _controller.AddState(walkState);
            _controller.AddState(runState);

            _controller.SetState("Walk");

        }

        public override void OnUpdate()
        {
            _controller.Parameters.SetFloat("Speed", 2.0f);

            var posX = _controller.GetFloat("PositionX");

            var position = _controller.GetVec2("Position");
            var color = _controller.GetColor("Color");
            var sprite = _controller.GetSprite("Sprite");

            // _targetRenderer.Color = color;
             _targetRenderer.Transform.WorldPosition = position;
            _targetRenderer.Sprite = sprite;
        }
    }
}
