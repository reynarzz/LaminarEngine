using Engine;
using Game;

namespace Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);


            // new GFSEngine().Initialize<GameApplication>("GFS | By Reynardo Perez", 1024, 576).Run();
        }
    }
}