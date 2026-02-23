using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TextAsset : AssetResourceBase
    {
        public string Text { get; private set; }

        public TextAsset(string text, string path, Guid guid) : base(path, guid)
        {
            Text = text;
        }

        internal override void UpdateResource(object data, string path, Guid guid)
        {
            Text = data as string;
        }
    }
}
