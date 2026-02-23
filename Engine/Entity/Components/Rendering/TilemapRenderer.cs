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

        [SerializedField] public TilemapRenderingOptions Options { get; set; }

        [SerializedField] public TilemapAsset Tilemap { get; private set; }

        private RendererData2D _rendererData;

        internal override void OnInternalInitialize()
        {
            base.OnInternalInitialize();
            _rendererData = (RendererData as RendererData2D);
            _rendererData.Mesh = new Mesh();
            _rendererData.Mesh.IndicesToDrawCount = 0;
            _rendererData.PrivateBatch = true;
            _rendererData.Bounds = new Bounds()
            {
                Min = vec3.One * int.MaxValue,
                Max = vec3.One * int.MinValue
            };

            RenderingLayer.PushRenderer(this);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            RenderingLayer.PushRenderer(this);
        }

        public void SetTilemap(TilemapAsset tilemap, TilemapRenderingOptions options)
        {
            Tilemap = tilemap;
            Options = options;

            var data = tilemap.GetData();

            var layer = GetLayer();
            // TODO: Mesh should use arrays instead.
            _rendererData.Mesh.Vertices = layer.Vertices.ToList();
            _rendererData.Mesh.IndicesToDrawCount = layer.IndicesToDraw;
        }

        internal TilemapLevelLayer GetLayer()
        {
            // TODO: check for null

            var level = Tilemap.GetData().Levels.FirstOrDefault(x => x.Value.LevelIndex == Options.LevelIndex).Value;
            return level.Layers.FirstOrDefault(x => x.Value.LayerIndex == Options.LayerIndex).Value;
        }
    }

    public struct TilemapRenderingOptions
    {
        [SerializedField] public bool RenderIntGridLayer { get; set; }
        [SerializedField] public bool RenderTilesLayer { get; set; }
        [SerializedField] public bool RenderAutoLayer { get; set; }
        [SerializedField] public int LevelIndex { get; set; }
        [SerializedField] public int LayerIndex { get; set; }
        [SerializedField] public int WorldDepth { get; set; }
    }
}
