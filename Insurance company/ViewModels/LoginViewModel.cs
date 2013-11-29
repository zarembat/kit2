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
            ServiceReference.EmployeeSet Employee = null;
            ServiceReference.InsuranceCompanyEntities dataServiceClient = new ServiceReference.InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
            //System.Data.Services.Client.DataServiceQuery<ServiceReference.EmployeeSet> employeeQuery = dataServiceClient.EmployeeSet.Where(e => e.Login == UserName && e.Password == LoginWindow.PasswordInput.Password).FirstOrDefault();
            //CollectionViewSource ordersViewSource;
            var LoginTask = Task.Factory.StartNew(() =>
            {
                // Employee = dataServiceClient.EmployeeSet.Where(e => e.Login == UserName && e.Password == LoginWindow.PasswordInput.Password).FirstOrDefault(); // Looking for an employee in the database

                var Emp = from emp in dataServiceClient.EmployeeSet
                           where emp.Login == UserName
                           select emp;
                if (Emp == null)
                    MessageBox.Show("Invalid username or password!");
                else
                {
                    new EmployeePanel().Show(); // We open Employee panel
                    LoginWindow.Close(); // We close login window
                }

            });
            LoginTask.Wait();
            if (Employee != null)
            {
                MessageBox.Show("Fajno");
                new EmployeePanel().Show(); // We open Employee panel
                LoginWindow.Close(); // We close login window
            }
        }

    }
}