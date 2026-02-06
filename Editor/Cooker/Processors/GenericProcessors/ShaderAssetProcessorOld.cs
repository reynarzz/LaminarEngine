using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class ShaderAssetProcessorOld : IAssetProcessor
    {
        private readonly StringBuilder _sb = new();

        AssetProccesResult IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var str = reader.ReadToEnd();

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
                        else if(platform == CookingPlatform.Android)
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