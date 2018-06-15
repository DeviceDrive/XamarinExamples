using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using DeviceDrive.SDK;
using DeviceDrive.SDK.Contracts;
using LightSwitch.Controls;
using Xamarin.Forms;

namespace LightSwitch
{
	public partial class MainPage : ContentPage
	{
		#region Constants
		public const string PropertyUpdatedMessage = "PropertyUpdatedMessage";
		#endregion

		#region Class Members

		bool _onAppearingCalled = false;
		Command _selectDeviceCommand;
		readonly ObservableCollection<DeviceModel> _devices = new ObservableCollection<DeviceModel>();
		readonly ObservableCollection<DevicePropertyModel> _selectedDeviceProperties = new ObservableCollection<DevicePropertyModel>();
		readonly IDeviceDrivePlatform _platform;
        readonly ITermsService _termsService;
        readonly IContainer _container;
        
		#endregion

		public MainPage(IDeviceDrivePlatform platform, IContainer container)
		{
			InitializeComponent();
			BindingContext = this;

			Title = "LightSwitch";

			_platform = platform;
            _container = container;

            using (var scope = container.BeginLifetimeScope())
            {
                _termsService = scope.Resolve<ITermsService>();
            }
            
		}

		/// <summary>
		/// Execute when page is displayed
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (_onAppearingCalled)
				return;

			_onAppearingCalled = true;

			DeviceDriveManager.Current.Initialized += async (sender, e) => {

                // Subscribe to the sleep/resume messages from the main app class
                MessagingCenter.Subscribe<App>(this, App.AppOnResumeMessage, async (obj) => await DeviceDriveManager.Current.OnResumedAsync());
                MessagingCenter.Subscribe<App>(this, App.AppOnSleepMessage, async (obj) => await DeviceDriveManager.Current.OnPausedAsync());

                // Whenever the bool cell property changes its value, we need to update the property in the SDK
                MessagingCenter.Subscribe<BoolPropertyCell, Tuple<string, string>>(
                    this, BoolPropertyCell.BoolPropertyChangedMessage, async (sender2, model) => {
                        await DeviceDriveManager.Current.SetDevicePropertyValueAsync(SelectedDevice, model.Item1, model.Item2);
                    });

                // Handle delete active device message from the menu page
                MessagingCenter.Subscribe<MenuPage>(this, MenuPage.DeleteActiveDeviceMessage, async (obj) =>
                {
                    // Delete active device
                    if (SelectedDevice != null)
                    {
                        if (await DisplayAlert(Title, $"Delete {SelectedDevice.Name}?", "OK", "Cancel"))
                        {
                            SelectedDevice = null;
                            await DeviceDriveManager.Current.DeleteDeviceAsync(SelectedDevice);
                        }
                    }
                });

                // Handle start linkup active device from the menu page
                MessagingCenter.Subscribe<MenuPage>(this, MenuPage.LinkUpActiveDeviceMessage, async (obj) =>
                {
                    // LinkUp Active Device again
                    if (SelectedDevice != null)
                        await Navigation.PushModalAsync(new LinkUpPage(SelectedDevice));
                });

                // Handle start smartlinkup process menu page
                MessagingCenter.Subscribe<MenuPage>(this, MenuPage.SmartLinkUpDeviceMessage, async (obj) =>
                {
                    // LinkUp Active Device again
                    await Navigation.PushModalAsync(new SmartLinkUpPage());
                });

                MessagingCenter.Subscribe<MenuPage>(this, MenuPage.TermsConditionsMessage, async (obj) =>
                {
                    try
                    {
                        String terms = await _termsService.GetDecodedTermsConditions();
                        await Navigation.PushModalAsync(new TermsPage(_container));
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert(Title, "Cannot fetch Terms and Conditions!", "OK");
                    }
                });

                MessagingCenter.Subscribe<SmartLinkUpPage>(this, SmartLinkUpPage.LinkUpFinishedMessage, async (obj) =>
                {
                    await LoadDevicesAsync();
                });

                // Events
                DeviceDriveManager.Current.StateChanged += HandleSDKStateChanged;
				DeviceDriveManager.Current.AuthenticatedChanged += HandleAuthenticationChanged;
				DeviceDriveManager.Current.DevicesUpdated += HandleDevicesUpdated;
				DeviceDriveManager.Current.DeviceUpdated += HandleDeviceUpdated;
				DeviceDriveManager.Current.DeviceDeleted += HandleDeviceDeleted;

				// Show welcome screen
				if (!DeviceDriveManager.Current.Preferences.WelcomeScreenDisplayed)
				{
					MessagingCenter.Subscribe<StartupPage>(this, StartupPage.AuthenticatedMessage, async (obj) =>
					{
						MessagingCenter.Unsubscribe<StartupPage>(this, StartupPage.AuthenticatedMessage);
						await LoadDevicesAsync();
                        await CheckTermsConditionsAcceptance();
                    });

					await Navigation.PushModalAsync(new StartupPage());
				}
				else
				{
					// Check for authentication
					if ((await DeviceDriveManager.Current.Authentication.TryGetIsAuthenticatedAsync()))
                    {
						await LoadDevicesAsync();
                        await CheckTermsConditionsAcceptance();
                    }
					else
						await Navigation.PushModalAsync(new StartupPage());
				}
			};

