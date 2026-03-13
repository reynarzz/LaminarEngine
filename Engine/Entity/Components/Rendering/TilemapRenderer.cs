using Engine.Graphics;
using Engine.Layers;
using Engine.Types;
using GlmNet;
using Engine;
using System.Runtime.InteropServices;

namespace Engine
{
    public struct Tile
    {
        public int Index { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public Tile(int index, bool flipX, bool flipY)
        {
            Index = index;
            FlipX = flipX;
            FlipY = flipY;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TilemapVertex : IVertex2D<TilemapVertex>
    {
        public vec2 Position;
        public vec2 UV;
        public uint Color;
        private int _textureIndex;

        public int TextureIndex { get => _textureIndex; set => _textureIndex = value; }
        private unsafe static VertexAtrib[] _attrib =
        [
            new() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(TilemapVertex), Offset = 0 },
            new() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(TilemapVertex), Offset = sizeof(float) * 2 },
            new() { Count = 1, Normalized = false, Type = GfxValueType.Uint, Stride = sizeof(TilemapVertex), Offset = sizeof(uint) * 4 },
            new() { Count = 1, Normalized = false, Type = GfxValueType.Int, Stride = sizeof(TilemapVertex), Offset = sizeof(int) * 5 },
        ];

        static VertexAtrib[] IVertex<TilemapVertex>.GetVertexAttributes()
        {
            return _attrib;
        }
    }

    [UniqueComponent]
    public class TilemapRenderer : Renderer2D
    {
        [SerializedField]
        public override Color Color { get => base.Color; set => base.Color = value; }

        [SerializedField]
        public override int SortOrder { get => base.SortOrder; set => base.SortOrder = value; }

        private TilemapAsset _tilemapAsset;
        [SerializedField]
        public TilemapAsset Tilemap
        {
            get => _tilemapAsset;
            private set
            {
                if (_tilemapAsset == value)
                    return;

                _tilemapAsset = value;
                _rendererData.IsDirty = true;
            }
        }


        private TilemapRenderingOptions _renderingOptions;
        [SerializedField]
        public TilemapRenderingOptions Options
        {
            get => _renderingOptions;
            private set
            {
                if (_renderingOptions.LevelIndex == value.LevelIndex && _renderingOptions.LayerIndex == value.LayerIndex)
                    return;

                _renderingOptions = value;
                _rendererData.IsDirty = true;

            }
        }
        private RendererData2D _rendererData;
        private Mesh<TilemapVertex> _mesh = new();

        internal override void OnInternalInitialize()
        {
            base.OnInternalInitialize();
            _rendererData = RendererData as RendererData2D;
            _rendererData.Mesh = _mesh;
            _rendererData.Mesh.IndicesToDrawCount = 0;
            _rendererData.Bounds = Bounds.GetInitialized();
            _rendererData.VertexType = typeof(TilemapVertex);

            RenderingLayer.PushRenderer(this);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            RenderingLayer.PushRenderer(this);
        }

        internal override void Draw()
        {
            base.Draw();
            if (_rendererData.IsDirty)
            {
                var layer = GetLayer();

                if (layer != null)
                {
                    layer.Vertices = layer.Vertices ?? Array.Empty<TilemapVertex>();

                    _mesh.Vertices = layer.Vertices;
                    _mesh.IndicesToDrawCount = layer.Vertices.Length == 0 ? 0 : layer.IndicesToDraw;
                    _rendererData.Bounds = layer.Bounds;
                }
                else
                {
                    _mesh.Vertices = Array.Empty<TilemapVertex>();
                    _mesh.IndicesToDrawCount = 0;
                    _rendererData.Bounds = default;
                }
            }
        }
        public void SetTilemap(TilemapAsset tilemap, TilemapRenderingOptions options)
        {
            _tilemapAsset = tilemap;
            _renderingOptions = options;
            _rendererData.IsDirty = true;
        }

        internal TilemapLevelLayer GetLayer()
        {
            var data = _tilemapAsset?.GetData();

            if (data == null)
                return null;

            _renderingOptions.LevelIndex = Mathf.Clamp(_renderingOptions.LevelIndex, 0, data.Levels.Count - 1);

            var levelIndex = data.Levels.Count > _renderingOptions.LevelIndex ? _renderingOptions.LevelIndex : data.Levels.Count - 1;

            if (levelIndex < 0)
                return null;

            var level = data.Levels.FirstOrDefault(x => x.Value.LevelIndex == levelIndex).Value;

            _renderingOptions.LayerIndex = Mathf.Clamp(_renderingOptions.LayerIndex, 0, level.Layers.Count - 1);

            var layerIndex = level.Layers.Count > _renderingOptions.LayerIndex ? _renderingOptions.LayerIndex : level.Layers.Count - 1;

            if (layerIndex < 0)
                return null;

            return level.Layers.FirstOrDefault(x => x.Value.LayerIndex == layerIndex).Value;
        }
    }

    public struct TilemapRenderingOptions
    {
        [SerializedField] public int LevelIndex;
        [SerializedField] public int LayerIndex;
    }
}
