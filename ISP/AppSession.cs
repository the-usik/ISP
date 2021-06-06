using ISP.Database;
using ISP.Database.Models;
using System;
using Windows.Storage;

namespace ISP {
    public static class AppSession {
        public static UserModel CurrentUser;
        public static int CurrentUserId = 0;
        public static bool Authorized {
            get {
                return (CurrentUser != null) && CurrentUserId > 0;
            }
        }

        public static void Save() {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[ApplicationConstants.AUTH_SETTINGS_STTRING] = CurrentUserId;
        }

        public async static void Load() {
            try {
                var localSettings = ApplicationData.Current.LocalSettings;
                var data = localSettings.Values[ApplicationConstants.AUTH_SETTINGS_STTRING];

                if (data == null) return;
                CurrentUserId = (int) data;
                CurrentUser = await DatabaseAccess.FindUser(CurrentUserId);
            } catch (Exception exception) {
                Console.WriteLine(exception.Message);
            }
        }

        public static void Clear() {
            try {
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values.Clear();

                CurrentUserId = 0;
                CurrentUser = null;
                Save();
            } catch (Exception exception) {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
