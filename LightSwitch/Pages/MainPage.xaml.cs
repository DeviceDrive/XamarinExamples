using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DeviceDrive.SDK;
using DeviceDrive.SDK.Contracts;
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

		#endregion

		public MainPage()
		{
			InitializeComponent();
			BindingContext = this;

			Title = "LightSwitch";

			DeviceDriveManager.Current.Preferences.TestEnvironmentActivated = true;

			BindingContext = this;
				
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

			Device.BeginInvokeOnMainThread(async () =>
			{
				var isAuthenticated = await DeviceDriveManager.Current.Authentication.TryGetIsAuthenticatedAsync();
				if (!DeviceDriveManager.Current.Preferences.WelcomeScreenDisplayed || !isAuthenticated)
				{
					MessagingCenter.Subscribe<StartupPage>(this, StartupPage.AuthenticatedMessage, async (obj) =>
					{
						MessagingCenter.Unsubscribe<StartupPage>(this, StartupPage.AuthenticatedMessage);
						await LoadDevicesAsync();
					});

					await Navigation.PushModalAsync(new StartupPage());
				}
				else
					await LoadDevicesAsync();
			});

			MessagingCenter.Subscribe<App>(this, App.AppOnSleepMessage, async (msg) =>
			{
				if (SelectedDevice != null)
					await DeviceDriveManager.Current.Data.StopDeviceNotificationUpdatesAsync(new DeviceModel[] { SelectedDevice });
			});

			MessagingCenter.Subscribe<App>(this, App.AppOnResumeMessage, async (msg) =>
			{
				if (SelectedDevice != null)
					await DeviceDriveManager.Current.Data.StartDeviceNotiticationUpdatesAsync(new DeviceModel[] { SelectedDevice });
			});

			MessagingCenter.Subscribe<BoolPropertyCell, DevicePropertyModel>(
				this, BoolPropertyCell.BoolPropertyChangedMessage, async (sender, model) =>
				{
					var result = await DeviceDriveManager.Current.Data.UpdateDevicePropertyValue(SelectedDevice, model.Name, model.Value);
					System.Diagnostics.Debug.WriteLine(result);
				});

			MessagingCenter.Subscribe<MenuPage>(this, MenuPage.SignOuMessage, async (obj) =>
			{
				var isAuthenticated = await DeviceDriveManager.Current.Authentication.TryGetIsAuthenticatedAsync();
				if (!isAuthenticated)
				{
					IsDevicesVisible = false;

					await Navigation.PushModalAsync(new StartupPage());
				}
			});

			MessagingCenter.Subscribe<MenuPage>(this, MenuPage.DeleteActiveDeviceMessage, async (obj) =>
			{
				// Delete active device
				if (SelectedDevice != null)
				{
					if (await DisplayAlert(Title, $"Delete {SelectedDevice.Name}?", "OK", "Cancel"))
					{
						SelectedDevice = null;
						await DeviceDriveManager.Current.Devices.DeleteDeviceAsync(SelectedDevice);
						await LoadDevicesAsync();
					}
				}
			});

			MessagingCenter.Subscribe<MenuPage>(this, MenuPage.LinkUpActiveDeviceMessage, async (obj) =>
			{
				// LinkUp Active Device again
				if (SelectedDevice != null)
					await Navigation.PushModalAsync(new LinkUpPage(SelectedDevice));
			});

			MessagingCenter.Subscribe<LinkUpPage, string>(this, LinkUpPage.LinkUpFinishedSuccessMessage, async (sender, deviceName) => {
				await LoadDevicesAsync();
				var newSelectedDevice = Devices.FirstOrDefault(device => device.Name == deviceName);
				if (newSelectedDevice != null)
					await SetSelectedDeviceAsync(newSelectedDevice);
			});

			DeviceDriveManager.Current.Data.DeviceDataUpdated += Devices_DeviceDataUpdated;
		}

		#region Properties

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
			set
			{
				SetValue(IsDevicesVisibleProperty, value);
			}
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
			set
			{
				SetValue(IsLoadingDevicePropertiesProperty, value);
			}
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
			set
			{
				SetValue(IsDevicePropertiesVisibleProperty, value);
			}
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
			set
			{
				SetValue(IsLoadingDevicesProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the SelectedDevice of the MainPage instance.
		/// </summary>
		public DeviceModel SelectedDevice
		{
			get
			{
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

		public string SelectedDeviceName
		{
			get
			{
				return SelectedDevice?.Name;
			}
		}

		public ObservableCollection<DevicePropertyModel> SelectedDeviceProperties
		{
			get
			{
				return _selectedDeviceProperties;
			}
		}

		/// <summary>
		/// List of devices
		/// </summary>
		/// <value>The devices.</value>
		public ObservableCollection<DeviceModel> Devices
		{
			get
			{
				return _devices;
			}
		}

		#endregion

		#region Commands

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
						await SetSelectedDeviceAsync(Devices.ElementAt(index));
					}
				}));
			}
		}

		public Command AdddDeviceCommand
		{
			get
			{
				return new Command(async () => await Navigation.PushModalAsync(new LinkUpPage()));
			}
		}
		#endregion

		#region Private Members

		async Task LoadDevicesAsync()
		{
			var devices = await DeviceDriveManager.Current.Devices.GetDevicesAsync();
			Devices.Clear();
			foreach (var device in devices)
				Devices.Add(device);

			IsDevicesVisible = true;
			IsLoadingDevices = false;

			OnPropertyChanged(nameof(SelectedDeviceName));

			if (SelectedDevice != null)
				await LoadDeviceDataAsync(SelectedDevice);
		}

		Task SetSelectedDeviceAsync(DeviceModel deviceModel)
		{
			IsLoadingDeviceProperties = true;
			IsDevicePropertiesVisible = false;

			SelectedDevice = deviceModel;
			return LoadDeviceDataAsync(deviceModel);
		}

		async Task LoadDeviceDataAsync(DeviceModel device)
		{
			IsLoadingDeviceProperties = true;

			await DeviceDriveManager.Current.Data.UpdateDataValuesAsync(device);
			var props = await DeviceDriveManager.Current.Data.GetPropertiesAsync(device);
			SelectedDeviceProperties.Clear();
			foreach (var prop in props)
				SelectedDeviceProperties.Add(prop);

			await DeviceDriveManager.Current.Data.StartDeviceNotiticationUpdatesAsync(new DeviceModel[] { SelectedDevice });

			IsLoadingDeviceProperties = false;
			IsDevicePropertiesVisible = true;
		}

		void Devices_DeviceDataUpdated(object sender, DeviceDataUpdatedEventArgs e)
		{
			// find device in list and update
			var deviceToUpdate = Devices.FirstOrDefault(dev => dev.UserDeviceId.Equals((sender as DeviceModel).UserDeviceId));
			if (deviceToUpdate != null)
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					var props = await DeviceDriveManager.Current.Data.GetPropertiesAsync(deviceToUpdate);
					if (props != null)
					{
						if (props.Count() != SelectedDeviceProperties.Count())
						{
							SelectedDeviceProperties.Clear();
							foreach (var prop in props)
								SelectedDeviceProperties.Add(prop);
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
									MessagingCenter.Send(this, PropertyUpdatedMessage, prop);
								}
							}
						}
					}
				});
			}
		}

		#endregion

	}
}