			// Start initialization
			Task.Run(async () =>
                {
                    try
                    {
                        await DeviceDriveManager.Current.InitializeAsync("78795c26-9ca8-4251-8cbb-0cb13850026f", "f2026cb1-27a0-4f1e-9fe3-12f46e070f03", _platform);
                    }
                    catch(Exception ex)
                    {
                        await DisplayAlert(Title, "Cannot initialize DeviceDriveManager!", "OK");
                    }
                   
                });	

		}

        private async Task CheckTermsConditionsAcceptance()
        {
           
            // Check if Terms and Conditions are accepted, if not, show dialog
            try
            {
                String terms = "";
                terms = await _termsService.GetDecodedTermsConditions();
                bool accepted = await _termsService.Check();
                if (!accepted)
                {
                    await Navigation.PushModalAsync(new TermsPage(_container));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title, "Cannot fetch Terms and Conditions!", "OK");
            }

            
        }

        #region Properties

        /// <summary>
        /// The StatusText property.
        /// </summary>
        public static BindableProperty StatusTextProperty = BindableProperty.Create(
			nameof(StatusText), typeof(string), typeof(MainPage), null,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the StatusText of the MainPage instance.
		/// </summary>
		public string StatusText
		{
			get { return (string)GetValue(StatusTextProperty); }
			set { SetValue(StatusTextProperty, value); }
		}

        /// <summary>
        /// The StatusText property.
        /// </summary>
        public static BindableProperty LightBulbSwitchProperty = BindableProperty.Create(
            nameof(LightBulbSwitch), typeof(bool), typeof(MainPage), false,
            BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the StatusText of the MainPage instance.
        /// </summary>
        public bool LightBulbSwitch
        {
            get { return (bool)GetValue(LightBulbSwitchProperty); }
            set {
                SetValue(LightBulbSwitchProperty, value);
                lightBulbTop.Source = (value) ? ImageSource.FromFile("light_bulb_top_on.png") : ImageSource.FromFile("light_bulb_top_off.png");
                ApplyTint(value);
            }
        }

        private void ApplyTint(bool value)
        {
            lightBulbTop.Effects.Clear();
            lightBulbTop.Effects.Add(CreateTintImageEffect(value));
            if (Device.RuntimePlatform == Device.iOS)
            {
                //on iOS when we enter the screen, UIImageView.Image is null, in this case we have to set the effect again to be visible.
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    lightBulbTop.Effects.Clear();
                    lightBulbTop.Effects.Add(CreateTintImageEffect(LightBulbSwitch));
                    return false;
                });
            }
        }

        private Effect CreateTintImageEffect(bool value)
        {
            TintImageEffect tie = new TintImageEffect();
            int k1 = (int)Math.Round(1.5 * LightBulbIntensity) + 105;
            int k2 = (int)Math.Round(0.9 * LightBulbIntensity) + 165;
            tie.TintColor = value ? (Device.RuntimePlatform == Device.Android ? Color.FromRgb(k1, k1, k1) : Color.FromRgb(k2, k2, 0)) : Color.Transparent;
            return tie;
        }

