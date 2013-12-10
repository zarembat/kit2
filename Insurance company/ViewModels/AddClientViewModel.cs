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
using Insurance_company.Helpers.Database;

namespace Insurance_company.ViewModels
{
    class AddClientViewModel : BaseViewModel, IDataErrorInfo
    {
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(svcUri);
        private ClientSet _client;
        public ClientSet Client
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

        private AdressSet _address;
        public AdressSet Address
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
            _client = new ClientSet();
            _address = new AdressSet();
        }

        private void OnCustomerSave(object parameter)
        {
            try
            {
                context.AddToAdressSet(Address);
                context.AddRelatedObject(Address, "ClientSet", Client);
                Client.AdressSet = Address;
                Address.ClientSet.Add(Client);
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

            try
            {
                context.EndSaveChanges(result);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show("Client added successfully!");
                    // Clearing the form:
                    Client = new ClientSet();
                    Address = new AdressSet();
                }));
            }
            catch (DataServiceRequestException ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "Surname":
                        {
                            if (string.IsNullOrEmpty(Client.Surname))
                            {
                                result = "Surname is required!";
                                break;
                            }

                            if (!Regex.IsMatch(Client.Surname, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
                                result = "Only letters!";

                            break;
                        }
                    case "Name":
                        {
                            if (string.IsNullOrEmpty(Client.Name))
                            {
                                result = "Name is required!";
                                break;
                            }

                            if (!Regex.IsMatch(Client.Name, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
                                result = "Only letters!";

                            break;
                        }
                    case "PESEL":
                        {
                            if (string.IsNullOrEmpty(Client.PESEL))
                            {
                                result = "PESEL is required!";
                                break;
                            }

                            if (!Regex.IsMatch(Client.PESEL, @"^[0-9]{11}$"))
                                result = "11 digits!";

                            break;
                        }
                };
                return result;
            }
        }


    }
}
