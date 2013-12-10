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
    }
}
