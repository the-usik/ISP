using ISP.Database;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ISP.Pages.Dialogs {
    public sealed partial class AddRateDialog : ContentDialog {
        public bool Result = false;

        public AddRateDialog() {
            InitializeComponent();
        }

        private async void ContentDialog_AddButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            ContentDialogButtonClickDeferral deferral = args.GetDeferral();
            try {
                bool invalid = string.IsNullOrEmpty(TitleInput.Text) && string.IsNullOrEmpty(FeeInput.Text) && string.IsNullOrEmpty(SpeedInput.Text);
                if (invalid) {
                    args.Cancel = true;
                    ShowErrorMessage("Form data is incorrect.");

                    return;
                }

                await SendData();
                deferral.Complete();

            } catch (Exception) {
                args.Cancel = true;
                deferral.Complete();
                ShowErrorMessage("Form data type is incorrect.");
            }
        }

        private void ContentDialog_CancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }


        private async Task SendData() {
            string title = TitleInput.Text;
            decimal fee = Convert.ToDecimal(FeeInput.Text);
            int speed = Convert.ToInt32(SpeedInput.Text);

            await DatabaseAccess.AddRate(title, fee, speed);
            Result = true;
        }


        private void ShowErrorMessage(string message) {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
