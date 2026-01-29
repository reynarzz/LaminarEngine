using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Engine;
using Engine.Android;
using Game;
using System.Text;

namespace Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, 
        ScreenOrientation = ScreenOrientation.Landscape, 
        ConfigurationChanges = ConfigChanges.Orientation 
                           | ConfigChanges.ScreenSize 
                           | ConfigChanges.KeyboardHidden 
                           | ConfigChanges.Keyboard
                           | ConfigChanges.Navigation
                           | ConfigChanges.LayoutDirection)]
    public partial class MainActivity : Activity
    {
        private GLView? _glView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                var lp = Window.Attributes;
                lp.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
                Window.Attributes = lp;
            }

            _glView = new GLView(this);

            SetContentView(_glView);
        }
        private void Force60Hz()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return;

            var display = WindowManager.DefaultDisplay;
            foreach (var mode in display.GetSupportedModes())
            {
                if (Math.Abs(mode.RefreshRate - 60f) < 0.5f)
                {
                    var attrs = Window.Attributes;
                    attrs.PreferredDisplayModeId = mode.ModeId;
                    Window.Attributes = attrs;
                    break;
                }
            }
        }
        protected override void OnPause()
        {
            base.OnPause();
            _glView?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            SetScreenFlags();

            _glView?.OnResume();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                SetScreenFlags();
            }
        }

        private void SetScreenFlags()
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