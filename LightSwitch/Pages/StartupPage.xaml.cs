using System;
using System.Collections.Generic;
using DeviceDrive.SDK;
using Xamarin.Forms;

namespace LightSwitch
{	
	public partial class StartupPage : ContentPage
	{
		#region Private Members

		public const string AuthenticatedMessage = "AuthenticatedMessage";

		#endregion

		public StartupPage()
		{
			InitializeComponent();
			BindingContext = this;
			IsInAuthentication = true;
		}

		#region Overrides

		protected override bool OnBackButtonPressed()
		{
			if (StartupPageNumber > 0)
				StartupPageNumber--;

			return true;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The StartupPageNumber property.
		/// </summary>
		public static BindableProperty StartupPageNumberProperty =
			BindableProperty.Create(nameof(StartupPageNumber), typeof(int), typeof(StartupPage), 0,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (StartupPage)bindable;
					ctrl.StartupPageNumber = (int)newValue;
				});

		/// <summary>
		/// Gets or sets the PropertyName of the int instance.
		/// </summary>
		public int StartupPageNumber
		{
			get { return (int)GetValue(StartupPageNumberProperty); }
			set
			{
				SetValue(StartupPageNumberProperty, value);

				if(value == 1)
					IsInAuthentication = false;
			}

		}

		public ImageSource LigthSwitchLogo
		{
			get { return ImageSource.FromResource(LightSwitch.Resources.ResourcePath.Path + ".LightSwitch.png"); }
		}

		/// <summary>
		/// The IsInAuthentication property.
		/// </summary>
		public static BindableProperty IsInAuthenticationProperty =
			BindableProperty.Create(nameof(IsInAuthentication), typeof(bool), typeof(StartupPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (StartupPage)bindable;
					ctrl.IsInAuthentication = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsInAuthentication of the StartupPage instance.
		/// </summary>
		public bool IsInAuthentication
		{
			get { return (bool)GetValue(IsInAuthenticationProperty); }
			set
			{
				SetValue(IsInAuthenticationProperty, value);
			}
		}

		#endregion

		#region Commands

		public Command NextCommand
		{
			get
			{
				return new Command((obj) =>
				{
					StartupPageNumber++;
				});
			}
		}

		public Command DoneCommand
		{
			get
			{
				return new Command(async (obj) =>
				{
					DeviceDriveManager.Current.Preferences.WelcomeScreenDisplayed = true;
					await Navigation.PopModalAsync(true);

					MessagingCenter.Send(this, AuthenticatedMessage);
				});
			}
		}

		public Command SignInCommand
		{
			get
			{
				return new Command(async () =>
				{

					try
					{
						IsInAuthentication = true;

						var retVal = await DeviceDriveManager.Current.Authentication.AuthenticateAsync(
							AuthenticationPolicy.SignIn);

						if (retVal != null)
							StartupPageNumber++;
						else
							IsInAuthentication = false;
					}
					catch (Exception ex) 
					{
						IsInAuthentication = false;
						await DisplayAlert(Title, ex.Message, "OK");
					}
				});
			}
		}

		public Command SignUpCommand
		{
			get
			{
				return new Command(async () =>
				{
					try
					{
						IsInAuthentication = true;
							
						var retVal = await DeviceDriveManager.Current.Authentication.AuthenticateAsync(
							AuthenticationPolicy.Signup);

						if (retVal != null)
							StartupPageNumber++;
						else
							IsInAuthentication = false;
					}
					catch (Exception ex)
					{
						IsInAuthentication = false;
						await DisplayAlert(Title, ex.Message, "OK");
					}

				});
			}
		}

		#endregion
	}
}
