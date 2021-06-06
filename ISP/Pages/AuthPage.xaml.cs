using ISP.Database.Models;
using ISP.Exceptions;
using ISP.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ISP.Pages {
    public sealed partial class AuthPage : Page {
        public interface IValidateViewModel {
            bool Validate();
        }

        public class SignUpViewModel : UserModel, IValidateViewModel {
            
            public DateTimeOffset BirthDateOffset {
                get {
                    return new DateTimeOffset();
                }

                set {
                    Bdate = value.DateTime;
                }
            }

            public bool Validate() {
                bool invalid =
                    string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) ||
                    string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password) ||
                    Password.Length < 8 || string.IsNullOrEmpty(Address) ||
                    Bdate == null;

                return !invalid;
            }
        }
        public class LogInViewModel : IValidateViewModel {
            public string Login { get; set; }
            public string Password { get; set; }

            public bool Validate() {
                bool invalid = string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password);
                return !invalid;
            }
        }

        public LogInViewModel LogInModel { get; set; }
        public SignUpViewModel SignUpModel { get; set; }

        public AuthPage() {
            this.InitializeComponent();
            this.Init();

            this.SignUpModel = new SignUpViewModel();
            this.LogInModel = new LogInViewModel();

            this.AuthButton.Click += AuthButton_Click;
            this.SignUpButton.Click += SignUpButton_Click;
            this.CancelButton.Click += CancelButton_Click;
        }

        private void Init() {
            if (!AppSession.Authorized) return;
            MainPage.Instance.OnLogin();
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e) {
            if (!LogInModel.Validate()) {
                ShowLogInMessageError("Auth form data invalid.");
                return;
            }

            try {
                UserModel user = await Database.DatabaseAccess.Login(LogInModel.Login, LogInModel.Password);

                AppSession.CurrentUser = user;
                AppSession.CurrentUserId = user.Id;
                AppSession.Save();

                MainPage.Instance.OnLogin();
            } catch (AuthorizationException exception) {
                ShowLogInMessageError(exception.Message);
            } catch (Exception exception) {
                ShowLogInMessageError("Internal error.");
                Console.WriteLine(exception);
            }
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e) {
            if (!SignUpModel.Validate()) {
                ShowSignUpMessageError("Invalid form data.");
                return;
            }

            try {
                int userId = await Database.DatabaseAccess.Register(SignUpModel);

                AppSession.CurrentUser = await Database.DatabaseAccess.FindUser(userId);
                AppSession.CurrentUserId = userId;
                AppSession.Save();

                MainPage.Instance.OnLogin();
            } catch (Exception exception) {
                ShowSignUpMessageError("User with specified login already exists.");
                Console.WriteLine(exception);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Exit();
        }

        private void ShowLogInMessageError(string message) {
            Helpers.ShowTextBlockMessage(AuthErrorTextBlock, message, null);
        }

        private void ShowSignUpMessageError(string message) {
            Helpers.ShowTextBlockMessage(SignUpErrorTextBlock, message, null);
        }
    }
}
