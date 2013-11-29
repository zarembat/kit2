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

namespace Insurance_company.ViewModels
{
    class AddClientViewModel : BaseViewModel
    {

        private ServiceReference.ClientSet _client;
        public ServiceReference.ClientSet Client
        {
            get { return _client; }
            set
            {
                if (value != _client)
                {
                    _client = value;
                    RaisePropertyChanged(() => Client); // Notify UI
                };
            }
        }

        private ServiceReference.AdressSet _address;
        public ServiceReference.AdressSet Address
        {
            get { return _address; }
            set
            {
                if (value != _address)
                {
                    _address = value;
                    RaisePropertyChanged(() => Address);
                };
            }
        }

        public ICommand SaveClientCommand { get { return new DelegateCommand(OnCustomerSave); } }

        public AddClientViewModel() {
            setEntities();
        }

        private void setEntities()
        {
            _client = new ServiceReference.ClientSet();
            _address = new ServiceReference.AdressSet();
        }

        private async Task saveClient()
        {
            //using (var db = new InsuranceCompanyEntities())
            //{
            //    try // Adding new client
            //    {
            //        Client.AdressSet = Address;
            //        db.ClientSet.Add(Client);
            //        await db.SaveChangesAsync(); // Saving changes to the database
            //    }
            //    catch (DbEntityValidationException e)
            //    {
            //        MessageBox.Show("Adding a new client caused an error: " + e.Message);
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show("Adding a new client caused an error: " + e.Message);
            //    }
            //}
        }

        private void OnCustomerSave(object parameter)
        {
            Task.Factory.StartNew(() => saveClient()).ContinueWith(t =>
            {
                MessageBox.Show("Client added successfully!");
                // Clearing the form:
                Client = new ServiceReference.ClientSet();
                Address = new ServiceReference.AdressSet();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
