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

        public string AppId { get; set; }
        public string AccessToken { get; set; }
        public Login()
        {
            InitializeComponent();
            AppId = "266452880168662"; // Our APP ID

            this.Loaded += (object sender, RoutedEventArgs e) =>
            {
                // Add the message hook
                FacebookWebBrowser.MessageHook += FacebookMessageHook;

                // Delete the cookies since the last authentication (so that somebody else could log in)
                DeleteFacebookCookie();

                // Create the destination URL
                var destinationURL = String.Format("https://www.facebook.com/dialog/oauth?client_id={0}&display=popup&redirect_uri=http://www.facebook.com/connect/login_success.html&response_type=token",
                                                    AppId
                );
                FacebookWebBrowser.Navigate(destinationURL);
            };
        }

        private void OnNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // If authenticated:
            var url = e.Uri.Fragment;
            if (url.Contains("access_token") && url.Contains("#"))
            {
                url = (new System.Text.RegularExpressions.Regex("#")).Replace(url, "?", 1);
                AccessToken = System.Web.HttpUtility.ParseQueryString(url).Get("access_token");
                new EmployeePanel().Show();
                this.Close();
            }
        }

        private void DeleteFacebookCookie()
        {
            //Set the current user cookie to have expired yesterday
            string cookie = String.Format("c_user=; expires={0:R}; path=/; domain=.facebook.com", DateTime.UtcNow.AddDays(-1).ToString("R"));
            Application.SetCookie(new Uri("https://www.facebook.com"), cookie);
        }

        private void OnNavigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri.LocalPath == "/r.php")
            {
                MessageBox.Show("To create a new account go to www.facebook.com");
                e.Cancel = true;
            }
        }

        IntPtr FacebookMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // msg == 130 is the last call for when the window gets closed on a window.close() in javascript
            if (msg == 130)
            {
                this.Close();
            }
            return IntPtr.Zero;
        }
                
    }
}