        /// <summary>
        /// The StatusText property.
        /// </summary>
        public static BindableProperty LightBulbIntensityProperty = BindableProperty.Create(
            nameof(LightBulbIntensity), typeof(int), typeof(MainPage), 50,
            BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the StatusText of the MainPage instance.
        /// </summary>
        public int LightBulbIntensity
        {
            get { return (int)GetValue(LightBulbIntensityProperty); }
            set {
                SetValue(LightBulbIntensityProperty, value);
                lightBulbIntensitySlider.Value = value;
                ApplyTint(LightBulbSwitch);
            }
        }

        /// <summary>
        /// The IsDevicesVisible property.
        /// </summary>
        public static BindableProperty IsDevicesVisibleProperty =
			BindableProperty.Create(nameof(IsDevicesVisible), typeof(bool), typeof(MainPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (MainPage)bindable;
					ctrl.IsDevicesVisible = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsDevicesVisible of the MainPage instance.
		/// </summary>
		public bool IsDevicesVisible
		{
			get { return (bool)GetValue(IsDevicesVisibleProperty); }
			set { SetValue(IsDevicesVisibleProperty, value); }
		}

        /// <summary>
        /// The IsDevicesVisible property.
        /// </summary>
        public static BindableProperty IsLightBulbVisibleProperty =
            BindableProperty.Create(nameof(IsLightBulbVisible), typeof(bool), typeof(MainPage), false,
                BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MainPage)bindable;
                    ctrl.IsLightBulbVisible = (bool)newValue;
                });

        /// <summary>
        /// Gets or sets the IsDevicesVisible of the MainPage instance.
        /// </summary>
        public bool IsLightBulbVisible
        {
            get { return (bool)GetValue(IsLightBulbVisibleProperty); }
            set { SetValue(IsLightBulbVisibleProperty, value); }
        }

        /// <summary>
        /// The IsLoadingDeviceProperties property.
        /// </summary>
        public static BindableProperty IsLoadingDevicePropertiesProperty =
			BindableProperty.Create(nameof(IsLoadingDeviceProperties), typeof(bool), typeof(MainPage), true,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (MainPage)bindable;
					ctrl.IsLoadingDeviceProperties = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsLoadingDeviceProperties of the MainPage instance.
		/// </summary>
		public bool IsLoadingDeviceProperties
		{
			get { return (bool)GetValue(IsLoadingDevicePropertiesProperty); }
			set { SetValue(IsLoadingDevicePropertiesProperty, value); }
		}

		/// <summary>
		/// The IsDevicePropertiesVisible property.
		/// </summary>
		public static BindableProperty IsDevicePropertiesVisibleProperty =
			BindableProperty.Create(nameof(IsDevicePropertiesVisible), typeof(bool), typeof(MainPage), false,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (MainPage)bindable;
					ctrl.IsDevicePropertiesVisible = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsDevicePropertiesVisible of the MainPage instance.
		/// </summary>
		public bool IsDevicePropertiesVisible
		{
			get { return (bool)GetValue(IsDevicePropertiesVisibleProperty); }
			set { SetValue(IsDevicePropertiesVisibleProperty, value); }
		}

		/// <summary>
		/// The IsLoadingDevices property.
		/// </summary>
		public static BindableProperty IsLoadingDevicesProperty =
			BindableProperty.Create(nameof(IsLoadingDevices), typeof(bool), typeof(MainPage), true,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (MainPage)bindable;
					ctrl.IsLoadingDevices = (bool)newValue;
				});

		/// <summary>
		/// Gets or sets the IsLoadingDevices of the MainPage instance.
		/// </summary>
		public bool IsLoadingDevices
		{
			get { return (bool)GetValue(IsLoadingDevicesProperty); }
			set { SetValue(IsLoadingDevicesProperty, value); }
		}

		/// <summary>
		/// Gets or sets the SelectedDevice of the MainPage instance.
		/// </summary>
		public DeviceModel SelectedDevice
		{
			get
			{
				if (!DeviceDriveManager.Current.IsInitialized)
					return null;

                if (DeviceDriveManager.Current.Preferences == null)
                    return null;

				var id = DeviceDriveManager.Current.Preferences.GetObjectForKey(
					() => Devices.FirstOrDefault()?.UserDeviceId);

				if (id != null)
					return Devices.FirstOrDefault((arg) => arg.UserDeviceId == id);

				return Devices.FirstOrDefault();
			}
			set
			{				
				DeviceDriveManager.Current.Preferences.SetObjectForKey(value?.UserDeviceId);
				OnPropertyChanged(nameof(SelectedDeviceName));
			}
		}

		/// <summary>
		/// Returns the name of the selected device
		/// </summary>
		public string SelectedDeviceName { get { return SelectedDevice?.Name; } }

        /// <summary>
		/// Returns the name of the selected device
		/// </summary>
		public int SelectedDeviceId { get { return SelectedDevice != null ? SelectedDevice.UserDeviceId : -1; } }

        /// <summary>
        /// Returns the list of properties for the selected device
        /// </summary>
        public ObservableCollection<DevicePropertyModel> SelectedDeviceProperties { get { return _selectedDeviceProperties; } }

		/// <summary>
		/// Returns the list of devices
		/// </summary>
		public ObservableCollection<DeviceModel> Devices { get { return _devices; } }

		#endregion

		#region Commands

		/// <summary>
		/// Returns the command for selecting a device
		/// </summary>
		public Command SelectDeviceCommand
		{
			get
			{
				return _selectDeviceCommand ?? (_selectDeviceCommand = new Command(async () =>
				{

					var buttons = Devices.Select(d => d.Name).ToList();
					var retVal = await DisplayActionSheet("Select Device:", "Cancel", null, buttons.ToArray());
					var index = buttons.IndexOf(retVal);
					if (index > -1)
					{
						var device = Devices.ElementAt(index);

						SelectedDevice = device;
						await LoadDevicePropertiesAsync(device);

					}
				}));
			}
		}

		/// <summary>
		/// Returns the command adding a new device
		/// </summary>
		/// <value>The addd device command.</value>
		public Command AdddDeviceCommand
		{
			get
			{
				return new Command(async () => await Navigation.PushModalAsync(new LinkUpPage()));
			}
		}

        public Command TapLightBulbCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (IsLightBulbVisible) SendPropertyValueChange("switch", LightBulbSwitch ? "0" : "1");
                    LightBulbSwitch = !LightBulbSwitch;
                });
            }
        }
        #endregion

        Boolean fireEvent = true;

        private void LightBulbIntensitySliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (fireEvent && IsLightBulbVisible)
            {
                fireEvent = false;
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    fireEvent = true;
                    if (SelectedDeviceId != -1)
                    {
                        SendPropertyValueChange("intensity", (lightBulbIntensitySlider.Value).ToString(CultureInfo.InvariantCulture));
                    }
                    return false;
                });
            }
        }

        async void SendPropertyValueChange(String parameter, String value)
        {
            try
            {
                // We have a data value change - tell the service!
                var device = await DeviceDriveManager.Current.GetCachedDeviceAsync(SelectedDeviceId);

                // Signal that we updated the property
                await DeviceDriveManager.Current.SetDevicePropertyValueAsync(device, parameter, value?.ToString());

            }
            catch (Exception ex)
            {
                await DisplayAlert(Title, $"{ex.Message}", "OK");
            }
        }

        #region Private Members

        /// <summary>
        /// Loads devices useing the SDK
        /// </summary>
        async Task LoadDevicesAsync()
		{
			IsLoadingDevices = true;
			IsDevicesVisible = !IsLoadingDevices;

			try
			{
				var devices = await DeviceDriveManager.Current.GetDevicesAsync();

				Devices.Clear();
				foreach (var device in devices)
					Devices.Add(device);

				// Update GUI
				OnPropertyChanged(nameof(SelectedDeviceName));

			}
			finally
			{
				IsLoadingDevices = false;
				IsDevicesVisible = !IsLoadingDevices;
			}


			// Update properties
			await LoadDevicePropertiesAsync(SelectedDevice);
		}

		/// <summary>
		/// Loads device data/properties
		/// </summary>
		async Task LoadDevicePropertiesAsync(DeviceModel device)
		{
			if (device == null)
			{
				SelectedDeviceProperties.Clear();
				IsLoadingDeviceProperties = false;
				IsDevicePropertiesVisible = !IsLoadingDeviceProperties;
				return;
			}

			IsLoadingDeviceProperties = true;
			IsDevicePropertiesVisible = !IsLoadingDeviceProperties;

            bool lightInterface = false;

            try
			{
                // find device in list and update
				var props = await DeviceDriveManager.Current.GetDevicePropertiesAsync(device);
                if (props != null)
                {
                    if (props.Count() != SelectedDeviceProperties.Count())
                    {
                        SelectedDeviceProperties.Clear();
                        foreach (var prop in props)
                        {
                            if (prop.InterfaceName.Equals("com.devicedrive.light")) lightInterface = true;
                            if (prop.Name.ToLower().Equals("switch")) LightBulbSwitch = prop.Value.Equals("1");
                            if (prop.Name.ToLower().Equals("intensity")) LightBulbIntensity = (int)Math.Round(Double.Parse(prop.Value, CultureInfo.InvariantCulture));
                            SelectedDeviceProperties.Add(prop);
                        }
                    }
                    else
                    {
                        foreach (var prop in props)
                        {
                            System.Diagnostics.Debug.WriteLine("Updating " + prop.Name + " = " + prop.Value);
                            var listItemProp = SelectedDeviceProperties.FirstOrDefault(p => p.Name == prop.Name);
                            if (listItemProp != null)
                            {
                                listItemProp.Name = prop.Name;
                                listItemProp.Value = prop.Value;
                                if (prop.Name.ToLower().Equals("switch")) LightBulbSwitch = prop.Value.Equals("1");
                                if (prop.Name.ToLower().Equals("intensity")) LightBulbIntensity = (int)Math.Round(Double.Parse(prop.Value, CultureInfo.InvariantCulture));

                                MessagingCenter.Send(this, PropertyUpdatedMessage, prop);
                            }
                        }
                    }
                }
			}
			finally
			{
				IsLoadingDeviceProperties = false;
				IsDevicePropertiesVisible = !IsLoadingDeviceProperties;
                IsLightBulbVisible = lightInterface;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Handles SDK state changes
		/// </summary>
		void HandleSDKStateChanged(object sender, StateChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("SDK State changed to: " + e.NewState);

			if (e.NewState == SDKState.Initialized)
				StatusText = String.Empty;
			else
				StatusText = e.NewState.ToString();
		}

		/// <summary>
		/// Handles SDK authentication changes
		/// </summary>
		async void HandleAuthenticationChanged(object sender, AuthenticationChangedEventArgs e)
		{
			if (!e.IsAuthenticated)
			{
				IsDevicesVisible = false;
				await Navigation.PushModalAsync(new StartupPage());
			}
			else
			{
				IsDevicesVisible = true;
                await CheckTermsConditionsAcceptance();
            }
		}

		/// <summary>
		/// Handles updates in the list of devices
		/// </summary>
		void HandleDevicesUpdated(object sender, DevicesUpdatedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("HandleDevicesUpdated");

			var selectedDevice = e.UpdatedDevices.FirstOrDefault(d => d.UserDeviceId == SelectedDevice?.UserDeviceId);

			Devices.Clear();
			foreach (var d in e.UpdatedDevices)
				Devices.Add(d);

			SelectedDevice = selectedDevice;
		}

		/// <summary>
		/// Handles device property changes
		/// </summary>
		void HandleDeviceUpdated(object sender, DeviceUpdatedEventArgs e)
		{
			var listDevice = Devices.FirstOrDefault(p => p.UserDeviceId == e.Device.UserDeviceId);
			if (listDevice == SelectedDevice)
			{
				foreach (var prop in e.Properties)
				{
					var listItemProp = SelectedDeviceProperties.FirstOrDefault(p => p.Name == prop.Name);
					if (listItemProp != null)
					{
						// Update existing item
						listItemProp.Name = prop.Name;
						listItemProp.Value = prop.Value;
						MessagingCenter.Send(this, PropertyUpdatedMessage, prop);
					}
					else
					{
						// Add new property
						SelectedDeviceProperties.Add(prop);
					}
				}
			}
		}

		/// <summary>
		/// Handles device deleted 
		/// </summary>
		async void HandleDeviceDeleted(object sender, DeviceDeletedEventArgs e)
		{
			// Update list of devices
			var selectedDevice = SelectedDevice;
			Devices.Remove(e.Device);

			if (selectedDevice == e.Device)
			{
				SelectedDevice = Devices.FirstOrDefault();
				await LoadDevicePropertiesAsync(SelectedDevice);
			}
		}

		#endregion

	}
}
