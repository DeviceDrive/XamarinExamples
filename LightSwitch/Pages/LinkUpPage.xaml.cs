using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceDrive.SDK;
using Xamarin.Forms;
using LinkUp.SDK.Xamarin;
using DeviceDrive.SDK.Contracts;
using System.Collections.ObjectModel;
using System.Linq;

namespace LightSwitch
{
	public partial class LinkUpPage : ContentPage
	{
		public const string LinkUpFinishedSuccessMessage = "LinkUpFinishedSuccessMessage";

		#region Private Members

		readonly ObservableCollection<string> _availableDevices = new ObservableCollection<string>();
		bool _onAppearingCalled = false;

		#endregion

		/// <summary>
		/// Create a new instance of the page for linking up a new device
		/// </summary>
		public LinkUpPage()
		{
			InitializeComponent();

			BindingContext = this;
			IsNewLinkUp = true;
		}

		/// <summary>
		/// Create a new instance of the page for re-linking up an existing device
		/// </summary>
		/// <param name="device">Device.</param>
		public LinkUpPage(DeviceModel device):this()
		{
			IsNewLinkUp = false;
			DeviceToken = device.Token;
			DeviceName = device.Name;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (_onAppearingCalled)
				return;

			_onAppearingCalled = true;

			Device.BeginInvokeOnMainThread(async () =>
			{
				if (string.IsNullOrEmpty(DeviceToken))
					ActivatePageEnterDeviceName();
				else
					await ActivatePageEnterWifiPasswordAsync();
			});
		}

		#region Properties

		/// <summary>
		/// Get/set wether this is a new linkup or re-linking up an existing device
		/// </summary>
		public bool IsNewLinkUp { get;set; }

		/// <summary>
		/// Returns the list of available devices
		/// </summary>
		/// <value>The available devices.</value>
		public ObservableCollection<string> AvailableDevices
		{
			get { return _availableDevices; }
		}

		/// <summary>
		/// The ActivePageNumber property.
		/// </summary>
		public static BindableProperty ActivePageNumberProperty =
			BindableProperty.Create(nameof(ActivePageNumber), typeof(int), typeof(LinkUpPage), 0,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.ActivePageNumber = (int)newValue;
				});

		/// <summary>
		/// Gets or sets the ActivePageNumber of the LinkUpPage instance.
		/// </summary>
		public int ActivePageNumber
		{
			get { return (int)GetValue(ActivePageNumberProperty); }
			set
			{
				SetValue(ActivePageNumberProperty, value);
			}
		}

