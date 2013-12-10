using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_company.Helpers;
using System.Windows;
using System.Data.Entity.Validation;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Insurance_company.ServiceReference;
using System.Data.Services.Client;

namespace Insurance_company.ViewModels
{
    class AddUserViewModel : BaseViewModel
    {
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(svcUri);
        private EmployeeSet _employee;
        public EmployeeSet Employee
        {
            get { return _employee; }
            set
            {
                if (value != _employee)
                {
                    _employee = value;
                    RaisePropertyChanged(() => Employee); // Notify UI
                };
            }
        }

        
        public ICommand SaveUserCommand { get { return new DelegateCommand(OnCustomerSave); } }

        public AddUserViewModel() {
            setEntities();
        }

        private void setEntities()
        {
            _employee = new EmployeeSet();
        }

        private void OnCustomerSave(object parameter)
        {

            try {
                context.AddToEmployeeSet(Employee);
                context.BeginSaveChanges(OnSaveChangesCompleted, null);
            }
            catch (DataServiceClientException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void OnSaveChangesCompleted(IAsyncResult result)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    context.EndSaveChanges(result);
                    MessageBox.Show("User added successfully!");
                    // Clearing the form:
                    Employee = new EmployeeSet();
                }
                catch (DataServiceRequestException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }));
        }
    }
}
