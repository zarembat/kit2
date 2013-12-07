using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Insurance_company.Helpers;
using Insurance_company.Views;
using System.Windows.Data;
using System.Data.Entity.Infrastructure;
using System.Collections;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Linq.Expressions;
using Insurance_company.ServiceReference;
using System.Data.Services.Client;
using System.Data.Objects;

namespace Insurance_company.ViewModels
{
    class SearchClientViewModel : BaseViewModel
    {
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        List<EntityParameter> ClientParameters = new List<EntityParameter>();
        List<EntityParameter> AddressParameters = new List<EntityParameter>();

        private ObservableCollection <ClientSet> _clients = new ObservableCollection <ClientSet>();

        public ObservableCollection <ClientSet> Clients
        {
            get { return _clients; }
            set
            {
                if (_clients != value)
                {
                    _clients = value;
                    RaisePropertyChanged(() => "Clients");
                }
            }

        }

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

        private ServiceReference.AdressSet _address;
        public ServiceReference.AdressSet Address
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

        private string CreateClientSearchQuery(int addressId)
        {

            string esqlQuery = null;

            if (addressId > 0 || Client.ClientId > 0 || (Client.Name != null && !Client.Name.Equals("")) || (Client.PESEL != null && !Client.PESEL.Equals("")) || (Client.Surname != null && !Client.Surname.Equals("")))
            {
                esqlQuery += @"SELECT VALUE Client FROM InsuranceCompanyEntities.ClientSet AS Client WHERE ";
                if (addressId > 0)
                {
                    Client.AdressAdressId = addressId;
                    EntityParameter param = new EntityParameter();
                    param.ParameterName = "AdressAdressId";
                    param.Value = addressId;
                    ClientParameters.Add(param);
                    esqlQuery += "Client.AdressAdressId = @AdressAdressId AND ";
                }
            }
            if (Client.Name != null && !Client.Name.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "Name";
                param.Value = Client.Name;
                ClientParameters.Add(param);
                esqlQuery += "Client.Name = @Name AND ";
            }
            if (Client.PESEL != null && !Client.PESEL.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "PESEL";
                param.Value = Client.PESEL;
                ClientParameters.Add(param);
                esqlQuery += "Client.PESEL = @PESEL AND ";
            }
            if (Client.Surname != null && !Client.Surname.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "Surname";
                param.Value = Client.Surname;
                ClientParameters.Add(param);
                esqlQuery += "Client.Surname = @Surname AND ";
            }

            if (esqlQuery != null)
                esqlQuery = esqlQuery.Remove(esqlQuery.Length - 5);
            return esqlQuery;

        }

        private string CreateAddressSearchQuery()
        {
            string esqlQuery = null;

            if ((Address.Town != null && !Address.Town.Equals("")) || (Address.HouseNumber != null && !Address.HouseNumber.Equals("")) || (Address.Street != null && !Address.Street.Equals("")) || (Address.ZipCode != null && !Address.ZipCode.Equals("")))
                esqlQuery += @"SELECT VALUE Address FROM InsuranceCompanyEntities.AdressSet AS Address WHERE ";

            if (Address.Town != null && !Address.Town.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "Town";
                param.Value = Address.Town;
                AddressParameters.Add(param);
                esqlQuery += "Address.Town = @Town AND ";
            }
            if (Address.Street != null && !Address.Street.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "Street";
                param.Value = Address.Street;
                AddressParameters.Add(param);
                esqlQuery += "Address.Street = @Street AND ";
            }
            if (Address.HouseNumber != null && !Address.HouseNumber.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "HouseNumber";
                param.Value = Address.HouseNumber;
                AddressParameters.Add(param);
                esqlQuery += "Address.HouseNumber = @HouseNumber AND ";
            }
            if (Address.ZipCode != null && !Address.ZipCode.Equals(""))
            {
                EntityParameter param = new EntityParameter();
                param.ParameterName = "ZipCode";
                param.Value = Address.ZipCode;
                AddressParameters.Add(param);
                esqlQuery += "Address.ZipCode = @ZipCode AND ";
            }

            if (esqlQuery != null)
                esqlQuery = esqlQuery.Remove(esqlQuery.Length - 5);
            return esqlQuery;
        }

        public ICommand SearchClientCommand { get { return new DelegateCommand(OnCustomerSearch); } }

        public SearchClientViewModel() {
            _client = new ServiceReference.ClientSet();
            _address = new ServiceReference.AdressSet();
        }

        public Expression<Func<ClientSet, bool>> GetWhereLambdaClient(ClientSet client, int addressId1)
        {
            ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(ClientSet), "c");

            System.Linq.Expressions.Expression surnameExpr;
            System.Linq.Expressions.Expression nameExpr;
            System.Linq.Expressions.Expression peselExpr;
            System.Linq.Expressions.Expression cond = null;

            if (addressId1 != -1) {

                System.Linq.Expressions.Expression prop = System.Linq.Expressions.Expression.Property(param, "AdressAdressId");
                System.Linq.Expressions.Expression val = System.Linq.Expressions.Expression.Constant(addressId1);
                cond = System.Linq.Expressions.Expression.Equal(prop, val);
            }
            
