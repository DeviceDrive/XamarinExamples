# DeviceDrive SDK

## Setting up
This documentation is written based on a Xamarin.Forms app for iOS and Android. 

- Add the NuGet package DeviceDrive.SDK (remember to allow prelreases in the package manager).

### iOS Setup
To be able to communicate with a device using Wifi and SSL on iOS, you need to add the following settings to your Info.plist file in the iOS project (this has already been added in the demo project):

```
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>    
    <true/>
</dict>
```

### Android Setup
Android needs the following permissions in the AndroidManifest.xml file (already been added in the demo project):

```
<uses-permission android:name="android.permission.CHANGE_WIFI_STATE" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.BATTERY_STATS" />
<uses-permission android:name="android.permission.BIND_NOTIFICATION_LISTENER_SERVICE" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_PHONE_STATE" />
<uses-permission android:name="android.permission.READ_LOGS" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
```

## Setting Up

**iOS:**

```csharp
LoadApplication(new App(new DeviceDriveTouchPlatform()));
```

**Android:**

In the `MainActivity.cs` file:

```csharp
LoadApplication(new App(new DeviceDriveDroidPlatform(this)));
``` 

Where `this` is the current activity.

## Initialization

> Previously initialization was done in the `MainActivity` or `AppDelegate` but shold now be done in the main page of the application to make the application more snappy and faster on startup.


Initialize the SDK from within the `OnAppearing` method in the main page of the application:

```csharp
await DeviceDriveManager.Current.InitializeAsync(
	"APPLICATION_ID", "APPLICATION_SECRET",
    platform);
```

Where `platform` is the iOS or Android platform implementation - this variable should be injected when creating the main page:

The method is async and can be run in a thread:

```csharp
Task.Run(async ()=> { .... initialization ... });
```

To get a notification when the app is initialized you should listen to the `DeviceDriveManager.Current.Initialized` event which will be raised once the initialization of the SDK is done. This is where you can start subscribing to events, load devices and update the user interface.

If you want to use the SDK from within the app you should check if the SDK is initialized by testing the `IsInitialized` property:

```csharp
if(DeviceDriveManager.Current.IsInitialized){
	... call SDK methods ...
}
```

## Application Lifecycle
To enable automatic handling of the application lifecycle, the SDK should be notified whenever the app changes its state and goes from an active state to a suspended state:

This example shows how to subscribe using the Xamarin Messaging Centre:

```csharp
MessagingCenter.Subscribe<App>(this, "AppOnResumeMessage", async (obj) => 
	await DeviceDriveManager.Current.OnResumedAsync()); 

MessagingCenter.Subscribe<App>(this, "AppOnSleepMessage", async (obj) => 
	await DeviceDriveManager.Current.OnPausedAsync());
```

You main application class needs to publish these messages as well (again using Xamarin Forms):

```csharp
protected override void OnSleep()
{
	// Handle when your app sleeps
	MessagingCenter.Send(this, "AppOnSleepMessage");
}

protected override void OnResume()
{
	// Handle when your app resumes
	MessagingCenter.Send(this, "AppOnResumeMessage");
}
```

## SDK Events
The event exposes a set of events that are important to use to get notified when changes occures. The SDK ensures that all events are called on the main thread and it should therefore be possible to update the UI from within all event handlers.

### StateChanged event
To get a notification whenever the SDK starts or stops an operation from the backround you can listen to the `StateChanged` event. This event contains information about the old and the new state of the SDK and can be used to display status messages to the user.

### AuthenticatedChanged event
To get notified whenever the curretn authentication state changes, subscribe to the `AuthenticatedChanged` event. The event contains information about wether the SDK is authenticated or not.

### NotificationStatusChanged event
The SDK will try to set up an automatic real-time update channel using SignalR to get notifications when a device's status has changed. To be notified about notification you should subscribe to the `NotificationStatusChanged` event. The event provides information about the current status of the real-time notifications.

### DevicesUpdated event
When loading devices the SDK will return a list of cached devices before querying the cloud for more devices. You should subscribe to the `DevicesUpdated` event to get information about changes in the list of available devices and handle this by updating the UX. The event contains a list of devices which should be considered the new list of devices. The list may contain new devices or updated devices, and might also indicate that a previously cached device has been deleted (by not including it in the list in the event). The app should update its user interface when this event occurs.

### DeviceUpdated event
When a device changes its data properties the event `DeviceUpdated` will be called. The event provides you with information about the device that was changed and the data property that was changed. The app should update its user interface when this event occurs.

### DeviceDeleted event
Whenever a device is deleted the SDK will call the `DeviceDeleted` event. The app should update its user interface when this event occurs.

## Startup Flow
A suggested startup flow for an app using the SDK would be:

1. Initialize the SDK from within the Main Page's `OnAppearing` method.
2. When the `Initialized` event is called from the SDK the app should query for authentication status. If the SDK is authenticated the app should load cached devices with the `DeviceDriveManager.Current.GetDevicesAsync()` method and respond to the events described in the events section of this document.

### Authentication
The app can authenticate by calling the `AuthenticateAsync` method of the `Authentication` object in the SDK:

```csharp
var authenticationResult = await DeviceDriveManager.Current.Authentication.AuthenticateAsync(
	AuthenticationPolicy.SignIn);
```

The `authenticationResult` variable contains information about the currently logged in user in addition to the current authentication token from the cloud service. The `AuthenticateAsync` method takes a policy parameter which can be used to call different authentication methods like editing the profile for the current user.

#### Logging out
Logging out is done by calling the `LogoutAsync` method:

```csharp
await DeviceDriveManager.Current.Authentication.LogoutAsync();
```

A notification will be provided through the `AuthenticatedChanged` event.
