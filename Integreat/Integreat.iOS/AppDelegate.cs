using System;
using System.Globalization;
using Foundation;
using Integreat.Shared;
using UIKit;
using Autofac;
using Integreat.Shared.Services.Tracking;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;

namespace Integreat.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            var backgroundColor = Color.FromRgb(249,249,249);
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(nfloat.Parse(backgroundColor.R.ToString(CultureInfo.InvariantCulture)), nfloat.Parse(backgroundColor.G.ToString(CultureInfo.InvariantCulture)), nfloat.Parse(backgroundColor.B.ToString(CultureInfo.InvariantCulture)));
            var cb = new ContainerBuilder();
            cb.RegisterInstance(CreateAnalytics());
            LoadApplication(new IntegreatApp(cb));

            FirebasePushNotificationManager.Initialize(options, true);

            return base.FinishedLaunching(app, options);
        }

        private IAnalyticsService CreateAnalytics()
        {
            var instance = AnalyticsService.GetInstance();
            instance.Initialize();
            return instance;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
#if DEBUG
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken, FirebaseTokenType.Sandbox);
#endif
#if RELEASE
                    FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken,FirebaseTokenType.Production);
#endif

        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }
        // To receive notifications in foreground on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            FirebasePushNotificationManager.Connect();
            base.OnActivated(uiApplication);

        }
        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background execution this method is called instead of WillTerminate when the user quits.
            FirebasePushNotificationManager.Disconnect();
        }

    }
}