            if (client.Name != null)
            {
                nameExpr = GetEqualsExpr(param, "name", client.Name);
                if (cond == null)
                    cond = nameExpr;
                else
                    cond = System.Linq.Expressions.Expression.And(cond, nameExpr);
            }

            if (client.Surname != null)
            {
                surnameExpr = GetEqualsExpr(param, "surname", client.Surname);

                if (cond == null)
                    cond = surnameExpr;
                else
                    cond = System.Linq.Expressions.Expression.And(cond, surnameExpr);
            }

            if (client.PESEL != null)
            {
                peselExpr = GetEqualsExpr(param, "pesel", client.PESEL);
                if (cond == null)
                    cond = peselExpr;
                else
                    cond = System.Linq.Expressions.Expression.And(cond, peselExpr);
            }

            if (cond != null)
                return System.Linq.Expressions.Expression.Lambda<Func<ClientSet, bool>>(cond, param);

            return null;
        }

        public Expression<Func<AdressSet, bool>> GetWhereLambdaAddress(AdressSet address)
        {
            ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(AdressSet), "a");

            System.Linq.Expressions.Expression townExpr;
            System.Linq.Expressions.Expression streetExpr;
            System.Linq.Expressions.Expression houseNumberExpr;
            System.Linq.Expressions.Expression zipCodeExpr;
            System.Linq.Expressions.Expression cond = null;

            if (address.Town != null)
            {
                townExpr = GetEqualsExpr(param, "town", address.Town);
                cond = townExpr;
            }

            if (address.Street != null)
            {
                streetExpr = GetEqualsExpr(param, "street", address.Street);

                if (cond == null)
                    cond = streetExpr;
                else
                    cond = System.Linq.Expressions.Expression.And(cond, streetExpr);
            }

            if (address.HouseNumber != null)
            {
                houseNumberExpr = GetEqualsExpr(param, "houseNumber", address.HouseNumber);
                if (cond == null)
                    cond = houseNumberExpr;
                else
                    cond = System.Linq.Expressions.Expression.And(cond, houseNumberExpr);
            }

            if (address.ZipCode != null)
            {
                zipCodeExpr = GetEqualsExpr(param, "zipCode", address.ZipCode);
                if (cond == null)
                    cond = zipCodeExpr;
                else
                    cond = System.Linq.Expressions.Expression.And(cond,zipCodeExpr);
            }

            if (cond != null)
                return System.Linq.Expressions.Expression.Lambda<Func<AdressSet, bool>>(cond, param);

            return null;
        }

        private System.Linq.Expressions.Expression GetEqualsExpr(ParameterExpression param,
                                         string property,
                                         string value)
        {
            System.Linq.Expressions.Expression prop = System.Linq.Expressions.Expression.Property(param, property);
            System.Linq.Expressions.Expression val = System.Linq.Expressions.Expression.Constant(value);
            return System.Linq.Expressions.Expression.Equal(prop, val);
        }
        private void OnCustomerSearch(object parameter)
        {
            IEnumerable<AdressSet> addresses = null;
            IEnumerable<ClientSet> clients = null;
            Expression<Func<ClientSet, bool>> myLambda = null;
            Expression<Func<AdressSet, bool>> myLambdaAddress = null;
            bool label = false;
            

            myLambdaAddress = GetWhereLambdaAddress(Address);
            if (myLambdaAddress != null)
            {
                addresses = context.AdressSet.Where(myLambdaAddress);
                if (addresses.Count() == 0)
                    addresses = null;
            }
                

            if (addresses == null && myLambdaAddress != null)
                MessageBox.Show("No clients matching these address criteria were found!");

            else
            {

                if (addresses == null)
                {
                    myLambda = GetWhereLambdaClient(Client, -1);

                    if (myLambda != null)
                    {
                        clients = context.ClientSet.Where(myLambda);
                        foreach (ClientSet client in clients)
                        {
                            label = true;
                            _clients.Add(client);
                        }
                    }
                }
                    
                else
                {
                    foreach (AdressSet address in addresses)
                    {
                        myLambda = GetWhereLambdaClient(Client, address.AdressId);

                        if (myLambda != null)
                        {
                            label = true;
                            clients = context.ClientSet.Where(myLambda);

                            foreach (ClientSet client in clients)
                            {
                                _clients.Add(client);
                            }
                        }

                    }
                }

                if (!label)
                    MessageBox.Show("All fields are empty");

                else
                {


                    if (_clients.Count() == 0)
                        MessageBox.Show("No clients matching these criteria were found!");

                    else
                    {
                        ClientsWindow cw = new ClientsWindow();
                        cw.DataContext = new ClientsViewModel(_clients);
                        cw.ShowDialog();
                        _clients = new ObservableCollection<ClientSet>(); // Zerujemy kolekcję w razie kolejnego wyszukiwania
                    }
                }

            }
        }
            
    }
}
