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
using Insurance_company.ServiceReference;
using System.Linq.Expressions;

namespace Insurance_company.ViewModels
{
    class SearchPolicyViewModel : BaseViewModel
    {
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        private ObservableCollection<ServiceReference.PolicySet> _policies;

        public ObservableCollection<ServiceReference.PolicySet> Policies
        {
            get { return _policies; }
            set
            {
                if (_policies != value)
                {
                    _policies = value;
                    RaisePropertyChanged(() => "Policies");
                }
            }

        }

        private ObservableCollection<ServiceReference.ClientSet> _clients;
        public ObservableCollection<ServiceReference.ClientSet> Clients
        {
            get { return _clients; }
            set
            {
                if (value != _clients)
                {
                    _clients = value;
                    RaisePropertyChanged(() => Clients);
                };
            }
        }

        List<EntityParameter> PolicyParameters = new List<EntityParameter>();

        private ServiceReference.PolicySet _policy;

        public ServiceReference.PolicySet Policy
        {
            get { return _policy; }
            set
            {
                if (value != _policy)
                {
                    _policy = value;
                    RaisePropertyChanged(() => "_policy");
                };
            }
        }

        public ICommand SearchPolicyCommand { get { return new DelegateCommand(OnPolicySearch); } }

        public SearchPolicyViewModel() {
            _policy = new ServiceReference.PolicySet();
            Task.Factory.StartNew(() =>
            {
                
                Clients = new ObservableCollection<ClientSet>(context.ClientSet);
                
            });
        }

        public Expression<Func<PolicySet, bool>> GetWhereLambda(PolicySet policy)
        {
            ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(PolicySet), "p");

            System.Linq.Expressions.Expression clientExpr;
            System.Linq.Expressions.Expression durationExpr;
            System.Linq.Expressions.Expression typeExpr;
            System.Linq.Expressions.Expression cond = null;

            if (policy.ClientClientId != 0)
            {
                if (!policy.ClientClientId.Equals(""))
                {
                    System.Linq.Expressions.Expression prop = System.Linq.Expressions.Expression.Property(param, "ClientClientId");
                    System.Linq.Expressions.Expression val = System.Linq.Expressions.Expression.Constant(policy.ClientClientId);
                    clientExpr = System.Linq.Expressions.Expression.Equal(prop, val);

                    if (cond == null)
                        cond = clientExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, clientExpr);
                }
            }


            if (policy.Duration != 0)
            {
                if (!policy.Duration.Equals(""))
                {
                    System.Linq.Expressions.Expression prop = System.Linq.Expressions.Expression.Property(param, "Duration");
                    System.Linq.Expressions.Expression val = System.Linq.Expressions.Expression.Constant(policy.Duration);
                    durationExpr = System.Linq.Expressions.Expression.Equal(prop, val);

                    if (cond == null)
                        cond = durationExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, durationExpr);
                }
            }

            if (policy.ObjectType != null)
            {
                if (!policy.ObjectType.Equals(""))
                {
                    System.Linq.Expressions.Expression prop = System.Linq.Expressions.Expression.Property(param, "ObjectType");
                    System.Linq.Expressions.Expression val = System.Linq.Expressions.Expression.Constant(policy.ObjectType);
                    typeExpr = System.Linq.Expressions.Expression.Equal(prop, val);

                    if (cond == null)
                        cond = typeExpr;
                    else
                        cond = System.Linq.Expressions.Expression.And(cond, typeExpr);
                }
            }

            if (cond != null)
                return System.Linq.Expressions.Expression.Lambda<Func<PolicySet, bool>>(cond, param);

            return null;
        }

        private void OnPolicySearch(object parameter)
        {
            IEnumerable<PolicySet> policies = null;
            Expression<Func<PolicySet, bool>> myLambda = null;
            _policies = new ObservableCollection<PolicySet>();

            myLambda = GetWhereLambda(Policy);
            Task.Factory.StartNew(() =>
            {
                if (myLambda == null)
                    MessageBox.Show("All fields are empty");
                else
                {
                    policies = context.PolicySet.Where(myLambda);

                    foreach (PolicySet policy in policies)
                    {
                        _policies.Add(policy);
                    }
                }
            }).ContinueWith(t =>
            {
                if (_policies.Count() == 0)
                    MessageBox.Show("No policies matching these criteria were found!");

                else
                {
                    PoliciesWindow pw = new PoliciesWindow();
                    pw.DataContext = new PoliciesViewModel(_policies);
                    pw.ShowDialog();
                    _policies = new ObservableCollection<PolicySet>(); // Zerujemy kolekcję w razie kolejnego wyszukiwania
                }
            });

        }
        
    }
}
