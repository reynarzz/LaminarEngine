using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Engine;
using Engine.Android;
using Game;
using System.Text;

namespace Android
{
    // dotnet publish -f net9.0-android -c Release
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private GLView? _glView;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);

            _glView = new GLView(this);

            SetContentView(_glView);
        }
      
        protected override void OnPause()
        {
            base.OnPause();
            _glView?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _glView?.OnResume();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                Window.DecorView.SystemUiFlags = SystemUiFlags.ImmersiveSticky
                                               | SystemUiFlags.HideNavigation
                                               | SystemUiFlags.Fullscreen
                                               | SystemUiFlags.LayoutHideNavigation
                                               | SystemUiFlags.LayoutFullscreen
                                               | SystemUiFlags.LayoutStable;
            }
        }
    }
}