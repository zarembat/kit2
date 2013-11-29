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

namespace Insurance_company.ViewModels
{

    class ClientsViewModel : BaseViewModel
    {

        private ObservableCollection<ServiceReference.ClientSet> _clients;

        public ObservableCollection<ServiceReference.ClientSet> Clients
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
            if (!(parameter is ServiceReference.ClientSet))
                return;
            EditClient EditClientWindow = new EditClient(); // Creating Edit Window object
            EditClientWindow.DataContext = new EditClientViewModel(parameter as ServiceReference.ClientSet); // Passing client object to the Edit Window
            EditClientWindow.ShowDialog();
        }

        public void refresh(System.Data.Entity.DbSet<ServiceReference.ClientSet> clients) // Refreshing the list of clients in the DataGrid
        {
            // Clients = new ObservableCollection<ClientSet>(clients);
            ObservableCollection<ServiceReference.ClientSet> clientss = new ObservableCollection<ServiceReference.ClientSet>(clients);
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
                //    _clients = new ObservableCollection<ClientSet>(db.ClientSet);
                //}
            });
            GetClientsTask.Wait();
        }

        public ClientsViewModel(ObservableCollection<ServiceReference.ClientSet> clients)
        {
            _clients = clients;
        }

        public ClientsViewModel(DbSet<ServiceReference.ClientSet> clients)
        {
            _clients = new ObservableCollection<ServiceReference.ClientSet>(clients);
        }

    }
}
