using Engine;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal enum EditorIcon
    {
        Play,
        Stop,
        Pause,
        Skip
    }

    internal sealed class EditorTextureDatabase
    {
        private static EditorTextureDatabase _instance;
        private Dictionary<EditorIcon, Texture2D> _icons;
        private EditorTextureDatabase()
        {
            _icons = new Dictionary<EditorIcon, Texture2D>()
            {
                { EditorIcon.Play, LoadIconFromDisk("play.png") },
                { EditorIcon.Pause, LoadIconFromDisk("pause.png") },
                { EditorIcon.Stop, LoadIconFromDisk("stop.png") },
                { EditorIcon.Skip, LoadIconFromDisk("skip_hq.png") },
            };
        }

        private Texture2D LoadIconFromDisk(string path)
        {
            var finalPath = Path.Combine(EditorPaths.DataRoot, "Resources/Icons", path);
            if (File.Exists(finalPath))
            {
                var image = StbImageSharp.ImageResult.FromStream(File.Open(finalPath, FileMode.Open));

                return new Texture2D(TextureMode.Clamp, TextureFilter.Linear, image.Width, image.Height, 4, image.Data);
            }

            Debug.Error($"Editor icon doesn't exists at path: {finalPath}");
            return null;
        }

        public static Texture2D GetIcon(EditorIcon iconType)
        {
            if (_instance == null)
            {
                _instance = new EditorTextureDatabase();
            }

            return _instance._icons[iconType];
        }

        public static nint GetIconImGui(EditorIcon iconType)
        {
            var icon = GetIcon(iconType);

            if(icon != null)
            {
                return (nint)(icon.NativeResource as GLTexture).Handle;
            }

            return 0;
        }
    }
}
