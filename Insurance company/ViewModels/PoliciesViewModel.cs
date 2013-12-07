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
using Insurance_company.ServiceReference;
using System.Data.Services.Client;

namespace Insurance_company.ViewModels
{
    class PoliciesViewModel : BaseViewModel
    {
        private ObservableCollection<PolicySet> _policies;
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        public ObservableCollection<PolicySet> Policies
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

        public void refresh(IQueryable<PolicySet> policies) // Refreshing the list of policies in the DataGrid
        {
            //Policies = new ObservableCollection<PolicySet>(policies);
            ObservableCollection<PolicySet> policiess = new ObservableCollection<PolicySet>(policies);
            for (int i = Policies.Count; i < policies.Count(); i++)
            {
                Policies.Add(policiess[i]);
            }
        }

        public ICommand PoliciesGridLeftDoubleClickCommand { get { return new DelegateCommand(OnPoliciesGridLeftDoubleClick); } }

        private void OnPoliciesGridLeftDoubleClick(object parameter) // Clicking twice on a policy item opens Edit Window
        {
            if (!(parameter is PolicySet))
                return;
            EditPolicy EditPolicyWindow = new EditPolicy();
            EditPolicyWindow.DataContext = new EditPolicyViewModel(parameter as PolicySet); // Passing policy object which was clicked to the Edit Window
            EditPolicyWindow.ShowDialog();
        }

        public PoliciesViewModel()
        {

            DataServiceQuery<PolicySet> query = (DataServiceQuery<PolicySet>)(from policy in context.PolicySet select policy);

            try
            {
                query.BeginExecute(OnPoliciesQueryComplete, query);
            }
            catch (DataServiceQueryException e)
            {
                throw new ApplicationException(
                    "An error occurred during query execution.", e);
            }
        }

        public PoliciesViewModel(ObservableCollection<PolicySet> policies)
        {
            _policies = policies;
        }

        private void OnPoliciesQueryComplete(IAsyncResult result)
        {
            DataServiceQuery<PolicySet> query = result.AsyncState as DataServiceQuery<PolicySet>;
            Policies = new ObservableCollection<PolicySet>(query.EndExecute(result));
        }
        
    }
}
