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
            // TextRendering();
            Debug.Success("Game Layer");
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

        public override void Close() { }
    }
}
