using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DeviceDrive.SDK;

namespace LightSwitch
{
	public partial class MenuPage : ContentPage
	{
		public const string DeleteActiveDeviceMessage = "DeleteActiveDeviceMessage";
		public const string LinkUpActiveDeviceMessage = "LinkUpActiveDeviceMessage";
        public const string SmartLinkUpDeviceMessage = "SmartLinkUpDeviceMessage";
        public const string TermsConditionsMessage = "TermsConditionsMessage";

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

        public Command SmartLinkUpDeviceCommand
        {
            get
            {
                return new Command(() =>
                {
                    HideMenuPage();
                    MessagingCenter.Send(this, SmartLinkUpDeviceMessage);
                });
            }
        }

        public Command TermsConditionsCommand
        {
            get
            {
                return new Command(() =>
                {
                    HideMenuPage();
                    MessagingCenter.Send(this, TermsConditionsMessage);
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
