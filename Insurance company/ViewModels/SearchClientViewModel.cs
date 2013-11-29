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
using Insurance_company.Models;
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

using System.Data.Objects;

namespace Insurance_company.ViewModels
{
    class SearchClientViewModel : BaseViewModel
    {

        List<EntityParameter> ClientParameters = new List<EntityParameter>();
        List<EntityParameter> AddressParameters = new List<EntityParameter>();

        private ObservableCollection<ClientSet> _clients = new ObservableCollection<ClientSet>();

        public ObservableCollection<ClientSet> Clients
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
            _client = new ClientSet();
            _address = new AdressSet();
        }

        private void OnCustomerSearch(object parameter)
        {

            string addressQuery = CreateAddressSearchQuery();
            int addressId = -1;
            Task.Factory.StartNew(() =>
            {
                using (EntityConnection conn = new EntityConnection("name=InsuranceCompanyEntities"))
                {
                    conn.Open();

                    if (addressQuery != null) // Co najmniej jedno pole adresowe zostało uzupełnione
                    {

                        using (EntityCommand cmd = new EntityCommand(addressQuery, conn))
                        {

                            foreach (EntityParameter param in AddressParameters)
                            {
                                cmd.Parameters.Add(param);
                            }

                            AddressParameters = new List<EntityParameter>();

                            using (DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                            {
                                while (rdr.Read())
                                {
                                    addressId = (int)rdr["AdressId"];
                                    if (addressId > 0) // Znaleźliśmy adres spełniający te kryteria
                                        GetClients(addressId, conn); // Szukamy dla niego klientów
                                }
                            }
                        }

                    }
                    else // Nie uzupełniono danych adresowych, szukamy po prostu klientów
                    {
                        GetClients(-1, conn);
                    }

                    conn.Close();

                }}).ContinueWith(t => {

                if (_clients.Count > 0) // Znaleźliśmy klientów, przekazujemy dane i otwieramy okienko
                {
                    ClientsWindow cw = new ClientsWindow();
                    cw.DataContext = new ClientsViewModel(_clients);
                    cw.ShowDialog();
                    _clients = new ObservableCollection<ClientSet>(); // Zerujemy kolekcję w razie kolejnego wyszukiwania
                }
                else
                    MessageBox.Show("No clients matching these criteria were found!");
                }, TaskScheduler.FromCurrentSynchronizationContext());


        }

        private void GetClients(int addressId, EntityConnection c)
        {
            string esqlQuery = CreateClientSearchQuery(addressId);
            if (esqlQuery != null)
            {
                using (EntityCommand cmd = new EntityCommand(esqlQuery, c))
                {

                    foreach (EntityParameter param in ClientParameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    ClientParameters = new List<EntityParameter>();

                    using (DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (rdr.Read())
                        {
                            ClientSet client = new ClientSet();
                            client.ClientId = (int)rdr["ClientId"];
                            client.Surname = rdr["Surname"].ToString();
                            client.Name = rdr["Name"].ToString();
                            client.PESEL = rdr["PESEL"].ToString();
                            client.AdressAdressId = (int)rdr["AdressAdressId"];
                            _clients.Add(client);
                        }
                    }

                }
            }

        }
    }
}
