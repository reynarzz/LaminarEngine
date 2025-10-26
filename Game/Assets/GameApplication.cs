using Engine;
using Engine.Graphics;
using Engine.Layers;
using GlmNet;
using System.Linq;

namespace Game
{
    public class GameApplication : ApplicationLayer
    {
        // -TODO:
        // Implement physics: boxcast, circle cast.
        /* Fix collision exit being called when the shape is destroyed, which causes the function to have a invalid actor,
             This collisionsExit/TriggerExit should not be called with invalid actors/components*/
        // Fix: rigidbody marked as interpolate if is made parent of another that is not, after exiting, the interpolation is disabled.
        // Avoid the batch to take more texture slots that the system is supported, take into account materials texture count.
        // Fix: Batch2d vertices shift when an object is destroyed. 
        // Fix: Texture units overflow, a batch should only bind: MAX_DEVICE_ALLOWED - RENDERER_NEEDED_TEXTURE_SLOTS


        // For the game:
        // Implement enemies
        // Five levels, small, one intro level falling from outside.
        // Colllect coins, hearts, attack enemies, go from door A to B
        // Start with nothing, then grab the hammer as a powerup (modify sprites)
        // Implement collectibles
        // Enemy life UI
        // Enemy AI
        // Enemy boss.
        // Implement collider AABB
        // Color palette swap to have more enemies.

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


            // ScreenGrabTest3();

            Portal();
            Portal().Transform.LocalPosition = new vec3(33, -9.1f);
            Portal().Transform.LocalPosition = new vec3(43, -1);

            FilmGrain();

            WaterTest();
            ParticleSystem();

            Debug.Success("Game Layer");
        }

        private void ParticleSystem()
        {
            var particleSystem = new Actor<ParticleSystem2D, Move>("ParticleSystem").GetComponent<ParticleSystem2D>();
            particleSystem.Transform.WorldPosition = new vec3(0, 4);

            particleSystem.EmitRate = 152;
            particleSystem.ParticleLife = 3;
            particleSystem.SortOrder = 7;
            particleSystem.StartColor = Color.White;
            particleSystem.EndColor = new Color(0, 0, 0, 0);
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
            //particleSystem.Material.Passes.ElementAt(0).Blending.Enabled = false;
            var sprite = new Sprite();
            sprite.Texture = Texture2D.White;

            particleSystem.Sprite = sprite;
        }

        private void TextRendering()
        {
            var actor = new Actor<TextWritterTest>("Text1");
            var test = actor.GetComponent<TextWritterTest>();
            var renderer = actor.AddComponent<TextRenderer>();
            test.Text = "This is a text written line by line!\nand this, is being written just below!!!\nspecial characters: !@#$%^&*()_+ ñ";
            test.DelayToWrite = 0.03f;
            test.Transform.WorldPosition = new vec3(0, 150);
            renderer.Color = Color.White;
            test.Transform.WorldScale = new vec3(0.8f, 0.8f, 0.5f);
            //renderer.OutlineSize = 1;
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

        private Actor Portal()
        {
            var screenGrabTest = new Actor<SpriteRenderer, Rotate>();
            var renderer = screenGrabTest.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 14;

            var screenShader = new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Portal.frag").Text);
            renderer.Material = new Material(screenShader);

            var pass = renderer.Material.Passes.ElementAt(0);
            pass.IsScreenGrabPass = true;

            screenGrabTest.Transform.LocalScale = new vec3(6, 6);
            screenGrabTest.Transform.LocalPosition = new vec3(-9, -7);
            renderer.Material.AddTexture("uStarsTex", Assets.GetTexture("stars.png"));

            return screenGrabTest;
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
            renderer.Material.SetProperty("uWaterColor", new vec3(0.6f, 0.2f, 0.0f));
            renderer.Material.AddTexture("uParticles", Assets.GetTexture("particles.png"));

            var pass2 = renderer.Material.PushPass(mainShader);
            pass2.IsScreenGrabPass = true;
            pass2.Stencil.Enabled = true;
            pass2.Stencil.Func = StencilFunc.NotEqual;
            pass2.Stencil.Ref = 3;
            pass2.Stencil.ZFailOp = StencilOp.Keep;

            renderer.Material.SetProperty(1, "uWaterColor", new vec3(1.0f, 0.2f, 0.0f));

            waterActor.Transform.LocalScale = new vec3(10, 3, 1);
            waterActor.Transform.LocalPosition = new vec3(2.5f, -11, 1);
        }

        public override void Close() { }
    }
}
