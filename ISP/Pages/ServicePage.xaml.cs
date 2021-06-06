using ISP.Database.Models;
using ISP.Pages.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISP.Pages {
    public sealed partial class ServicePage : Page {
        public class ServicePageViewModel : INotifyPropertyChanged {
            private bool waiting = false;
            private bool loadError = false;
            private string resultMessage = "";

            public bool LoadError {
                get => loadError;
                set {
                    loadError = value;
                    OnPropertyChanged();
                }
            }

            public string ResultMessage {
                get => resultMessage;
                set {
                    resultMessage = value.Trim();
                    OnPropertyChanged();
                    OnPropertyChanged("ResultMessageVisibility");
                }
            }

            public bool Waiting {
                get => waiting;
                set {
                    waiting = value;
                    OnPropertyChanged("ProgressBarVisibility");
                }
            }

            public List<ServiceModel> Services { get; set; }

            public Visibility ResultMessageVisibility => !string.IsNullOrEmpty(ResultMessage)
                ? Visibility.Visible
                : Visibility.Collapsed;

            public Visibility ProgressBarVisibility => Waiting
                ? Visibility.Visible
                : Visibility.Collapsed;

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "") {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public ServicePageViewModel ViewModel { get; set; }

        public ServicePage() {
            InitializeComponent();
            ViewModel = new ServicePageViewModel();
            Init();

            AddButton.Click += AddButton_Click;
            SearchInput.TextChanged += SerachInput_TextChanged;
            DeleteButton.Click += DeleteButton_Click;
            BuyButton.Click += BuyButton_Click;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e) {
            var model = ServiceDataGrid.SelectedItem as ServiceModel;
            if (model == null) {
                return;
            }

            ViewModel.Waiting = true;

            var confirmationDialog = new MessageDialog($"Delete a \"{model.Title}\" service?", "Delete confirmation");

            confirmationDialog.Commands.Add(
                new UICommand("Ok", new UICommandInvokedHandler((command) => DeleteService(model.Id)))
            );

            confirmationDialog.Commands.Add(
                new UICommand("Cancel", new UICommandInvokedHandler((command) => { }))
            );

            confirmationDialog.DefaultCommandIndex = 0;
            confirmationDialog.CancelCommandIndex = 1;

            await confirmationDialog.ShowAsync();

            ViewModel.Waiting = false;
        }
        private void SerachInput_TextChanged(object sender, TextChangedEventArgs e) {
            ServiceDataGrid.ItemsSource = ViewModel.Services.FindAll(match => {
                string data = SearchInput.Text.Trim().ToLower();
                return match.Title.ToLower().Contains(data);
            });
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e) {
            ViewModel.Waiting = true;
            var dialog = new AddServiceDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                Update();
            } else ViewModel.Waiting = false;
        }


        private void BuyButton_Click(object sender, RoutedEventArgs e) {
            
        }

        private void Init() {
            Update();
        }

        private async void Update() {
            ViewModel.Waiting = true;
            try {
                await UpdateDataGrid();
                ViewModel.Waiting = false;

                if (ViewModel.Services.Count < 1) {
                    ViewModel.ResultMessage = "Services not found.";
                    return;
                }
            } catch (Exception exception) {
                ViewModel.LoadError = true;
                Console.WriteLine(exception.Message);
            }
        }

        private async void DeleteService(int id) {
            try {
                bool result = await Database.DatabaseAccess.DeleteService(id);
                if (!result) {
                    ViewModel.LoadError = true;
                    return;
                }

                Update();
            } catch (Exception exception) {
                ViewModel.LoadError = true;
                Console.WriteLine(exception.Message);
            }
        }

        private async Task<List<ServiceModel>> UpdateDataGrid() {
            var services = await Database.DatabaseAccess.FetchServices();
            ServiceDataGrid.ItemsSource = services;
            ViewModel.Services = services;

            return services;
        }
    }
}
