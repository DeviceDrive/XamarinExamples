using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DeviceDrive.SDK;

namespace LightSwitch
{
	public partial class MenuPage : ContentPage
	{
		public const string SignOuMessage = "SignOuMessage";
		public const string DeleteActiveDeviceMessage = "DeleteActiveDeviceMessage";
		public const string LinkUpActiveDeviceMessage = "LinkUpActiveDeviceMessage";

		public MenuPage()
		{
			Title = "Menu";
			Device.OnPlatform(() => Icon = "MenuButton");
			InitializeComponent();
			BindingContext = this;
		}

		#region Properties

		public ImageSource LigthSwitchLogo
		{
			get { return ImageSource.FromResource(LightSwitch.Resources.ResourcePath.Path + ".LightSwitch.png"); }
		}

		#endregion

		#region Commands

		public Command EditProfileCommand
		{
			get
			{
				return new Command(async (obj) => {

					HideMenuPage();
					
					try
					{
						await DeviceDriveManager.Current.Authentication.EditProfileAsync();
					}
					catch (Exception)
					{
						// Handle cancellation
					}
				});
			}
		}

		public Command SignOutCommand
		{
			get
			{
				return new Command(async (obj) => {

					HideMenuPage();

					try
					{
						await DeviceDriveManager.Current.Authentication.LogoutAsync();
					}
					catch (Exception)
					{
						// Handle cancellation
						return;
					}

					MessagingCenter.Send(this, SignOuMessage);
				});
			}
		}

		public Command DeleteActiveDeviceCommand
		{
			get
			{
				return new Command(() =>
				{
					HideMenuPage();
					MessagingCenter.Send(this, DeleteActiveDeviceMessage);
				});
			}
		}

		public Command LinkUpActiveDeviceCommand
		{
			get
			{
				return new Command(() =>
				{
					HideMenuPage();
					MessagingCenter.Send(this, LinkUpActiveDeviceMessage);
				});
			}
		}
		#endregion

		#region Private Members

		private void HideMenuPage()
		{
			var p = (Application.Current.MainPage as MasterDetailPage);
			if (p != null)
				p.IsPresented = false;
		}
		#endregion
	}
}
