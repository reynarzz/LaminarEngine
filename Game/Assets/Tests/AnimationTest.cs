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

        public override void OnStart()
        {
            _controller = AddComponent<Animator>();

            var walkClip = new AnimationClip("Walk", true);

            SpriteCurve curve = new SpriteCurve();
            curve.AddKeyFrame(0, new Sprite());
            
            walkClip.AddCurve("Sprite", curve);
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
        }
    }
}
