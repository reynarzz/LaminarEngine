using Engine.Graphics;
using Engine.Layers;
using Engine.Types;
using GlmNet;
using Engine;

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
                SetTilemap(_tilemapAsset, Options);
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
                SetTilemap(Tilemap, _renderingOptions);
            }
        }
        private RendererData2D _rendererData;

        internal override void OnInternalInitialize()
        {
            base.OnInternalInitialize();
            _rendererData = RendererData as RendererData2D;
            _rendererData.Mesh = new Mesh();
            _rendererData.Mesh.IndicesToDrawCount = 0;
            _rendererData.PrivateBatch = true;
            _rendererData.Bounds = Bounds.GetInitialized();

            RenderingLayer.PushRenderer(this);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            RenderingLayer.PushRenderer(this);
        }

        public void SetTilemap(TilemapAsset tilemap, TilemapRenderingOptions options)
        {
            _tilemapAsset = tilemap;
            _renderingOptions = options;

            var layer = GetLayer();

            if (layer != null)
            {
                layer.Vertices = layer.Vertices ?? Array.Empty<Vertex>();

                _rendererData.Mesh.Vertices = layer.Vertices;
                _rendererData.Mesh.IndicesToDrawCount = layer.Vertices.Length == 0 ? 0 : layer.IndicesToDraw;
                _rendererData.Bounds = layer.Bounds;
            }
            else
            {
                _rendererData.Mesh.Vertices = Array.Empty<Vertex>();
                _rendererData.Mesh.IndicesToDrawCount = 0;
                _rendererData.Bounds = default;
            }

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
