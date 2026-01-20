using Engine;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using StbImageSharp;
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
        Skip,
        Material,
        Text,
        Scene
    }

    internal sealed class EditorTextureDatabase
    {
        private static EditorTextureDatabase _instance;
        private Dictionary<EditorIcon, Texture2D> _icons;
        private Dictionary<Type, EditorIcon> _typesToIconTypeMapper;
        private EditorTextureDatabase()
        {

            _icons = new Dictionary<EditorIcon, Texture2D>()
            {
                { EditorIcon.Play, LoadIconFromDisk("play.png") },
                { EditorIcon.Pause, LoadIconFromDisk("pause.png") },
                { EditorIcon.Stop, LoadIconFromDisk("stop.png") },
                { EditorIcon.Skip, LoadIconFromDisk("skip_hq.png") },
                { EditorIcon.Material, LoadIconFromDisk("material_icon.png") },
                { EditorIcon.Text, LoadIconFromDisk("text_icon.png") },
                { EditorIcon.Scene, LoadIconFromDisk("scene.png") },
            };

            _typesToIconTypeMapper = new Dictionary<Type, EditorIcon>()
            {
                { typeof(Material), EditorIcon.Material },
                { typeof(TextAsset), EditorIcon.Text },
                { typeof(SceneAsset), EditorIcon.Scene },
            };
        }

        private Texture2D LoadIconFromDisk(string path)
        {
            var finalPath = Path.Combine(EditorPaths.DataRoot, "Resources/Icons", path);
            if (File.Exists(finalPath))
            {
                StbImage.stbi_set_flip_vertically_on_load(1);
                var image = ImageResult.FromStream(File.Open(finalPath, FileMode.Open));

                return new Texture2D(TextureMode.Clamp, TextureFilter.Linear, image.Width, image.Height, 4, image.Data);
            }

            Debug.Error($"Editor icon doesn't exists at path: {finalPath}");
            return null;
        }

        public static Texture2D GetIcon(EditorIcon iconType)
        {
            CheckInstance();
            return _instance._icons[iconType];
        }
        public static Texture2D GetIcon(Type targetType)
        {
            CheckInstance();
            if (_instance._typesToIconTypeMapper.TryGetValue(targetType, out var iconType))
            {
                return GetIcon(iconType);
            }
            return null;
        }
        public static nint GetIconImGui(EditorIcon iconType)
        {
            var icon = GetIcon(iconType);

            if (icon != null)
            {
                return GetIconImGui(icon);
            }

            return 0;
        }

        public static nint GetIconImGui(Type targetType)
        {
            return GetIconImGui(GetIcon(targetType));

        }
        internal static nint GetIconImGui(Texture2D texture)
        {
            if(texture != null)
            {
                return (nint)(texture.NativeResource as GLTexture).Handle;
            }

            return 0;
        }

        private static void CheckInstance()
        {
            if (_instance == null)
            {
                _instance = new EditorTextureDatabase();
            }
        }
    }
}
