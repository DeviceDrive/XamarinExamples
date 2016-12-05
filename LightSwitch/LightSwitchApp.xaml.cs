using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DeviceDrive.SDK;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LightSwitch
{
	public partial class App : Application
	{
		public const string AppOnSleepMessage = "AppOnSleepMessage";
		public const string AppOnResumeMessage = "AppOnResumeMessage";

		public App()
		{
			InitializeComponent();

			// The root page of your application
			MainPage = new MasterDetailPage
			{
				Master = new MenuPage(),
				Detail = new NavigationPage(new MainPage()),
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
			MessagingCenter.Send(this, AppOnSleepMessage);
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
			MessagingCenter.Send(this, AppOnResumeMessage);
		}
	}
}
