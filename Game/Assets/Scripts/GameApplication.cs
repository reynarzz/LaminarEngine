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
        // Fix: rigidbody marked as interpolate if is made parent of another that is not, after exiting, the interpolation is disabled.
        // Avoid the batch to take more texture slots that the system is supported, take into account materials texture count.
        // Fix: Batch2d vertices shift when an object is destroyed. 
        // Fix: Texture units overflow, a batch should only bind: MAX_DEVICE_ALLOWED - RENDERER_NEEDED_TEXTURE_SLOTS
        // Fix: Can't change renderer sorting after is in a batch.
        // Implement:scroll list 
        // Bake tilemaps in binary file, the geometry should be already converted to the memory layout of the vertex array.
        // Why is creating new batches?
        // When a renderer is disabled and enabled, it will search for the first available one, it could find a huge batch reserved for another.So i have to search for the smallest, sameSort(if possible) valid one that a renderer fits in.
        // Collider: Bounciness and other properties could not be set properly if set in the awake function.

        // For the game:
        // game UI architecture
        // Level system
        // Implement enemies
        // -Enemy AI
        // Enemy life UI
      
        // Five levels, small, one intro level falling from outside.
        // Colllect coins, hearts, attack enemies, go from door A to B
        // Implement collectibles
        // Implement UI (inventory, pause menu)


        // -Stretch:
        // Implement bounds in sprites/renderers.
        // Implement event in transform to know when scale changed, and get the delta scale.
        // Add 'CheckIfValidObject()' to all properties of the engine's components and actor.
        // Investigate why colliders are not freed from memory automatically.
        // Game using both assets, and using stencil buffer to change between them sphere.
        // Investigate why AudioMixer frees from memory automatically

        public override void Initialize()
        {
            new Actor<LaunchScreen>("Launch Screen");
        }

        public override void Close() { }
    }
}
