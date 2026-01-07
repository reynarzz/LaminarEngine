using Engine;
using Engine.Layers;

namespace Game
{
    public class GameApplication : ApplicationLayer
    {
        // -TODO:
        // Fix: rigidbody marked as interpolate if is made parent of another that is not, after exiting, the interpolation is disabled.
        // Avoid the batch to take more texture slots that the system is supported, take into account materials texture count.
        // Fix: Batch2d vertices shift when an object is destroyed. 
        // Fix: Texture units overflow, a batch should only bind: MAX_DEVICE_ALLOWED - RENDERER_NEEDED_TEXTURE_SLOTS
        // GameIcon, it should be baked into the game data.
        // Fix player getting stuck in wall corners due to ground raycast,
        //     :this can be solved by checking if the player is colliding with a wall, if so, then push him away the wall.
        // Refactor renderingLayer to use renderData.
        // Remove the use of glGetIntegerv, and store prev buffer attached using GLDevice
        // use perspective camera with FOV of 70, and rotate +14 and -14 degrees to where the player is facing.
        // Resizing leave dangling resources causing frame drops.

        // For the game:
        // Improve game architecture.
        // ----Enemy AI
        // Five levels, small, one intro level falling from outside.
        // Water and lava.

        // Late release polish:
        // -final sound for hit should be sound like "vanish" when the character dies.
        // Finish inventory.
        // Implement UI (inventory, pause menu)
        // Enemy life UI
        // Implement more enemies.
        // Save system: level state (enemies, chests, items), player inventory (custom file format)
        // when a interactable is locked by items show the items needed.

        // -Stretch goals:
        // Bake tilemaps quads vertices, and collision data in binary file, the geometry should be already converted to the memory layout of the vertex array.
        // Implement bounds in sprites/renderers.
        // Implement event in transform to know when scale changed, and get the delta scale.
        // Add 'CheckIfValidObject()' to all properties of the engine's components and actor.
        // Game using both assets, and using stencil buffer to change between them sphere.
        // Investigate why AudioMixer frees from memory automatically
        // Port to mac, android, ios, linux.
        // Leave rendering in the main thread, and move all the layers to another one.
        // Android: Manage opengl context loss.
        // Implement gamepad
        // Improve performance of contact dispatcher.
        // Implement instancing rendering.

        public override void Initialize()
        {
#if RELEASE
            Screen.IsFullScreen = true;
            WindowManager.Window.CursorVisible = false;
#endif
            new Actor<LaunchScreen>("Launch Screen");
        }

        public override void Close() 
        {
//#if DEBUG
//            var sb = new StringBuilder();
//            foreach (var item in Assets.LoadedPaths())
//            {
//                sb.AppendLine(item);
//            }
//            File.WriteAllText(Paths.GetShipAssetsFilePath(), sb.ToString());
//#endif
        }
    }
}
