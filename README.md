# HeartRateMonitor_Win_And_Hololens_SampleProject  
  
This is a working sample project implementing the Bluetooth LE Heart Rate Monitor plugin in Unity for connecting Windows HoloLens and Windows 10 PCs with Bluetooth LE Heart Rate Monitors. The plugin is available on the Unity Asset store and is also available here, integrated into this working project.   
  
If you would like a version that reports additional values from the HRM devices (such as RR values), please contact me at customersupport@roguish.com.  
  
Device Compatibility  
   
The plugin looks for devices broadcasting the standard Bluetooth Heart Rate Measurement Characteristic. In field testing there have been confirmed failures to identify some HRM devices. The list of tested devices is below. If you have tested the plugin with another device, please let me know so I may add it to the list. 

Successful Devices
Polar H7 chest strap
Zephyr HxM Smart chest strap 
Rhythm 24 wrist worn 

Failed Devices
Zoom HRV wrist worn (Fails to be seen by HoloLens, can be seen by Win 10 app)
Polar OH1 (2 customers reported failure to see the OH1 in the list, but it can be seen by the app when built for Win 10 desktop).
  
  
Plugin Testing and Confirmation
  
I will occasionally update my toolset and republish to HoloLens to confirm that everything is still working. Below is the latest toolset I tested with:
   
2018/11/11  
Successful Build and test on HoloLens
HRM Plugin version: 1.3.4.0

Unity Editor Version: 2017.4.14f1  
(as seen in: ProjectSettings > ProjectVersion.txt)
  
Mixed Reality Toolkit 2017.4.2.0 (as seen in: Assets\HoloToolkit\MRTKVersion.txt  
https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2017.4.2.0  
  
Windows SDK 10.0.17763.0 (as seen in: C:\Program Files (x86)\Windows Kits\10\SDKManifest.xml)   
PlatformIdentity = "UAP, Version=10.0.16299.0"   
  
Microsoft Visual Studio Community 2017 Version: 15.8.7  
.NET Version 4.7.03056  
(as seen in Visual Studio About)  
  
Windows Home 10 Version: 10.0.17134  
(as seen via Commandline: winver: Version 1803 (OS Build 17134.345)  
Greater than Fall Creator's Update version (1709)   
https://pureinfotech.com/check-windows-10-fall-creators-update-installed/  
  
HoloLens (Windows Insider Program):  
OS 10.0.17763.1000  
  
  
  
