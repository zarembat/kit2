using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Insurance_company.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            if (login.Text.Length > 0 && PasswordInput.Password.Length > 0)
                button.IsEnabled = true;
        }

        private void passwordChanged(object sender, RoutedEventArgs e)
        {
            if (login.Text.Length > 0 && PasswordInput.Password.Length > 0)
                button.IsEnabled = true;
        }
    }
}
