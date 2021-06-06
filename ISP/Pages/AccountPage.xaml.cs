using ISP.Database.Models;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISP.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    class TestItem {
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public DateTime ActionDate { get; set; }
    }

    public sealed partial class AccountPage : Page {
        public UserModel User => AppSession.CurrentUser;

        public string FullName => $"{User.FirstName} ${User.LastName}";


        public AccountPage() {
            this.InitializeComponent();
            this.HistoryDataGrid.ItemsSource = new List<TestItem>() {
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now },
                new TestItem() { ActionName = "кончил", ActionDescription = "Данный пользователь кончил.", ActionDate = DateTime.Now }
            };
        }
    }
}
