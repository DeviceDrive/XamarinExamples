using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DeviceDrive.SDK;
using System.Threading.Tasks;
using Autofac;
using DeviceDrive.SDK.Contracts;
using DeviceDrive.SDK.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LightSwitch
{
	public partial class App : Application
	{
		public const string AppOnSleepMessage = "AppOnSleepMessage";
		public const string AppOnResumeMessage = "AppOnResumeMessage";

        private static IContainer _container;

        public App(IDeviceDrivePlatform platform)
		{
			InitializeComponent();

            var builder = new ContainerBuilder();
            builder.RegisterType<TermsService>().As<ITermsService>().SingleInstance();
            _container = builder.Build();

            // The root page of your application
            MainPage = new MasterDetailPage
			{
				Master = new MenuPage(),
				Detail = new NavigationPage(new MainPage(platform, _container)),
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
