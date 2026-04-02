using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class ShaderAssetProcessorOld : TextAssetProcessor
    {
        private readonly StringBuilder _sb = new();

        public override AssetProccesResult Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var result = base.Process(reader, meta, platform);
            var str = Encoding.UTF8.GetString(result.Data);

            _sb.Clear();

            if (str.Contains("#version"))
            {
                var lines = str.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("#version"))
                    {
                        if (platform == CookingPlatform.Windows)
                        {
                            lines[i] = "#version 330 core";
                            _sb.AppendLine(lines[i]);
                        }
                        else if (platform == CookingPlatform.Android || platform == CookingPlatform.IOS)
                        {
                            lines[i] = "#version 300 es";
                            _sb.AppendLine(lines[i]);
                            _sb.AppendLine("precision mediump float;");
                        }
                        else
                        {
                            lines[i] = "#version 330 core";
                            _sb.AppendLine(lines[i]);
                        }
                    }
                    else
                    {
                        _sb.AppendLine(lines[i]);
                    }
                }
            }
            else
            {
                Console.WriteLine("Shader doesn't contain '#version' tag");
            }

            return new AssetProccesResult()
            {
                IsSuccess = true,
                Data = Encoding.UTF8.GetBytes(_sb.ToString().TrimStart())
            };
        }
    }
}