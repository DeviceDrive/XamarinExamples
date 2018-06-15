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
	public partial class SmartLinkUpPage : ContentPage
	{
		public const string LinkUpFinishedMessage = "LinkUpFinishedMessage";

		#region Private Members

		bool _onAppearingCalled = false;

        /// <summary>
        /// The link up timeout seconds.
        /// </summary>
        const int LinkUpTimeoutSeconds = 60;

        #endregion

        /// <summary>
        /// Create a new instance of the page for linking up a new device
        /// </summary>
        public SmartLinkUpPage()
		{
			InitializeComponent();

			BindingContext = this;
			IsNewLinkUp = true;
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
		/// The ActivePageNumber property.
		/// </summary>
		public static BindableProperty ActivePageNumberProperty =
			BindableProperty.Create(nameof(ActivePageNumber), typeof(int), typeof(SmartLinkUpPage), 0,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(DeviceName), typeof(string), typeof(SmartLinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(WifiPassword), typeof(string), typeof(SmartLinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
		/// The CurrentWifiSSID property.
		/// </summary>
		public static BindableProperty CurrentWifiSSIDProperty =
            BindableProperty.Create(nameof(WifiPassword), typeof(string), typeof(SmartLinkUpPage), null,
                BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SmartLinkUpPage)bindable;
                    ctrl.CurrentWifiSSID = (string)newValue;
                });

        /// <summary>
        /// Gets or sets the current wifi ssid.
        /// </summary>
        /// <value>The current wifi ssid.</value>
        public string CurrentWifiSSID
        {
            get { return (string)GetValue(CurrentWifiSSIDProperty); }
            set { SetValue(CurrentWifiSSIDProperty, value); }
        }

        /// <summary>
		/// The CurrentWifiBSSID property.
		/// </summary>
		public static BindableProperty CurrentWifiBSSIDProperty =
            BindableProperty.Create(nameof(WifiPassword), typeof(string), typeof(SmartLinkUpPage), null,
                BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SmartLinkUpPage)bindable;
                    ctrl.CurrentWifiBSSID = (string)newValue;
                });

        /// <summary>
        /// Gets or sets the current wifi bssid.
        /// </summary>
        /// <value>The current wifi bssid.</value>
        public string CurrentWifiBSSID
        {
            get { return (string)GetValue(CurrentWifiBSSIDProperty); }
            set { SetValue(CurrentWifiBSSIDProperty, value); }
        }

        /// <summary>
		/// The CurrentWifiHostAddress property.
		/// </summary>
		public static BindableProperty CurrentWifiHostAddressProperty =
            BindableProperty.Create(nameof(WifiPassword), typeof(string), typeof(SmartLinkUpPage), null,
                BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SmartLinkUpPage)bindable;
                    ctrl.CurrentWifiHostAddress = (string)newValue;
                });

        /// <summary>
        /// Gets or sets the current wifi host address.
        /// </summary>
        /// <value>The current wifi host address.</value>
        public string CurrentWifiHostAddress
        {
            get { return (string)GetValue(CurrentWifiHostAddressProperty); }
            set { SetValue(CurrentWifiHostAddressProperty, value); }
        }


      

		/// <summary>
		/// The SelectedDeviceSSID property.
		/// </summary>
		public static BindableProperty SelectedDeviceSSIDProperty =
			BindableProperty.Create(nameof(SelectedDeviceSSID), typeof(string), typeof(SmartLinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(DeviceToken), typeof(string), typeof(SmartLinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(RegistrationProgress), typeof(string), typeof(SmartLinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(IsConfiguringDevice), typeof(bool), typeof(SmartLinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(IsWaitingForNetwork), typeof(bool), typeof(SmartLinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(NoDevicesFoundVisible), typeof(bool), typeof(SmartLinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(IsLoadingDevices), typeof(bool), typeof(SmartLinkUpPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			BindableProperty.Create(nameof(WaitingLabel), typeof(string), typeof(SmartLinkUpPage), null,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (SmartLinkUpPage)bindable;
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
			get {
                return new Command(async () =>
                {
                    await Navigation.PopModalAsync(true);
                    MessagingCenter.Send(this, LinkUpFinishedMessage, DeviceName);
                });
            }
		}

 
        public Command StartSmartLinkUpCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ActivePageNumber = 3;

                    await StartSmartLinkUp();
                });
            }
        }

        public Command DoneCommand
		{
			get 
			{ 
				return new Command(async (obj) => {

					MessagingCenter.Send(this, LinkUpFinishedMessage, DeviceName);
					await Navigation.PopModalAsync(true);
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

                    CurrentWifiSSID = netWorkInfo.NetworkSSID;
					await ActivatePageReadyForSmartLinkUp();
				});
			}
		}

		public Command ReadyForSmartLinkUpCommand
        {
			get
			{
				return new Command(async () =>
				{
					ActivePageNumber = 2;

					await ActivatePageReadyForSmartLinkUp();
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
            if (currentNetworkInfo != null) { 
                CurrentWifiSSID = currentNetworkInfo.NetworkSSID;
                CurrentWifiBSSID = currentNetworkInfo.NetworkBSSID;
                CurrentWifiHostAddress = currentNetworkInfo.HostAddress;
            }
			// Change page
			ActivePageNumber = 1;
		}

        /// <summary>
		/// Activates Page 3 - Wait for user to start SmartLinkup
		/// </summary>
		private async Task ActivatePageReadyForSmartLinkUp()
        {

            // Change page
            ActivePageNumber = 2;
        }

        /// <summary>
        /// Activates Page 4 - Runs the smart linkup.
        /// </summary>
        /// <returns>The smart link up.</returns>
        private async Task StartSmartLinkUp()
        {
            // Change Page
            ActivePageNumber = 3;
            IsConfiguringDevice = true;

            // Create new device in the cloud if this is a new linkup
            //RegistrationProgress = Strings.ProgressRetrievingToken;
            if (string.IsNullOrEmpty(DeviceToken))
                DeviceToken = await GetDeviceTokenAsync();

            // Start SmartLinkUp process
            DeviceDriveManager.Current.SmartLinkUpStateChanged += SmartLinkUpStateChanged;

            try
            {
                // Perfomr Smart LinkUp
                var result = await DeviceDriveManager.Current.SmartLinkUpDeviceAsync(DeviceToken, CurrentWifiSSID, CurrentWifiBSSID, WifiPassword, CurrentWifiHostAddress, LinkUpTimeoutSeconds);
                IsConfiguringDevice = false;

                switch (result.Result)
                {
                    case SmartLinkUpResult.Success:
                        ActivatePageRegistrationDone();
                        break;
                    case SmartLinkUpResult.Cancelled:
                        break;
                    case SmartLinkUpResult.InstanceIsRunning:
                        // Hide
                        await DisplayAlert(Title, "SmartLinkUp instance is already running.", "OK");
                        await Navigation.PopModalAsync(true);
                        break;
                    case SmartLinkUpResult.Failed:
                        // Got answer from WRF, but could not broadcast token. So password is correct, but can be device died
                        //await MvvmApp.Current.Presenter.ShowMessageAsync(Strings.AppName, Strings.LabelSmartConfigFailed, Strings.ButtonOK);
                        await DisplayAlert(Title, "Failed when sending token to device. Set device in SmartLinkUp and try again.", "OK");
                        ActivatePageReadyForSmartLinkUp();
                        break;
                    case SmartLinkUpResult.Timeout:
                        // Did not get answer from a device, make sure device is on and password is correct.
                        //await MvvmApp.Current.Presenter.ShowMessageAsync(Strings.AppName, Strings.LabelSmartConfigTimedOut, Strings.ButtonOK);
                        await DisplayAlert(Title, "Could not find your device.Make sure password is correct and that the device is ready for SmartLinkUp.", "OK");
                        await Navigation.PopModalAsync(true);
                        break;

                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += System.Environment.NewLine + ex.InnerException.Message;

                //await MvvmApp.Current.Presenter.ShowMessageAsync(Strings.AppName,
                //    errorMessage, Strings.ButtonOK);

                // Exit LinkUp View
                await CleanUpAsync();
                //await MvvmApp.Current.Presenter.DismissViewModelAsync(this.PresentationMode, true);

                return;
            }
            finally
            {
                DeviceDriveManager.Current.SmartLinkUpStateChanged -= SmartLinkUpStateChanged;
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
					DeviceToken, CurrentWifiSSID, WifiPassword, CurrentWifiSSID, IsNewLinkUp);

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
						if (newNetworkInfo != null && newNetworkInfo.NetworkSSID != CurrentWifiHostAddress &&
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

        void SmartLinkUpStateChanged(object sender, SmartLinkUpStateChangedEventArgs e)
        {

            switch (e.State)
            {
                case SmartLinkUpState.Broadcasting:
                    //RegistrationProgress = Strings.ProgressBroadcasting;
                    break;
                case SmartLinkUpState.PassingToken:
                    //RegistrationProgress = Strings.ProgressPassingToken;
                    break;
                case SmartLinkUpState.Cancelling:
                    //RegistrationProgress = Strings.ProgressCanceling;
                    break;

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
