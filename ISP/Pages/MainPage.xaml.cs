using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ISP.Pages;
using ISP.Database;
using ISP.Pages.Dialogs;
using System;
using Windows.Storage;
using ISP.Utils;

namespace ISP {
    public sealed partial class MainPage : Page {

        public static MainPage Instance;

        public MainPage() {
            Instance = this;

            InitializeComponent();
            Init();

            ElementSoundPlayer.State = ElementSoundPlayerState.On;
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }

        public void OnLogin() {
            UpdateNavigation();
            ExecuteItemAction("Account");
        }

        private void Init() {
            UpdateNavigation();

            if (!AppSession.Authorized) ShowAuthorizationPage();
        }

        private void UpdateNavigation() {
            if (!AppSession.Authorized) {
                UsersNavigationItem.Visibility = Visibility.Collapsed;
                LogoutNavigationItem.Visibility = Visibility.Collapsed;
                return;
            }

            LogoutNavigationItem.Visibility = Visibility.Visible;
            UsersNavigationItem.Visibility = AppSession.CurrentUser.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            if (!AppSession.Authorized) {
                Helpers.ShowMessage("Authorization error", "Cannot to open this page, please login account");
                return;
            }

            if (args.IsSettingsSelected) {
                contentFrame.Navigate(typeof(SettingsPage));
                return;
            }

            var tag = GetSelectedPageTag(sender);
            ExecuteItemAction(tag);
        }

        private void ExecuteItemAction(string tag) {
            switch (tag) {
                case "Blog":
                    contentFrame.Navigate(typeof(BlogPage));
                    break;

                case "Rates":
                    contentFrame.Navigate(typeof(RatePage));
                    break;

                case "Services":
                    contentFrame.Navigate(typeof(ServicePage));
                    break;

                case "Account":
                    contentFrame.Navigate(typeof(AccountPage));
                    break;

                case "About":
                    contentFrame.Navigate(typeof(BlogPage));
                    break;

                case "Logout":
                    Logout();
                    break;
            }
        }

        private string GetSelectedPageTag() {
            return GetSelectedPageTag(NavigationView);
        }

        private string GetSelectedPageTag(NavigationView item) {
            if (item.SelectedItem == null) return null;

            var selectedItem = item.SelectedItem as NavigationViewItem;

            if (selectedItem.Tag == null) return null;

            return selectedItem.Tag.ToString();
        }

        private void Logout() {
            AppSession.Clear();
            UpdateNavigation();
            ShowAuthorizationPage();
        }

        private void ShowAuthorizationPage() {
            contentFrame.Navigate(typeof(AuthPage));
        }


    }
}
