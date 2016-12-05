# LightSwitch Demo App
This example shows how to build a complete app using the DeviceDrive SDK to manage DeviceDrive WRF devices. 

The example builds upon the Light Switch example used in our documentation and tutorials where you can follow the steps on how to build a light switch device using the DeviceDrive WRF unit.

The demo contains a Xamarin project for iOS and Android (can be opened in Visual Studio) showing you how to use the SDK to build an end-to-end solution in a mobile app.

## Getting Started
Start by going through the getting started videos and tutorials to get the basic knowledge about the DeviceDrive Cloud here:

[TODO](Link)

After downloading the source code you need to update your application id and secret to get the example to work.

### Android
Open the file ```MainActivity.cs``` in the Android project and add your application id and secret (look for the TODO comment)

### iOS
Open the file ```AppDelegate.cs``` in the iOS project and add your application id and secret (look for the TODO comment)

## Requirements 
If you want to use the example and SDK to build your own app in Xamarin, just install our SDK using the NuGet package ```DeviceDrive.SDK 0.1.31-alpha``` (remember to allow for prerelease packages) in your solution (remember to install in all projects).

In addition there are some settings that needs to be added to the iOS and Android project:

### iOS
info.plist: 

```
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>    
    <true/>
</dict>

```


### Android
Permissions:

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