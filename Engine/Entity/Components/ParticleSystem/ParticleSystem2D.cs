using Engine.Graphics;
using Engine.Layers;
using GlmNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ParticleSystem2D : Renderer2D, IUpdatableComponent, IStartableComponent
    {
        private static mat4 _particlePositionM = mat4.identity();

        public float EmitRate { get; set; } = 30f;
        public float ParticleLife { get; set; } = 1.5f;
        public vec2 VelocityMin { get; set; } = new(-1, -1);
        public vec2 VelocityMax { get; set; } = new(1, -1);
        public vec2 Spread { get; set; } = new(2.5f, 0.5f);
        public Color StartColor { get; set; } = Color.White;
        public Color EndColor { get; set; } = Color.Transparent;
        public vec2 StartSize { get; set; } = vec2.One;
        public vec2 EndSize { get; set; } = vec2.One;
        public float AngularVelocity { get; set; } = 0;

        public bool IsWorldSpace { get; set; }
        public float SimulationSpeed { get; set; } = 1;

        private List<Particle> _particles = new();
        private float _emitAccumulator = 0f;

        public vec2 Gravity { get; set; }
        public bool Prewarm { get; set; }
        private bool _canEmit = true;
        private bool _emitEventSent;
        private RendererData2D _rendererData;

        public event Action OnEmitFinished;
        private List<Vertex> _vertices;
        protected override void OnAwake()
        {
            base.OnAwake();

            _rendererData = (RendererData as RendererData2D);
            _rendererData.Mesh = new Mesh();


            _rendererData.Mesh = new Mesh();
            _rendererData.Mesh.IndicesToDrawCount = 0;
            const float bufferOffset = 1.2f;

            _particles.Capacity = (int)MathF.Ceiling(EmitRate * ParticleLife * bufferOffset);

            _vertices = new List<Vertex>(_particles.Capacity * 4);
            _rendererData.Mesh.Vertices = _vertices;
            Sprite = new Sprite(0, Texture2D.White); // Remove this, reference default sprite
            _rendererData.PrivateBatch = true; // TODO: Remove this
        }

        internal override void OnInternalInitialize()
        {
            base.OnInternalInitialize();

            RenderingLayer.PushRenderer(this);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            RenderingLayer.PushRenderer(this);
        }


        void IStartableComponent.OnStart()
        {
            if (Prewarm)
            {
                // TODO: Pre warm here.
            }
        }
        void IUpdatableComponent.OnUpdate()
        {
            var dt = Time.DeltaTime * SimulationSpeed;

            for (int i = _particles.Count - 1; i >= 0; --i)
            {
                var particle = _particles[i];
                particle.Life -= dt;

                if (particle.Life <= 0)
                {
                    _particles.RemoveAt(i);
                    continue;
                }

                float time = 1.0f - (particle.Life / particle.StartLife);

                particle.Velocity += Gravity * dt;
                particle.Position += particle.Velocity * dt;
                particle.Rotation += particle.AngularVelocity * dt;
                particle.Color = Color.Lerp(StartColor, EndColor, time);
                particle.Size = Mathf.Lerp(StartSize, EndSize, time);

                _particles[i] = particle;
            }

            _emitAccumulator += dt * EmitRate;

            while (_emitAccumulator >= 1.0f)
            {
                EmitParticle();
                _emitAccumulator -= 1.0f;
            }

            if(_particles.Count == 0 && _canEmit == false && !_emitEventSent)
            {
                _emitEventSent = true;
                OnEmitFinished?.Invoke();
            }
        }

        private void EmitParticle()
        {
            RendererData.IsDirty = true;

            if (!_canEmit)
                return;
            var localPos = new vec4(RandomFloat(-Spread.x, Spread.x), RandomFloat(-Spread.y, Spread.y), 0, 1);
            var startPos = IsWorldSpace ? Transform.WorldMatrix * localPos : localPos;

            var particle = new Particle()
            {
                Color = StartColor,
                StartLife = ParticleLife,
                Life = ParticleLife,
                Position = new vec2(startPos.x, startPos.y),
                Rotation = 0,
                Velocity = new vec2(RandomFloat(VelocityMin.x, VelocityMax.x), RandomFloat(VelocityMin.y, VelocityMax.y)),
                AngularVelocity = RandomFloat(-AngularVelocity, AngularVelocity),
                Size = StartSize,
                IsWorldSpace = IsWorldSpace
            };

            _particles.Add(particle);
        }
        public void Pause()
        {
            _canEmit = false;
        }

        public void Play()
        {
            _canEmit = true;
            _emitEventSent = false;
        }
        private float RandomFloat(float min, float max)
        {
            return (float)(Random.Shared.NextDouble() * (max - min) + min);
        }

        internal override void Draw()
        {
            var texture = Sprite.Texture;
            var chunk = Sprite.GetAtlasCell();
            float ppu = texture.PixelPerUnit;


            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                _particlePositionM[3] = new vec4(particle.Position, Transform.WorldPosition.z, 1);

                var particleM = _particlePositionM *
                                glm.rotate(glm.radians(particle.Rotation), new vec3(0, 0, 1));

                var particleModel = particle.IsWorldSpace ? particleM : Transform.WorldMatrix * particleM;

                var size = particle.Size;

                QuadVertices quad = default;
                GraphicsHelper.CreateQuad(ref quad, chunk.Uvs, size.x, size.y, chunk.Pivot, particle.Color, particleModel);

                int baseIndex = i * 4;

                // Resize if needed
                while (_rendererData.Mesh.Vertices.Count < baseIndex + 4)
                {
                    _rendererData.Mesh.Vertices.Add(default);
                }

                // Update vertex data
                _rendererData.Mesh.Vertices[baseIndex + 0] = quad.v0;
                _rendererData.Mesh.Vertices[baseIndex + 1] = quad.v1;
                _rendererData.Mesh.Vertices[baseIndex + 2] = quad.v2;
                _rendererData.Mesh.Vertices[baseIndex + 3] = quad.v3;
            }

            _rendererData.Mesh.IndicesToDrawCount = _particles.Count * 6;
        }

        protected internal override void OnDestroy()
        {
            base.OnDestroy();

            OnEmitFinished = null;
        }
    }
}