using Foundation;
using UIKit;

namespace iOS;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override bool FinishedLaunching (UIApplication application, NSDictionary? launchOptions)
	{
		// Override point for customization after application launch.
		return true;
	}

	public override UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
	{
		// Called when a new scene session is being created.
		// Use this method to select a configuration to create the new scene with.
		// "Default Configuration" is defined in the Info.plist's 'UISceneConfigurationName' key.
		return new UISceneConfiguration ("Default Configuration", connectingSceneSession.Role);
	}

	public override void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions)
	{
		// Called when the user discards a scene session.
		// If any sessions were discarded while the application was not running, this will be called shortly after 'FinishedLaunching'.
		// Use this method to release any resources that were specific to the discarded scenes, as they will not return.
	}
	
	private nint _bgTask = UIApplication.BackgroundTaskInvalid;

	public override void DidEnterBackground(UIApplication application)
	{
		_bgTask = application.BeginBackgroundTask("MyTask", () =>
		{
			// Expiration handler iOS is about to kill this app, clean up
			application.EndBackgroundTask(_bgTask);
			_bgTask = UIApplication.BackgroundTaskInvalid;
		});
		
		// Do your work here before time runs out (<=30 sec)
	}

	public override void WillEnterForeground(UIApplication application)
	{
		if (_bgTask != UIApplication.BackgroundTaskInvalid)
		{
			application.EndBackgroundTask(_bgTask);
			_bgTask = UIApplication.BackgroundTaskInvalid;
		}
	}
}
