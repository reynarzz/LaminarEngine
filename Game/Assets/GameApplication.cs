using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.Layers;
using Engine.Utils;
using GlmNet;
using System.Linq;

namespace Game
{
    public class GameApplication : ApplicationLayer
    {
        // -TODO:
        /* Fix collision exit being called when the shape is destroyed, which causes the function to have a invalid actor,
             This collisionsExit/TriggerExit should not be called with invalid actors/components*/
        // Fix: rigidbody marked as interpolate if is made parent of another that is not, after exiting, the interpolation is disabled.
        // Avoid the batch to take more texture slots that the system is supported, take into account materials texture count.
        // Fix: Batch2d vertices shift when an object is destroyed. 
        // Fix: Texture units overflow, a batch should only bind: MAX_DEVICE_ALLOWED - RENDERER_NEEDED_TEXTURE_SLOTS
        // Implement:scroll list 


        // For the game:
        // Implement enemies
        // -Enemy AI
        // Enemy life UI
        //
        // Five levels, small, one intro level falling from outside.
        // Colllect coins, hearts, attack enemies, go from door A to B
        // Implement collectibles
        // Implement UI (inventory, pause menu)
        

        // -Stretch:
        // Implement bounds in sprites/renderers.
        // Implement event in transform to know when scale changed, and get the delta scale.
        // Add 'CheckIfValidObject()' to all properties of the engine's components and actor.
        // Investigate why colliders are not freed from memory automatically.
        // Game using both assets, and using stencil buffer to change beteen them sphere.
        // Investigate why AudioMixer frees from memory automatically


        public override void Initialize()
        {
            new Actor<GameManager>("GameManager");

            //PostProcessingStack.Push(new BloomPostProcessing());
            //ScreenGrabTest();
            //ScreenGrabTest3();

            // FilmGrain();

            WaterTest();
            ParticleSystem();
            // TextRendering();
            Debug.Success("Game Layer");
        }

        private void ParticleSystem()
        {
            var particleSystem = new Actor<ParticleSystem2D, Move>("ParticleSystem").GetComponent<ParticleSystem2D>();
            particleSystem.Transform.WorldPosition = new vec3(-34, -6);

            particleSystem.EmitRate = 152;
            particleSystem.ParticleLife = 3;
            particleSystem.SortOrder = 17;
            particleSystem.StartColor = Color.White;
            particleSystem.EndColor = Color.White;// new Color(0, 0, 0, 0);
            particleSystem.EndSize = new vec2(0, 0);
            particleSystem.Spread = new vec2(0.0f, 0);
            particleSystem.SimulationSpeed = 1;
            particleSystem.StartSize = new vec2(0.3f);
            particleSystem.IsWorldSpace = true;
            particleSystem.AngularVelocity = 40;
            var mainShader = new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text);

            var mat1 = new Material(mainShader);
            mat1.Name = "Particle material";
            particleSystem.Material = mat1;

            //var screenShader = new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/ScreenGrabWobble.frag").Text);
            //particleSystem.Material = new Material(screenShader);
            //particleSystem.Material.GetPass(0).IsScreenGrabPass = true;

            //particleSystem.Material.Passes.ElementAt(0).Blending.Enabled = false;
            particleSystem.Sprite = new Sprite();
        }

        private void ScreenGrabTest()
        {
            var screenShader = new Shader(Assets.GetText("Shaders/ScreenVert.vert").Text, Assets.GetText("Shaders/CTRTv.frag").Text);
            PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));
        }

        private void ScreenGrabTest2()
        {
            var screenShader = new Shader(Assets.GetText("Shaders/ScreenVert.vert").Text, Assets.GetText("Shaders/GrayScale.frag").Text);
            PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));
        }

        private void ScreenGrabTest3()
        {
            var vertex = Assets.GetText("Shaders/ScreenVert.vert").Text;
            var screenShader = new Shader(vertex, Assets.GetText("Shaders/Ripple.frag").Text);
            PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));

            var screenShader2 = new Shader(vertex, Assets.GetText("Shaders/ChromaticAberration.frag").Text);
            PostProcessingStack.Push(new PostProcessingSinglePass(screenShader2));

        }

        private void FilmGrain()
        {
            var screenShader = new Shader(Assets.GetText("Shaders/ScreenVert.vert").Text, Assets.GetText("Shaders/FilmGrain.frag").Text);
            PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));
        }

        private void WaterTest()
        {
            var waterActor = new Actor<SpriteRenderer>();
            var renderer = waterActor.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 9;

            var mainShader = new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/WaterFrag.frag").Text);

            renderer.Material = new Material(mainShader);

            var pass = renderer.Material.Passes.ElementAt(0);
            pass.Stencil.Enabled = true;
            pass.Stencil.Func = StencilFunc.Equal;
            pass.Stencil.Ref = 3;
            pass.Stencil.ZFailOp = StencilOp.Keep;
            renderer.Material.SetProperty("uWaterColor", new vec3(0.2f, 0.4f, 0.7f));
            renderer.Material.AddTexture("uParticles", Assets.GetTexture("particles.png"));

            var pass2 = renderer.Material.PushPass(mainShader);
            pass2.IsScreenGrabPass = true;
            pass2.Stencil.Enabled = true;
            pass2.Stencil.Func = StencilFunc.NotEqual;
            pass2.Stencil.Ref = 3;
            pass2.Stencil.ZFailOp = StencilOp.Keep;

            renderer.Material.SetProperty(1, "uWaterColor", new vec3(0.1f, 0.50f, 0.84f));

            waterActor.Transform.LocalScale = new vec3(10, 3, 1);
            waterActor.Transform.LocalPosition = new vec3(2.5f, -11, 1);
        }

        public override void Close() { }
    }
}
