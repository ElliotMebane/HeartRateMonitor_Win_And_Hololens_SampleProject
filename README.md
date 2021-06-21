This is a sample project implementing the Bluetooth LE Heart Rate Monitor plugin in Unity for connecting Bluetooth LE Heart Rate Monitors and Windows HoloLens and Windows 10 PCs. The plugin is also available on the Unity Asset store at this link:   https://assetstore.unity.com/packages/tools/input-management/hololens-heart-rate-monitor-plugin-76113 
 
If you would like a version of the plugin with advanced features, such as reporting additional or values from specific HRM devices (such as RR values), please contact me at customersupport@roguish.com.  
  
-----------  
  
Device Compatibility  
  
The plugin looks for devices broadcasting the standard Bluetooth Heart Rate Measurement Characteristic. In field testing there have been confirmed failures to identify some HRM devices. The list of tested devices is below. If you have tested the plugin with another device, please let me know so I may add it to the list.  
  
Successful Devices  
-- Polar H7 (chest strap)  
-- Zephyr HxM Smart (chest strap)  
-- Scosche Rhythm 24 (wrist-worn)  
  
Failed Devices  
-- Zoom HRV wrist-worn (Fails to be seen by HoloLens, can be seen by Win 10 app)  
-- Polar OH1 (2 customers reported failure to see the OH1 in the list, but it can be seen by the app when built for Win 10 desktop).  
  
-----------  
  
2018/11/11  
Plugin Testing and Confirmation  
Successful Build and test on HoloLens HRM Plugin version: 1.3.4.0  
    
I will occasionally update my toolset and republish to HoloLens to confirm that everything is still working. This is the toolset used in my latest test:
   
Unity Editor Version: 2017.4.14f1  
(as seen in: ProjectSettings > ProjectVersion.txt)  
  
Mixed Reality Toolkit 2017.4.2.0 (as seen in: Assets\HoloToolkit\MRTKVersion.txt  
https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2017.4.2.0  
  
Windows SDK 10.0.17763.0 (as seen in: C:\Program Files (x86)\Windows Kits\10\SDKManifest.xml)  
  
Microsoft Visual Studio Community 2017 Version: 15.8.7  
.NET Version 4.7.03056  
(as seen in Visual Studio About)  
  
Windows Home 10 Version: 10.0.17134  
(as seen via Commandline: winver: Version 1803 (OS Build 17134.345)  
Greater than Fall Creator's Update version (1709)  
https://pureinfotech.com/check-windows-10-fall-creators-update-installed/  
  
HoloLens (Windows Insider Program):  
OS 10.0.17763.1000  

-----------  
  
There are several applications I recommend for testing your HRM device's BLE capabilities:  
- Android/iOS – nRF Connect from Nordic Semiconductor:  
  - https://play.google.com/store/apps/details?id=no.nordicsemi.android.mcp&hl=en  
  - https://itunes.apple.com/us/app/nrf-connect/id1054362403?mt=8  
- Microsoft Bluetooth LE Explorer for Windows 10 (and now available for HoloLens also):  
  - https://www.microsoft.com/en-us/store/p/bluetooth-le-explorer/9n0ztkf1qd98  
  
-----------  
  
Release Notes  
   
1.3.4.0 (2018/04)  
- Updated and tested for the latest Microsoft Mixed Reality Toolkit, Unity, Windows OS, HoloLens OS, etc.  
- Added Windows 10 examples.  
- Expanded HRM device compatibility. Now compatibility with Zoom HRV watch and probably other HRMs that previously may not have worked.  
- The Plugin now simply disregards the flags byte and returns the value of the second byte as the heart rate. This is valid for most devices, however the full Bluetooth spec for Heart Rate Service allows the heart rate to be returned as 2-bytes, allowing for much faster heart rates (mainly for animals). Unfortunately, the ZoomHRV returns a flag byte of 00, which is pretty much nonsense according to the BLE spec and was causing an attempt to read heart rate from 2-bytes, but there was only one to read from. Also, in HRV mode the Zoom HRV broadcasts a flag byte of 10, which matches a SCOSCHE armband I recentlly tried. The new plugin has been tested with the Zoom HRV and works in both modes (active workout and HRV mode) that use flags 00 and 10, so it should work with a SCOSCHE armband as well.   
- I also removed the application of a gatt protection level. In prior versions it was set to expect an encrypted communication channel. Removing that was necessary to connect with the Zoom HRM, so I suspect their broadcast is not using the BLE encryption setting. For users who want to use one of the protection levels I've enabled it with a property “gattProtectionLevel” specified below.  
- Updated to conform to new Name reporting of Windows BLE. DeviceInformation.Name used to return the displayable name of the device, but now it returns “Heart Rate”. The displayable name must be retrieved for each DeviceInformation object in the list.   
- Added Advertisement Listening for RSSI updating.  
- Restricted the byte array data returned with receivedMeasurementData to only the first 2 bytes of the data (the flags byte and the heart rate byte).  
- Please use receivedMeasurementDataLimited now. Users who want RR data please contact me for a pro version of the plugin. 
 
1.2.0.0  
- Archive and Expose the un-processed Byte Array data received from the HRM device in a List: receivedMeasurementData  
- Added customizable size limit to the hrms and  receivedMeasurementData Lists  
- Added VERSION string   

1.1.2.0 Original release  
- Plugin Overview
- The Heart Rate Monitor plugin for Unity enables apps built for HoloLens and Windows 10 devices to receive data from an external Bluetooth LE Heart Rate Monitor.  
