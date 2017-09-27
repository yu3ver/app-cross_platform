using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;

namespace localization
{
    /// <summary>
    /// Wrapper class for the CrossFirebaseImplementation so it can be used in the Shared project (because the shared project itself has no reference to the CrossFirebase package)
    /// </summary>
    public static class Firebase
    {
        public static IFirebasePushNotification Current => CrossFirebasePushNotification.Current;
    }
}
