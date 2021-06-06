using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISP.Utils {
    static class Helpers {
        public static void ShowError(string message) {
            ShowMessage("Error", message);
        }

        public static async void ShowMessage(string title, string content) {
            var dialog = new MessageDialog(title, content);
            await dialog.ShowAsync();
        }

        public static void ShowTextBlockMessage(TextBlock element, string message, TimeSpan? span) {
            element.Text = message;
            element.Visibility = Visibility.Visible;
            
            if (span == null) {
                span = new TimeSpan(0, 0, 3);
            }

            var timer = new DispatcherTimer() {
                Interval = (TimeSpan) span
            };

            timer.Tick += (object sender, object e) => {
                element.Visibility = Visibility.Collapsed;
                timer.Stop();
            };

            timer.Start();
        }
    }
}
