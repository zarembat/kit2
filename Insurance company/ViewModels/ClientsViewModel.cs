using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_company.Helpers;
using Insurance_company.Views;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.Entity;
using Insurance_company.ServiceReference;
using System.Data.Services.Client;

namespace Insurance_company.ViewModels
{

    class ClientsViewModel : BaseViewModel
    {

        private ObservableCollection <ClientSet> _clients;
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));

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

        public ICommand ClientsGridLeftDoubleClickCommand { get { return new DelegateCommand(OnClientsGridLeftDoubleClick); } }

        private void OnClientsGridLeftDoubleClick(object parameter) // Clicking twice on a DataGrid item opens Edit Window
        {
            if (!(parameter is ClientSet))
                return;
            EditClient EditClientWindow = new EditClient(); // Creating Edit Window object
            EditClientWindow.DataContext = new EditClientViewModel(parameter as ClientSet); // Passing client object to the Edit Window
            EditClientWindow.ShowDialog();
        }

        public void refresh(IQueryable<ClientSet> clients) // Refreshing the list of clients in the DataGrid
        {
            // Clients = new ObservableCollection <ClientSet>(clients);
            ObservableCollection <ClientSet> clientss = new ObservableCollection <ClientSet>(clients);
            for (int i = Clients.Count; i < clients.Count(); i++)
            {
                Clients.Add(clientss[i]);
            }
        }

        public ClientsViewModel()
        {
            var GetClientsTask = Task.Factory.StartNew(() =>
            {
                //using (var db = new InsuranceCompanyEntities())
                //{
                //    _clients = new ObservableCollection <ClientSet>(db.ClientSet);
                //}
                _clients = new ObservableCollection <ClientSet>(context.ClientSet);

            });
            GetClientsTask.Wait();
        }

        public ClientsViewModel(ObservableCollection <ClientSet> clients)
        {
            _clients = clients;
        }

        public ClientsViewModel(DbSet<ClientSet> clients)
        {
            _clients = new ObservableCollection <ClientSet>(clients);
        }

    }
}
