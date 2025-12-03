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
        // Implement:scroll list 
        // Bake tilemaps quads vertices, and collision data in binary file, the geometry should be already converted to the memory layout of the vertex array.
        // GameIcon, it should be baked into the game data.

        // For the game:
        // Level transition using door and faded in fade out.
        // Implement enemies.
        // -Enemy AI
        // Enemy life UI
        // implement player's iframes after getting hit
        // Five levels, small, one intro level falling from outside.
        // Colllect coins, hearts, attack enemies, go from door A to B
        // Implement collectibles
        // Loot jumps from enemies, and chests, keys, and other key items will remain in inventory.
        // Platform should check with a box cast if the player is below.

        // Late release polish:
        // -final sound for hit should be sound like "vanish" when the character dies.
        // Finish inventory.
        // Implement UI (inventory, pause menu)

        // -Stretch:
        // Implement bounds in sprites/renderers.
        // Implement event in transform to know when scale changed, and get the delta scale.
        // Add 'CheckIfValidObject()' to all properties of the engine's components and actor.
        // Game using both assets, and using stencil buffer to change between them sphere.
        // Investigate why AudioMixer frees from memory automatically

        public override void Initialize()
        {
            new Actor<LaunchScreen>("Launch Screen");
        }

        public override void Close() { }
    }
}
