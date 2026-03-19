using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TextAsset : Asset
    {
        public string Text { get; private set; }

        public TextAsset(string text, Guid guid) : base(guid)
        {
            Text = text;
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {
            Text = data as string;
        }
    }
}
