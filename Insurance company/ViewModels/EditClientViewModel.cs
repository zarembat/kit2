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


namespace Insurance_company.ViewModels
{
    class EditClientViewModel : BaseViewModel
    {

        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        private ClientSet _client;
        public ClientSet Client
        {
            get { return _client; }
            set
            {
                if (value != _client)
                {
                    _client = value;
                    RaisePropertyChanged(() => "_client");
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
                    RaisePropertyChanged(() => "_address");
                };
            }
        }

        public ICommand SaveClientCommand { get { return new DelegateCommand(OnCustomerSave); } }

        public EditClientViewModel()
        {
        }

        public EditClientViewModel(ClientSet client) // This is called from ClientsViewModel when an item is clicked twice
        {
            _client = client;
            var address = context.AdressSet.Where(a => a.AdressId == client.AdressAdressId).FirstOrDefault(); // Looking for an Address
            if (address != null)
                _address = address;
            else
                MessageBox.Show("The address was not found for this client (ID: " + client.AdressAdressId + "!");
            
        }

        private void OnCustomerSave(object parameter)
        {
            Task.Factory.StartNew(() =>
            {

                try
                {
                    //var client = context.ClientSet.Find(Client.ClientId); // Looking for an old entry in the database
                    //var address = db.AdressSet.Find(Address.AdressId);
                    //db.Entry(client).CurrentValues.SetValues(Client); // Updating values
                    //db.Entry(address).CurrentValues.SetValues(Address);
                    //db.SaveChanges(); // Saving changes

                    var clientToChange = (from client in context.ClientSet
                                            where client.ClientId == Client.ClientId
                                            select client).Single();
                    var addressToChange = (from address in context.AdressSet
                                          where address.AdressId == Address.AdressId
                                          select address).Single();
                    

                }
                catch (DbEntityValidationException e)
                {
                    MessageBox.Show(e.Message);
                }

            }).ContinueWith(t =>
            {
                MessageBox.Show("Client edited successfully!");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }


    }
}
