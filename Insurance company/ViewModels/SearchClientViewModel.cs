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
                    RaisePropertyChanged(() => Clients);
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


        public ICommand SearchClientCommand { get { return new DelegateCommand(OnCustomerSearch); } }

        public SearchClientViewModel() {
            _client = new ClientSet();
            _address = new AdressSet();
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
                if (!client.Name.Equals(""))
                {
                    nameExpr = GetEqualsExpr(param, "name", client.Name);
                    if (cond == null)
                        cond = nameExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, nameExpr);
                }
            }
            {
                
            }

            if (client.Surname != null)
            {
                if (!client.Surname.Equals(""))
                {
                    surnameExpr = GetEqualsExpr(param, "surname", client.Surname);

                    if (cond == null)
                        cond = surnameExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, surnameExpr);
                }
            }

            if (client.PESEL != null)
            {
                if (!client.PESEL.Equals(""))
                {
                    peselExpr = GetEqualsExpr(param, "pesel", client.PESEL);
                    if (cond == null)
                        cond = peselExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, peselExpr);
                }
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
                if (!address.Town.Equals("")) {
                    townExpr = GetEqualsExpr(param, "town", address.Town);
                    cond = townExpr;
                }
                
            }

            if (address.Street != null)
            {
                if (!address.Street.Equals(""))
                {
                    streetExpr = GetEqualsExpr(param, "street", address.Street);

                    if (cond == null)
                        cond = streetExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, streetExpr);
                }
            }

           if (address.HouseNumber != null)
            {
                if (!address.HouseNumber.Equals(""))
                {

                    houseNumberExpr = GetEqualsExpr(param, "houseNumber", address.HouseNumber);
                    if (cond == null)
                        cond = houseNumberExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, houseNumberExpr);
                }
            }

            if (address.ZipCode != null)
            {
                if (!address.ZipCode.Equals(""))
                {
                    zipCodeExpr = GetEqualsExpr(param, "zipCode", address.ZipCode);
                    if (cond == null)
                        cond = zipCodeExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, zipCodeExpr);
                }
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

            Task.Factory.StartNew(() =>
            {
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
                            label = true;
                            foreach (ClientSet client in clients)
                            {
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

                }
            }).ContinueWith(t =>
            {

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
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
            
    }
}
