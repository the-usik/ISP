using ISP.Database;
using ISP.Database.Models;
using ISP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISP.Pages.Dialogs {


    public sealed partial class AddServiceDialog : ContentDialog {
        public class ServiceViewModel {
            public ServiceModel Service { get; set; }
            public bool IsServiceValid => !string.IsNullOrEmpty(Service.Title) && Service.Price > 0;

            public string Title {
                get => Service.Title;
                set {
                    Service.Title = value.Trim();
                }
            }

            public string PriceText {
                get => Service.Price.ToString();
                set {
                    try {
                        Service.Price = Convert.ToDecimal(value);
                    } catch (Exception) {
                        Service.Price = 0;
                    }
                }
            }

            public ServiceViewModel() {
                Service = new ServiceModel();
            }
        }

        public ServiceViewModel ServiceVM { get; set; }

        public AddServiceDialog() {
            this.InitializeComponent();
            ServiceVM = new ServiceViewModel();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (!ServiceVM.IsServiceValid) {
                args.Cancel = true;
                Panic("Incorrect form data.");
                return;
            }

            var deferral = args.GetDeferral();
            try {
                bool result = await DatabaseAccess.AddService(ServiceVM.Service);
                if (!result) {
                    args.Cancel = true;
                    Panic("Internal database error");
                }
            } catch (Exception exception) {
                args.Cancel = true;
                Panic("Specified titile already exists!");
                Console.WriteLine(exception.Message);
            }

            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        private void Panic(string message) {
            Helpers.ShowTextBlockMessage(ErrorMessageBlock, message, null);
        }
    }
}
