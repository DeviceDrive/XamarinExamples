using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DeviceDrive.SDK;
using DeviceDrive.SDK.Droid;

namespace LightSwitch.Droid
{
	[Activity(Label = "LightSwitch", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, 
	          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			DeviceDriveManager.Current.Initialize(
				// TODO: Add AppID and Application Secret here:
				"", "",
				new DeviceDriveDroidPlatform(this));

			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			LoadApplication(new App());
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			DeviceDriveDroidPlatform.HandleOnActivityResult(requestCode, resultCode, data);
		}
	}
}