		/// <summary>
		/// The NewDeviceName property.
		/// </summary>
		public static BindableProperty NewDeviceNameProperty =
			BindableProperty.Create(nameof(DeviceName), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.DeviceName = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the NewDeviceName of the LinkUpPage instance.
		/// </summary>
		public string DeviceName
		{
			get { return (string)GetValue(NewDeviceNameProperty); }
			set
			{
				SetValue(NewDeviceNameProperty, value);
			}
		}

		/// <summary>
		/// The WifiPassword property.
		/// </summary>
		public static BindableProperty WifiPasswordProperty =
			BindableProperty.Create(nameof(WifiPassword), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.WifiPassword = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the WifiPassword of the LinkUpPage instance.
		/// </summary>
		public string WifiPassword
		{
			get { return (string)GetValue(WifiPasswordProperty); }
			set
			{
				SetValue(WifiPasswordProperty, value);
			}
		}

		/// <summary>
		/// The WifiNetworkName property.
		/// </summary>
		public static BindableProperty WifiNetworkNameProperty =
			BindableProperty.Create(nameof(WifiNetworkName), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.WifiNetworkName = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the WifiNetworkName of the LinkUpPage instance.
		/// </summary>
		public string WifiNetworkName
		{ 
			get { return (string)GetValue(WifiNetworkNameProperty); }
			set
			{
				SetValue(WifiNetworkNameProperty, value);
			}
		}

		/// <summary>
		/// The SelectedDeviceSSID property.
		/// </summary>
		public static BindableProperty SelectedDeviceSSIDProperty =
			BindableProperty.Create(nameof(SelectedDeviceSSID), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.SelectedDeviceSSID = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the SelectedDeviceSSID of the LinkUpPage instance.
		/// </summary>
		public string SelectedDeviceSSID
		{
			get { return (string)GetValue(SelectedDeviceSSIDProperty); }
			set
			{
				SetValue(SelectedDeviceSSIDProperty, value);
			}
		}

		/// <summary>
		/// The NewDeviceToken property.
		/// </summary>
		public static BindableProperty NewDeviceTokenProperty =
			BindableProperty.Create(nameof(DeviceToken), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.DeviceToken = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the NewDeviceToken of the LinkUpPage instance.
		/// </summary>
		public string DeviceToken
		{
			get { return (string)GetValue(NewDeviceTokenProperty); }
			set
			{
				SetValue(NewDeviceTokenProperty, value);
			}
		}

		/// <summary>
		/// The RegistrationProgress property.
		/// </summary>
		public static BindableProperty RegistrationProgressProperty =
			BindableProperty.Create(nameof(RegistrationProgress), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.RegistrationProgress = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the RegistrationProgress of the LinkUpPage instance.
		/// </summary>
		public string RegistrationProgress
		{
			get { return (string)GetValue(RegistrationProgressProperty); }
			set
			{
				SetValue(RegistrationProgressProperty, value);
			}
		}

		/// <summary>
		/// The IsConfiguringDevice property.
		/// </summary>
		public static BindableProperty IsConfiguringDeviceProperty =
			BindableProperty.Create(nameof(IsConfiguringDevice), typeof(bool), typeof(LinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.IsConfiguringDevice = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsConfiguringDevice of the LinkUpPage instance.
		/// </summary>
		public bool IsConfiguringDevice
		{
			get { return (bool)GetValue(IsConfiguringDeviceProperty); }
			set
			{
				SetValue(IsConfiguringDeviceProperty, value);
			}
		}

		/// <summary>
		/// Returns true if the current device supports reading network list
		/// </summary>
		public bool SupportsListingWifiNetworks
		{
			get { return DeviceDriveManager.Current.CanReadNetworkList; }
		}

		/// <summary>
		/// Returns true if manual registration of wifi networks are necessary
		/// </summary>
		/// <value><c>true</c> if is manual network registration; otherwise, <c>false</c>.</value>
		public bool IsManualNetworkRegistration
		{
			get { return !DeviceDriveManager.Current.CanReadNetworkList; }
		}

		/// <summary>
		/// The IsWaitingForNetwork property.
		/// </summary>
		public static BindableProperty IsWaitingForNetworkProperty =
			BindableProperty.Create(nameof(IsWaitingForNetwork), typeof(bool), typeof(LinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.IsWaitingForNetwork = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsWaitingForNetwork of the LinkUpPage instance.
		/// </summary>
		public bool IsWaitingForNetwork
		{
			get { return (bool)GetValue(IsWaitingForNetworkProperty); }
			set
			{
				SetValue(IsWaitingForNetworkProperty, value);
			}
		}

		/// <summary>
		/// The NoDevicesFoundVisible property.
		/// </summary>
		public static BindableProperty NoDevicesFoundVisibleProperty =
			BindableProperty.Create(nameof(NoDevicesFoundVisible), typeof(bool), typeof(LinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.NoDevicesFoundVisible = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the NoDevicesFoundVisible of the LinkUpPage instance.
		/// </summary>
		public bool NoDevicesFoundVisible
		{
			get { return (bool)GetValue(NoDevicesFoundVisibleProperty); }
			set
			{
				SetValue(NoDevicesFoundVisibleProperty, value);
			}
		}

		/// <summary>
		/// The IsLoadingDevices property.
		/// </summary>
		public static BindableProperty IsLoadingDevicesProperty =
			BindableProperty.Create(nameof(IsLoadingDevices), typeof(bool), typeof(LinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.IsLoadingDevices = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsLoadingDevices of the LinkUpPage instance.
		/// </summary>
		public bool IsLoadingDevices
		{
			get { return (bool)GetValue(IsLoadingDevicesProperty); }
			set
			{
				SetValue(IsLoadingDevicesProperty, value);
			}
		}

		/// <summary>
		/// The WaitingLabel property.
		/// </summary>
		public static BindableProperty WaitingLabelProperty =
			BindableProperty.Create(nameof(WaitingLabel), typeof(string), typeof(LinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (LinkUpPage)bindable;
					ctrl.WaitingLabel = (string)newValue;
				});

		/// <summary>
		/// Gets or sets the WaitingLabel of the LinkUpPage instance.
		/// </summary>
		public string WaitingLabel
		{
			get { return (string)GetValue(WaitingLabelProperty); }
			set
			{
				SetValue(WaitingLabelProperty, value);
			}
		}
		#endregion

		#region Commands

		public Command CancelCommand
		{
			get { return new Command(async () => await Navigation.PopModalAsync(true)); }
		}

		public Command DoneCommand
		{
			get 
			{ 
				return new Command(async (obj) => {

					MessagingCenter.Send(this, LinkUpFinishedSuccessMessage, DeviceName);
					await Navigation.PopModalAsync(true);
				}); 
			}
		}

		public Command SearchAgainCommand
		{
			get
			{
				return new Command(async () =>
				{

					await LoadAvailableDevicesAsync();

				});
			}
		}

		public Command EnterWifiPasswordCommand
		{
			get
			{
				return new Command(async () => {

					await ActivatePageEnterWifiPasswordAsync();

				});
			}
		}

		public Command SelectWifiPasswordCommand
		{
			get
			{
				return new Command(async () =>
				{
					var netWorkInfo = await DeviceDriveManager.Current.GetCurrentWifiNetworkAsync();
					if (netWorkInfo == null)
					{
						await DisplayAlert(Title, "Could not find Wifi network.", "OK");
						await Navigation.PopModalAsync(true);
						return;
					}

					WifiNetworkName = netWorkInfo.NetworkSSID;
					await ActivatePageSelectDeviceAsync();
				});
			}
		}

		public Command SelectDeviceCommand
		{
			get
			{
				return new Command(async () =>
				{
					ActivePageNumber = 2;

					await ActivatePageSelectDeviceAsync();
				});
			}
		}

		public Command SelectLinkUpPageCommand
		{
			get
			{
				return new Command(async (ssidObject) => { 
					SelectedDeviceSSID = ssidObject as string;
					await ActivatePageLinkUpAsync();
				});
			}
		}
		#endregion

		#region Private Members

		/// <summary>
		/// Activate Page 1 - Enter name of the page enter device.
		/// </summary>
		private void ActivatePageEnterDeviceName()
		{
			ActivePageNumber = 0;
			IsWaitingForNetwork = false;
			IsConfiguringDevice = false;
			DeviceToken = string.Empty;
			DeviceName = string.Empty;
		}

		/// <summary>
		/// Activates Page 2 - Enter wifi password for current network.
		/// </summary>
		private async Task ActivatePageEnterWifiPasswordAsync()
		{
			// Load current network
			var currentNetworkInfo = await DeviceDriveManager.Current.GetCurrentWifiNetworkAsync();
			if (currentNetworkInfo != null)
				SelectedDeviceSSID = currentNetworkInfo.NetworkSSID;

			// Change page
			ActivePageNumber = 1;
		}

		/// <summary>
		/// Activates Page 3 - Select device or tell the user to select network manually
		/// </summary>
		private async Task ActivatePageSelectDeviceAsync()
		{
			// Set up page
			IsWaitingForNetwork = true;
			ActivePageNumber = 2;
			NoDevicesFoundVisible = false;

			// Check internet connection
			if (!DeviceDriveManager.Current.IsInternetAvailable)
			{
				await DisplayAlert(Title, "Internet is not currently available.", "OK");
				return;
			}

			// Try to get a list of available networks
			WaitingLabel = "Loading Devices...";

			if (!DeviceDriveManager.Current.CanReadNetworkList)
			{
				// We have a situation where the provider could not list devices, and need to ask 
				// the user to manually connect to a network
				ListenToAppActivatedMessages();
				WaitingLabel = "We'll be waiting";

				await Task.Run(async () =>
				{
					if (string.IsNullOrEmpty(DeviceToken))
						DeviceToken = await GetDeviceTokenAsync();
				});
			}
			else
			{
				// Get available networks
				await LoadAvailableDevicesAsync();
			}
		}

		/// <summary>
		/// Activates Page 4 - LinkUp and configure device
		/// </summary>
		/// <returns>The page3.</returns>
		async Task ActivatePageLinkUpAsync()
		{
			ActivePageNumber = 3;
			IsWaitingForNetwork = false;
			IsConfiguringDevice = true;

			// Start linkup process
			DeviceDriveManager.Current.LinkUpStateChanged += LinkUp_StateChanged;

			try
			{
				// Get token
				if (string.IsNullOrEmpty(DeviceToken))
					DeviceToken = await GetDeviceTokenAsync();				

				// Bail out if device token is not set.
				if (string.IsNullOrEmpty(DeviceToken))
					return;

				// Perform linkup
				await DeviceDriveManager.Current.LinkUpDeviceAsync(
					DeviceToken, SelectedDeviceSSID, WifiPassword, SelectedDeviceSSID, IsNewLinkUp);

				// We are done
				IsConfiguringDevice = false;

				// Go to last page
				ActivatePageRegistrationDone();

			}
			catch (Exception ex)
			{
				var errorMessage = ex.Message;
				if (ex.InnerException != null)
					errorMessage += System.Environment.NewLine + ex.InnerException.Message;

				await DisplayAlert(Title, errorMessage, "OK");

				// Exit LinkUp View
				await CleanUpAsync();
				await Navigation.PopModalAsync(true);

				return;
			}
			finally
			{
				DeviceDriveManager.Current.LinkUpStateChanged -= LinkUp_StateChanged;
			}
		}

		/// <summary>
		/// Activates the page registration done.
		/// </summary>
		private void ActivatePageRegistrationDone()
		{
			ActivePageNumber = 4;
		}

		/// <summary>
		/// Cleans up.
		/// </summary>
		/// <returns>The up.</returns>
		private Task CleanUpAsync()
		{
			IsWaitingForNetwork = false;
			IsConfiguringDevice = false;

			return Task.FromResult(true);
		}

		void ListenToAppActivatedMessages()
		{
			MessagingCenter.Subscribe<App>(this, App.AppOnResumeMessage, (obj) => 
			{
				Task.Run(async () =>
				{

					int retryCount = 0;
					while (retryCount++ < 20)
					{
						// check if network has changed
						var newNetworkInfo = await DeviceDriveManager.Current.GetCurrentWifiNetworkAsync();
						if (newNetworkInfo != null && newNetworkInfo.NetworkSSID != WifiNetworkName &&
							newNetworkInfo.NetworkSSID.Contains("DeviceDrive"))
						{
							Device.BeginInvokeOnMainThread(async () => await ActivatePageLinkUpAsync());
							break;
						}

						await Task.Delay(500);
					}
				});
			});
		}

		async Task<string> GetDeviceTokenAsync()
		{
			// Get device token
			try
			{
				// Call Device API endpoint and ask for a new device token
				var deviceRegistration = await DeviceDriveManager.Current.CreateNewDevice(DeviceName);

				// If no token was set, show to user and display error
				if (string.IsNullOrEmpty(deviceRegistration.Token))
				{
					await DisplayAlert(Title, "Could not register with the DeviceDrive Cloud.", "OK");
					return null;
				}

				return deviceRegistration.Token;
			}
			catch (Exception ex)
			{
				// Show error message
				await DisplayAlert(Title, $"Could not register with the DeviceDrive Cloud.\n{ex.Message}", "OK");
				return null; ;
			}
		}

		async Task LoadAvailableDevicesAsync()
		{
			try
			{
				IsLoadingDevices = true;

				var availableNetworks = await DeviceDriveManager.Current.GetAvailableDevicesAsync(null);

				foreach(var network in availableNetworks)
					AvailableDevices.Add(network);

				// We have a list of devices, do nothing, just wait for a device selection
				IsWaitingForNetwork = false;
				WaitingLabel = string.Empty;
				IsLoadingDevices = false;
				NoDevicesFoundVisible = !AvailableDevices.Any();
			}
			catch (TaskCanceledException)
			{
				// Swallow
			}

		}

		/// <summary>
		/// Callback when linkup state changes
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void LinkUp_StateChanged(object sender, LinkUpState e)
		{
			switch (e)
			{
				case LinkUpState.StartingClearDevice:
					RegistrationProgress = "Connecting to device...";
					break;

				case LinkUpState.SendingWifiCredentials:
					RegistrationProgress = "Sending Wifi Credentials...";
					break;

				case LinkUpState.DeviceConnectedToAP:
					RegistrationProgress = "Connected to AP";
					break;

				case LinkUpState.DeviceTurnedOffAP:
					RegistrationProgress = "Access Point Closed";
					break;

				case LinkUpState.DeviceSentConfiguration:
					RegistrationProgress = "Device sent configuration";
					break;
			}
		}

		void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			SelectLinkUpPageCommand.Execute(e.SelectedItem as string);
		}
		#endregion
	}
}
