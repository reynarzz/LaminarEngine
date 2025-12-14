using GLKit;
using OpenGL;
using OpenGLES;
using static OpenGL.ES.GLES30;

namespace Engine.IOS
{

    public class GLViewController : GLKViewController
    {
        private EAGLContext _context;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create an OpenGL ES 3.0 context
            _context = new EAGLContext(EAGLRenderingAPI.OpenGLES3);

            var glkView = new GLKView(View.Frame, _context)
            {
                DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24,

            };

            View = glkView;

            EAGLContext.SetCurrentContext(_context);

            // Init GL state
            // GL.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);
            glClearColor(1.0f, 0, 0, 1.0f);
        }

        public override void Update()
        {
            // Game update logic here
        }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

            // TODO: draw your OpenGL stuff here
        }
    }
}