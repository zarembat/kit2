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
using System.Data.Services.Client;

namespace Insurance_company.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        Login LoginWindow = null;

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

            LoginWindow = Parameter as Login; // We pass window object to get the password
            InsuranceCompanyEntities context = new InsuranceCompanyEntities(svcUri);

            DataServiceQuery<EmployeeSet> query = (DataServiceQuery<EmployeeSet>)(from employee in context.EmployeeSet
                                                                                  where employee.Login == UserName && employee.Password == LoginWindow.PasswordInput.Password
                                                                                  select employee);

            try
            {
                query.BeginExecute(OnEmployeeQueryComplete, query);
            }
            catch (DataServiceQueryException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OnEmployeeQueryComplete(IAsyncResult result)
        {
            DataServiceQuery<EmployeeSet> query = result.AsyncState as DataServiceQuery<EmployeeSet>;
            
            EmployeeSet employee = query.EndExecute(result).FirstOrDefault();
            if (employee != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    
                    EmployeePanel ep = new EmployeePanel(employee);
                    ep.Show(); // We open Employee panel
                    LoginWindow.Close(); // We close login window
                }));                
            }
            else
                MessageBox.Show("Invalid login or password");
        }    

    }
}