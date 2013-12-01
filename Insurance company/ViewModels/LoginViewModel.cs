using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Insurance_company.Helpers;
using Insurance_company.Views;
using System.Windows.Data;
using Insurance_company.ServiceReference;

namespace Insurance_company.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private String _userName { get; set; }
        private String _userPassword { get; set; }

        public String UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        public LoginViewModel() {

        }

        public ICommand LoginCommand { get { return new DelegateCommand(OnLogin); } }

        private void OnLogin(object Parameter) {

            Login LoginWindow = Parameter as Login; // We pass window object to get the password
            EmployeeSet Employee = null;
            InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));

            var LoginTask = Task.Factory.StartNew(() =>
            {

              Employee = context.EmployeeSet.Where(e => e.Login == UserName && e.Password == LoginWindow.PasswordInput.Password).FirstOrDefault(); // Looking for an employee in the database

            });
            LoginTask.Wait();
            if (Employee != null)
            {
                new EmployeePanel().Show(); // We open Employee panel
                LoginWindow.Close(); // We close login window
            }
            else
                MessageBox.Show("Invalid login or password");
        }

    }
}