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
    class EditPolicyViewModel : BaseViewModel
    {
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        private string ObjectType;
        private const string CAR = "Car";
        private const string HOUSE = "House";

        private PolicySet _policy;
        public PolicySet Policy
        {
            get { return _policy; }
            set
            {
                if (value != _policy)
                {
                    _policy = value;
                    RaisePropertyChanged(() => Policy);
                };
            }
        }

        private HouseSet _house;
        public HouseSet House
        {
            get { return _house; }
            set
            {
                if (value != _house)
                {
                    _house = value;
                    RaisePropertyChanged(() => House);
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

        private CarSet _car;
        public CarSet Car
        {
            get { return _car; }
            set
            {
                if (value != _car)
                {
                    _car = value;
                    RaisePropertyChanged(() => Car);
                };
            }
        }

        private ObservableCollection<ClientSet> _clients;
        public ObservableCollection<ClientSet> Clients
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

        public ICommand SavePolicyCommand { get { return new DelegateCommand(OnPolicySave); } }

        public EditPolicyViewModel()
        {

        }

        public EditPolicyViewModel(PolicySet policy)
        {
            _policy = policy;
            ObjectType = _policy.ObjectType;
            
            if (ObjectType.Equals(CAR)) // Object is a car
            {
                InsuranceCompanyEntities context = new InsuranceCompanyEntities(svcUri);

                DataServiceQuery<CarSet> query = (DataServiceQuery<CarSet>)(from car in context.CarSet
                                                                                      where car.Policy_PolicyId == _policy.PolicyId
                                                                                      select car);

                try
                {
                    query.BeginExecute(OnCarQueryComplete, query);
                }
                catch (DataServiceQueryException e)
                {
                    throw new ApplicationException(
                        "An error occurred during query execution.", e);
                }
            }
            else if (ObjectType.Equals(HOUSE)) // Object is a house
            {

                DataServiceQuery<HouseSet> query = (DataServiceQuery<HouseSet>)(from house in context.HouseSet.Expand("AdressSet")
                                                                                where house.Policy_PolicyId == _policy.PolicyId
                                                                                select house);

                try
                {
                    query.BeginExecute(OnHouseQueryComplete, query);
                }
                catch (DataServiceQueryException e)
                {
                    throw new ApplicationException(
                        "An error occurred during query execution.", e);
                }
                
            }    
        }

        private void OnCarQueryComplete(IAsyncResult result)
        {
            try
            {
                DataServiceQuery<CarSet> query = result.AsyncState as DataServiceQuery<CarSet>;
                Car = query.EndExecute(result).FirstOrDefault();
            }
            catch (DataServiceQueryException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OnHouseQueryComplete(IAsyncResult result)
        {
            try
            {
                DataServiceQuery<HouseSet> query = result.AsyncState as DataServiceQuery<HouseSet>;
                House = query.EndExecute(result).FirstOrDefault();
                Address = House.AdressSet;
            }
            catch (DataServiceQueryException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OnPolicySave(object parameter)
        {

            context.AttachTo("PolicySet", Policy);
            context.UpdateObject(Policy);

            if (ObjectType.Equals(CAR))
            {
                context.UpdateObject(Car);
            }
            if (ObjectType.Equals(House))
            {
                context.UpdateObject(House);
                context.UpdateObject(Address);
            }

            try
            {
                context.BeginSaveChanges(OnSaveChangesCompleted, null);
            }
            catch (DataServiceClientException ex)
            {
                MessageBox.Show(ex.ToString());
            }
                
        }

        private void OnSaveChangesCompleted(IAsyncResult result)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    context.EndSaveChanges(result);
                    MessageBox.Show("Policy edited successfully!");
                }
                catch (DataServiceRequestException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }));
        }

    }
}
