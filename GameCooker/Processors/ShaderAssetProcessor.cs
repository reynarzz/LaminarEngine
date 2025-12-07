using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    internal class ShaderAssetProcessor : IAssetProcessor
    {
        private readonly StringBuilder _sb = new();

        byte[] IAssetProcessor.Process(string path, AssetMetaFileBase meta)
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
#if WINDOWS
                        lines[i] = "#version 330 core";
#else
                        lines[i] = "#version 410";
#endif
                    }

                    _sb.AppendLine(lines[i]);
                }
            }
            else
            {
                Console.WriteLine("Shader doesn't contain '#version' tag");
            }

            return Encoding.UTF8.GetBytes(_sb.ToString());
        }
    }
}