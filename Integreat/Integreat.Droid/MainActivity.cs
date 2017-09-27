
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Autofac;
using Firebase;
using Integreat.Shared;
using Integreat.Shared.Services.Tracking;
using Integreat.Shared.Utilities;
using localization;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Debug = System.Diagnostics.Debug;

namespace Integreat.Droid
{

    [Activity(Label = "Integreat", Icon = "@mipmap/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // var app = FirebaseApp.InitializeApp(Android.App.Application.Context);


            base.OnCreate(bundle);
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Forms.Init(this, bundle);

#if DEBUG
            int logcatLines = 0;
            Exec(new[] {"/system/bin/logcat", "-d"},
                (o, e) =>
                {
                    Debug.WriteLine(e.Data);
                    if (logcatLines == 10)
                    {
                        var p = (System.Diagnostics.Process) o;
                        p.CancelErrorRead();
                        p.CancelOutputRead();
                        if (!p.HasExited)
                        {
                            try
                            {
                                p.Kill();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Unable to kill: {0}", ex.Message);
                            }
                        }
                    }
                },
                (o, e) =>
                {
                    Debug.WriteLine(e.Data);
                }
            );
#endif

            try
            {
                DisplayCrashReport();
                
            }
            catch (Exception)
            {
                // suppress all errors on crash reporting
            }
#if DEBUG
            FirebasePushNotificationManager.Initialize(Android.App.Application.Context, true);
#else
              FirebasePushNotificationManager.Initialize(Android.App.Application.Context,false);
#endif
            ContinueApplicationStartup();

            FirebasePushNotificationManager.ProcessIntent(Intent);
        }

        static int Exec(string[] command, EventHandler<DataReceivedEventArgs> stdout, EventHandler<DataReceivedEventArgs> stderr)
        {
            var psi = new ProcessStartInfo(command[0], '"' + string.Join("\" \"", command.Skip(1)) + '"')
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            var p = new System.Diagnostics.Process()
            {
                StartInfo = psi,
                EnableRaisingEvents = true,
            };
            if (stdout != null)
                p.OutputDataReceived += new DataReceivedEventHandler(stdout);
            if (stderr != null)
                p.ErrorDataReceived += new DataReceivedEventHandler(stderr);

            using (p)
            {
                p.Start();
                if (stdout != null)
                    p.BeginOutputReadLine();
                if (stderr != null)
                    p.BeginErrorReadLine();
                p.WaitForExit();
                return p.ExitCode;
            }
        }

        private void ContinueApplicationStartup()
        {

            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            var cb = new ContainerBuilder();
            cb.RegisterInstance(CreateAnalytics());
            LoadApplication(new IntegreatApp(cb));


        }


        private IAnalyticsService CreateAnalytics()
        {
            var instance = AnalyticsService.GetInstance();
            instance.Initialize(this);
            return instance;
        }



        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = String.Format("Time: {0}\r\n{1}\r\n{2}",
                DateTime.Now, AppResources.ErrorGeneral, exception);
                File.WriteAllText(errorFilePath, errorMessage);

                // Log to Android Device Logging.
                Android.Util.Log.Error(AppResources.CrashReport, errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is displayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        private bool DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var errorFilePath = Path.Combine(libraryPath, errorFilename);

            // check if there is an error file present
            if (!File.Exists(errorFilePath))
            {
                // if not, no error happened
                return false;
            }

            // an error occurred last time the app was running. Clear cache to fix eventual corrupt cache issues
            Cache.ClearCachedResources();
            Cache.ClearCachedContent();

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this)
                .SetPositiveButton(AppResources.Close, (sender, args) =>
                {
                    File.Delete(errorFilePath);
                    ContinueApplicationStartup();
                })
                .SetNegativeButton(AppResources.Copy, (sender, args) =>
                {
                    // try to copy contents of file to clipboard

                    try
                    {
                        var clipboardmanager = (ClipboardManager)Forms.Context.GetSystemService(ClipboardService);
                        clipboardmanager.PrimaryClip = ClipData.NewPlainText(AppResources.CrashReport, File.ReadAllText(errorFilePath));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    File.Delete(errorFilePath);
                    ContinueApplicationStartup();
                })
                .SetMessage(errorText)
                .SetTitle(AppResources.CrashReport)
                .Show();

            return true;
        }


        //iOS: Different than Android. Must be in FinishedLaunching, not in Main.
        // In AppDelegate
        /*public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary options)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

			TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;  
    ...
}*/

    }
}

