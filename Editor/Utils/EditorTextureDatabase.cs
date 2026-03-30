using Engine;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using Engine;
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
        Scene,
        ScriptFile,
        WindowsSmall,
        AndroidSmall,
        AppleSmall,
        LinuxSmall,
        CirclePicker,
        Texture,
        Audio,
        Animator,
        AnimationClip,
        Camera,
        Transform,
        CircleCollider,
        CapsuleCollider,
        Physics,
        Tilemap,
        Actor,
        Actor2,
        Edit,
        Close,
        Plus,
        Minus,
        FolderOpenFilled,
        FolderClosedEmpty,
        FolderOpenEmpty,
        FolderClosedFilled,
        Font,
        Shader,
        Layer,
        Layer2
    }

    internal sealed class EditorTextureDatabase
    {
        private static EditorTextureDatabase _instance;
        private Dictionary<EditorIcon, Texture2D> _icons;
        private Dictionary<Type, EditorIcon> _typesToIconTypeMapper;
        private Dictionary<AssetType, EditorIcon> _assetTypeToEditorIcon;


        private EditorTextureDatabase()
        {

            _icons = new Dictionary<EditorIcon, Texture2D>()
            {
                { EditorIcon.Play, LoadIconFromDisk("play.png") },
                { EditorIcon.Pause, LoadIconFromDisk("pause.png") },
                { EditorIcon.Stop, LoadIconFromDisk("stop.png") },
                { EditorIcon.Skip, LoadIconFromDisk("skip_hq.png") },
                { EditorIcon.Material, LoadIconFromDisk("material24x24_2.png") },
                { EditorIcon.Text, LoadIconFromDisk("text_icon.png") },
                { EditorIcon.Scene, LoadIconFromDisk("scene.png") },
                { EditorIcon.ScriptFile, LoadIconFromDisk("csharp.png") },
                { EditorIcon.WindowsSmall, LoadIconFromDisk("windows32x32.png") },
                { EditorIcon.AndroidSmall, LoadIconFromDisk("android32x32.png") },
                { EditorIcon.AppleSmall, LoadIconFromDisk("apple32x32.png") },
                { EditorIcon.LinuxSmall, LoadIconFromDisk("linux32x32.png") },
                { EditorIcon.CirclePicker, LoadIconFromDisk("circlepicker24x24.png") },
                { EditorIcon.Texture, LoadIconFromDisk("sprite26x24.png") },
                { EditorIcon.Audio, LoadIconFromDisk("audio.png") },
                { EditorIcon.Animator, LoadIconFromDisk("skeleton24x24_2.png") },
                { EditorIcon.AnimationClip, LoadIconFromDisk("skeleton24x24_1.png") },
                { EditorIcon.Camera, LoadIconFromDisk("camera.png") },
                { EditorIcon.Transform, LoadIconFromDisk("xyz gizmo.png") },
                { EditorIcon.CircleCollider, LoadIconFromDisk("circlecollider24x24.png") },
                { EditorIcon.CapsuleCollider, LoadIconFromDisk("capsule24x24.png") },
                { EditorIcon.Physics, LoadIconFromDisk("physics24x24.png") },
                { EditorIcon.Tilemap, LoadIconFromDisk("tilemap50x50.png") },
                { EditorIcon.Actor, LoadIconFromDisk("cube32x32.png") },
                { EditorIcon.Actor2, LoadIconFromDisk("cube_empty32x32.png") },
                { EditorIcon.Edit, LoadIconFromDisk("edit.png") },
                { EditorIcon.Close, LoadIconFromDisk("close22x22.png") },
                { EditorIcon.Plus, LoadIconFromDisk("plus24x24.png") },
                { EditorIcon.Minus, LoadIconFromDisk("minus30x30.png") },
                { EditorIcon.FolderClosedEmpty, LoadIconFromDisk("folder_empty24x24.png") },
                { EditorIcon.FolderClosedFilled, LoadIconFromDisk("folder_filled.png") },
                { EditorIcon.FolderOpenFilled, LoadIconFromDisk("folder_open_filled.png") },
                { EditorIcon.FolderOpenEmpty, LoadIconFromDisk("folder_filled.png") },
                { EditorIcon.Font, LoadIconFromDisk("font30x30.png") },
                { EditorIcon.Shader, LoadIconFromDisk("shader50x50.png") },
                { EditorIcon.Layer, LoadIconFromDisk("layer24x24.png") },
                { EditorIcon.Layer2, LoadIconFromDisk("layer2_64x64.png") },
            };

            _typesToIconTypeMapper = new Dictionary<Type, EditorIcon>()
            {
                { typeof(Material), EditorIcon.Material },
                { typeof(Camera), EditorIcon.Camera },
                { typeof(TextAsset), EditorIcon.Text },
                { typeof(Texture), EditorIcon.Texture },
                { typeof(Texture2D), EditorIcon.Texture },
                { typeof(Sprite), EditorIcon.Texture },
                { typeof(SceneAsset), EditorIcon.Scene },
                { typeof(AudioClip), EditorIcon.Audio },
                { typeof(AudioSource), EditorIcon.Audio },
                { typeof(AnimatorController), EditorIcon.Animator },
                { typeof(Animator), EditorIcon.Animator },
                { typeof(AnimationClip), EditorIcon.AnimationClip },
                { typeof(Transform), EditorIcon.Transform},
                { typeof(CircleCollider2D), EditorIcon.CircleCollider },
                { typeof(CapsuleCollider2D), EditorIcon.CapsuleCollider },
                { typeof(RigidBody2D), EditorIcon.Physics },
                { typeof(TilemapAsset), EditorIcon.Tilemap },
                { typeof(Actor), EditorIcon.Actor2 },
                { typeof(FontAsset), EditorIcon.Font },
                { typeof(Shader), EditorIcon.Shader },
            };

            _assetTypeToEditorIcon = new()
            {
                { AssetType.Material, EditorIcon.Material },
                { AssetType.Texture, EditorIcon.Texture },
                { AssetType.Scene, EditorIcon.Scene },
                { AssetType.Audio, EditorIcon.Audio },
                { AssetType.Tilemap, EditorIcon.Tilemap },
                { AssetType.Text, EditorIcon.Text },
                { AssetType.AnimationClip, EditorIcon.AnimationClip },
                { AssetType.AnimatorController, EditorIcon.Animator },
                { AssetType.Font, EditorIcon.Font },
                { AssetType.Shader, EditorIcon.Shader },
                { AssetType.ShaderV2, EditorIcon.Shader },
                { AssetType.Script, EditorIcon.ScriptFile },
            };
        }

        internal void GetIcon(AssetType type)
        {

        }

        private Texture2D LoadIconFromDisk(string path)
        {
            var finalPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.EditorDataRoot, "Resources/Icons", path));
            if (File.Exists(finalPath))
            {
                StbImage.stbi_set_flip_vertically_on_load(1);
                using var file = File.Open(finalPath, FileMode.Open, FileAccess.Read);

                var image = ImageResult.FromStream(file);

                return new Texture2D(TextureWrapMode.Clamp, TextureFilterMode.Linear, image.Width, image.Height, 4, image.Data);
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
            if (targetType == null)
                return null;

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

        public static nint GetIconImGui(AssetType assetType)
        {
            if (_instance._assetTypeToEditorIcon.TryGetValue(assetType, out var iconType))
            {
                var icon = GetIcon(iconType);

                if (icon != null)
                {
                    return GetIconImGui(icon);
                }
            }

            return 0;
        }

        public static nint GetIconImGui(Type targetType)
        {
            var tex = GetIcon(targetType);
            if (tex != null)
            {
                return GetIconImGui(tex);
            }

            return 0;

        }
        internal static nint GetIconImGui(RenderTexture texture)
        {
            if (texture == null)
                return 0;
            return (nint)(texture.NativeResource as GLFrameBuffer).ColorTexture.Handle;
        }
        internal static nint GetIconImGui(Texture texture)
        {
            if (texture != null)
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
