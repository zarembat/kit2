using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_company.Helpers;
using Insurance_company.Views;
using System.Windows;
using System.Data.Entity.Validation;
using Insurance_company.ServiceReference;
using System.Data.Services.Client;
using System.Text.RegularExpressions;

namespace Insurance_company.ViewModels
{
    class EditClientViewModel : BaseViewModel
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
                    RaisePropertyChanged(() => Client);
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

        public EditClientViewModel()
        {
        }

        public EditClientViewModel(ClientSet client) // This is called from ClientsViewModel when an item is clicked twice
        {
            
            Client = client;
            try {
                DataServiceQuery<AdressSet> query = (DataServiceQuery<AdressSet>)(from address in context.AdressSet
                                                                                  where address.AdressId == _client.AdressAdressId
                                                                                  select address);            
                query.BeginExecute(OnAddressQueryComplete, query);
            }
            catch (DataServiceQueryException e)
            {
                throw new ApplicationException(
                    "An error occurred during query execution.", e);
            }
            catch (Exception e)
            {
                throw new ApplicationException(
                    "An error occurred during query execution.", e);
            }
            
        }
        private void OnAddressQueryComplete(IAsyncResult result)
        {
            DataServiceQuery<AdressSet> query = result.AsyncState as DataServiceQuery<AdressSet>;

            AdressSet address = query.EndExecute(result).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(new Action(() => {
            if (address != null)            
                    Address = address;
            else
                MessageBox.Show("No address for client with ID: " + _client.ClientId + " could be found!");
            }));
            
        }

        private void OnCustomerSave(object parameter)
        {
            if (!Validation())
                return;

            try
            {
                context.AttachTo("ClientSet", Client);
                context.UpdateObject(Client);
                context.UpdateObject(Address);            
                context.BeginSaveChanges(OnSaveChangesCompleted, null);
            }
            catch (DataServiceClientException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void OnSaveChangesCompleted(IAsyncResult result)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    context.EndSaveChanges(result);
                    MessageBox.Show("Client edited successfully!");
                }
                catch (DataServiceRequestException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }));
        }

        public bool Validation()
        {
            //Client
            if (string.IsNullOrEmpty(Client.Surname))
            {
                MessageBox.Show("Surname is required!");
                return false;
            }

            if (!Regex.IsMatch(Client.Surname, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                MessageBox.Show("Surname: Only letters!");
                return false;
            }

            if (string.IsNullOrEmpty(Client.Name))
            {
                MessageBox.Show("Name is required!");
                return false;
            }

            if (!Regex.IsMatch(Client.Name, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                MessageBox.Show("Name: Only letters!");
                return false;
            }

            if (string.IsNullOrEmpty(Client.PESEL))
            {
                MessageBox.Show("PESEL is required!");
                return false;
            }

            if (!Regex.IsMatch(Client.PESEL, @"^[0-9]{11}$"))
            {
                MessageBox.Show("PESEL: 11 digits!");
                return false;
            }

            //Address
            if (string.IsNullOrEmpty(Address.Town))
            {
                MessageBox.Show("Town is required!");
                return false;
            }

            if (!Regex.IsMatch(Address.Town, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                MessageBox.Show("Town: Only letters!");
                return false;
            }

            if (string.IsNullOrEmpty(Address.Street))
            {
                MessageBox.Show("Street is required!");
                return false;
            }

            if (!Regex.IsMatch(Address.Street, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ0-9]+$"))
            {
                MessageBox.Show("Street: Only letters!");
                return false;
            }

            if (string.IsNullOrEmpty(Address.HouseNumber))
            {
                MessageBox.Show("House number is required!");
                return false;
            }

            char[] letters = Address.HouseNumber.ToCharArray();
            if (!(Char.IsDigit(letters[0])))
            {
                MessageBox.Show("House number: Wrong format! e.g. 12a/12/10");
                return false;
            }

            bool isBackSlash = false;
            for (int i = 1; i < letters.Length; i++)
            {
                if (!isBackSlash)
                {
                    if (!(Char.IsDigit(letters[i - 1])))
                    {
                        if (!(letters[i].Equals('/')))
                        {
                            MessageBox.Show("House number: Wrong format! e.g. 12a/12");
                            break;
                        }
                    }

                    if (letters[i].Equals('/'))
                        isBackSlash = true;
                }

                else
                {
                    if (!(Char.IsDigit(letters[i])))
                    {
                        MessageBox.Show("House number: Wrong format! e.g. 12a/12");
                        break;
                    }
                }
            }

            if (letters[letters.Length - 1].Equals('/'))
            {
                MessageBox.Show("House number: Wrong format! e.g. 12a/12");
                return false;
            }

            if (string.IsNullOrEmpty(Address.ZipCode))
            {
                MessageBox.Show("Zip code is required!");
                return false;
            }

            if (!Regex.IsMatch(Address.ZipCode, @"^[0-9]{2}\-[0-9]{3}$"))
            {
                MessageBox.Show("Zip code: Wrong format e.g. 05-420!");
                return false;
            }

            return true;
        }
    }
}
