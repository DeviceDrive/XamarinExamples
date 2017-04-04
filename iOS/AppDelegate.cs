using System;
using System.Collections.Generic;
using System.Linq;
using DeviceDrive.SDK;
using DeviceDrive.SDK.Touch;
using Foundation;
using UIKit;

namespace LightSwitch.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App(new DeviceDriveTouchPlatform()));

			return base.FinishedLaunching(app, options);
		}
	}
}
