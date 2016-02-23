The Google Cast SDK lets you extend your Android app to control a TV or sound system. It supports many media formats, protocols and codecs to ease integration.  Continuous playback is enabled by autoplay and queuing APIs and Game Manager APIs make Cast enabled gaming a breeze. 



Required Android API Levels
===========================

We recommend setting your app's *Target Framework* and *Target Android version* to **Android 5.0 (API Level 21)** or higher in your app project settings.

This Google Play Service SDK's requires a *Target Framework* of at least Android 4.2 (API Level 17) to compile.

You may still set a lower *Minimum Android version* (as low as Android 2.3 - API Level 9) so your app will run on older versions of Android, however you must ensure you do not use any API's at runtime that are not available on the version of Android your app is running on.


You must also ensure you have installed *Android 6.0 (API 23)*'s *SDK Platform* in the *Android SDK Manager* or you will have compiler errors.  You do not need to *use* this API level in your app, however *AAPT* requires it to be installed anyway.




Android Manifest 
================

Some Google Play Services APIs require specific metadata, attributes, permissions or features to be declared in your *AndroidManifest.xml* file.

These can be added manually to the *AndroidManifest.xml* file, or merged in through the use of assembly level attributes.


Since the Cast SDK is implemented using AppCompat V7, your Activities using this API must use a theme that derives from `Theme.AppCompat`.  You can update the theme per activity, or you can set it globally in your *AndroidManifest.xml* file by adding the `android:theme="@style/Theme.AppCompat"` attribute to your `<application>` element like this:

```xml
<application android:theme="@style/Theme.AppCompat">
```



Samples
=======

You can find a Sample Application within each Google Play Services component.  The sample will demonstrate the necessary configuration and some basic API usages.






Learn More
==========

You can learn more about the various Google Play Services SDKs & APIs by visiting the official [Google APIs for Android][3] documentation


You can learn more about Google Play Services Cast by visiting the official [Cast SDK for Android](https://developers.google.com/cast/docs/android_sender) documentation.



[1]: https://console.developers.google.com/ "Google Developers Console"
[2]: https://developer.xamarin.com/guides/android/deployment,_testing,_and_metrics/MD5_SHA1/ "Finding your SHA-1 Fingerprints"
[3]: https://developers.google.com/android/ "Google APIs for Android"

