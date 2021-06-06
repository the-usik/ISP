using ISP.Database;
using ISP.Database.Models;
using ISP.Pages.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ISP.Pages {
    public sealed partial class RatePage : Page {
        private List<RateModel> rateList;

        public RatePage() {
            InitializeComponent();
            UpdateRatesDataGrid();
            AddButton.Click += OnAddButtonClick;
            DeleteButton.Click += OnDeleteButtonClick;
            SearchInput.TextChanged += OnSearchInputChanged;
        }

        private async void OnDeleteButtonClick(object sender, RoutedEventArgs e) {
            if (!(RateDataGrid.SelectedItem is RateModel model)) {
                return;
            }

            var confirmationDialog = new MessageDialog($"Delete a \"{model.Title}\" rate?", "Delete confirmation");

            confirmationDialog.Commands.Add(
                new UICommand("Ok", new UICommandInvokedHandler((command) => DeleteRate(model.Id)))
            );

            confirmationDialog.Commands.Add(
                new UICommand("Cancel", new UICommandInvokedHandler((command) => { }))
            );

            confirmationDialog.DefaultCommandIndex = 0;
            confirmationDialog.CancelCommandIndex = 1;

            await confirmationDialog.ShowAsync();
        }

        private async void OnAddButtonClick(object sender, RoutedEventArgs args) {
            var dialog = new AddRateDialog();
            dialog.Closed += (ContentDialog contentDialog, ContentDialogClosedEventArgs contentDialogClosedEventArgs) => {
                if (dialog.Result) UpdateRatesDataGrid();
            };
            await dialog.ShowAsync();
        }

        private void OnSearchInputChanged(object sender, TextChangedEventArgs args) {
            RateDataGrid.ItemsSource = rateList.FindAll(match => {
                string data = SearchInput.Text.Trim().ToLower();
                return match.Title.ToLower().Contains(data);
            });
        }

        private async void DeleteRate(int id) {
            try {
                await DatabaseAccess.DeleteRate(id);
                UpdateRatesDataGrid();
            } catch (Exception exception) {
                await new MessageDialog("Error", exception.Message).ShowAsync();
            }
        }

        private async void UpdateRatesDataGrid() {
            rateList = await DatabaseAccess.FetchRates();
            RateDataGrid.ItemsSource = rateList;
        }
    }
}
