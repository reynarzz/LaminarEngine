using Engine;
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
            var animClip = new AnimationClip("Position anim");

            var posCurve = new Vec2HermiteCurve();
            posCurve.AddKeyFrame(0, Transform.WorldPosition + new GlmNet.vec3());
            posCurve.AddKeyFrame(1, Transform.WorldPosition + new GlmNet.vec3(0, 1));
            posCurve.AddKeyFrame(2, Transform.WorldPosition + new GlmNet.vec3(1, 1));
            posCurve.AddKeyFrame(3, Transform.WorldPosition + new GlmNet.vec3(1, -1));
            posCurve.AutoSmoothTangents();

            var colorCurve = new ColorConstantCurve();
            colorCurve.AddKeyFrame(0, Color.Teal);
            colorCurve.AddKeyFrame(1, Color.Red);
            colorCurve.AddKeyFrame(2, Color.Brown);
            colorCurve.AddKeyFrame(3, Color.Cyan);
            //colorCurve.AutoSmoothTangents();


            animClip.AddCurve("Position", posCurve);
            animClip.AddCurve("Color", colorCurve);

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

            var sprite = _controller.GetSprite("Body");
            var posX = _controller.GetFloat("PositionX");

            var position = _controller.GetVec2("Position");
            var color = _controller.GetColor("Color");

            _targetRenderer.Color = color;
            _targetRenderer.Transform.WorldPosition = position;
        }
    }
}
