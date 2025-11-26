using Engine;
using Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class TextWritterTest : ScriptBehavior
    {
        public string Text { get; set; }
        public float DelayToWrite { get; set; } = 0.1f;
        private float _currentTime;
        private int _characterIndex = 0;

        private UIText _textRenderer;

        public override void OnStart()
        {
            _textRenderer = GetComponent<UIText>();
        }

        protected override void OnUpdate()
        {
            if((_currentTime -= Time.DeltaTime) <= 0)
            {
                _currentTime = DelayToWrite;
                if ((_textRenderer.Length != _characterIndex || _textRenderer.Length != Text.Length) && Text.Length > _characterIndex)
                {
                    //_textRenderer.Text[_characterIndex] = Text[_characterIndex];
                    _textRenderer.Append(Text[_characterIndex]);
                    _characterIndex++;
                }
            }
        }
    }
}
