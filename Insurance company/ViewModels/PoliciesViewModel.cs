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

namespace Insurance_company.ViewModels
{
    class PoliciesViewModel : BaseViewModel
    {
        private ObservableCollection<ServiceReference.PolicySet> _policies;

        public ObservableCollection<ServiceReference.PolicySet> Policies
        {
            get { return _policies; }
            set
            {
                if (_policies != value)
                {
                    _policies = value;
                    RaisePropertyChanged(() => Policies);
                }
            }

        }

        public void refresh(System.Data.Entity.DbSet<ServiceReference.PolicySet> policies) // Refreshing the list of policies in the DataGrid
        {
            //Policies = new ObservableCollection<PolicySet>(policies);
            ObservableCollection<ServiceReference.PolicySet> policiess = new ObservableCollection<ServiceReference.PolicySet>(policies);
            for (int i = Policies.Count; i < policies.Count(); i++)
            {
                Policies.Add(policiess[i]);
            }
        }

        public ICommand PoliciesGridLeftDoubleClickCommand { get { return new DelegateCommand(OnPoliciesGridLeftDoubleClick); } }

        private void OnPoliciesGridLeftDoubleClick(object parameter) // Clicking twice on a policy item opens Edit Window
        {
            if (!(parameter is ServiceReference.PolicySet))
                return;
            EditPolicy EditPolicyWindow = new EditPolicy();
            EditPolicyWindow.DataContext = new EditPolicyViewModel(parameter as ServiceReference.PolicySet); // Passing policy object which was clicked to the Edit Window
            EditPolicyWindow.ShowDialog();
        }

        public PoliciesViewModel()
        {
            var GetPoliciesTask = Task.Factory.StartNew(() =>
            {
                //using (var db = new InsuranceCompanyEntities())
                //{
                //    _policies = new ObservableCollection<PolicySet>(db.PolicySet);
                //}
            });
            GetPoliciesTask.Wait();
        }

        public PoliciesViewModel(ObservableCollection<ServiceReference.PolicySet> policies)
        {
            _policies = policies;
        }
        
    }
}
