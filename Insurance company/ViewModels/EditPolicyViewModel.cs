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
using System.Text.RegularExpressions;

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

                //DataServiceQuery query = (DataServiceQuery)(from house in context.HouseSet
                //                                            join address in context.AdressSet
                //                                            on house.AdressSet_AdressId equals address.AdressId
                //                                            where house.Policy_PolicyId == _policy.PolicyId
                //                                            select new { house, address });

                //try
                //{
                //    query.BeginExecute(OnHouseQueryComplete, query);
                //}
                //catch (DataServiceQueryException e)
                //{
                //    throw new ApplicationException(
                //        "An error occurred during query execution.", e);
                //}

                //Uri houseUri = new Uri(svcUri.AbsoluteUri + "/HouseSet/?Policy_PolicyId=" + _policy.PolicyId);
                //Uri addressUri = new UriInsuranceCompanyEntities context = new InsuranceCompanyEntities(svcUri);

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
            DataServiceQuery<CarSet> query = result.AsyncState as DataServiceQuery<CarSet>;
            Car = query.EndExecute(result).FirstOrDefault();
        }

        private void OnHouseQueryComplete(IAsyncResult result)
        {
            DataServiceQuery<HouseSet> query = result.AsyncState as DataServiceQuery<HouseSet>;
            House = query.EndExecute(result).FirstOrDefault();
            Address = House.AdressSet;
        }

        private void OnPolicySave(object parameter)
        {
            if (!Validation())
                return;

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

        public bool Validation()
        {
            if (Policy.ClientClientId <= 0)
            {
                MessageBox.Show("Cliend ID is required!");
                return false;
            }

            string clientid = Policy.ClientClientId.ToString();
            if (!Regex.IsMatch(clientid, @"^[0-9]+$"))
            {
                MessageBox.Show("Client ID: Only digits!");
                return false;
            }

            if (Policy.ObjectType == null)
            {
                MessageBox.Show("Object type is required!");
                return false;
            }

            if (Policy.Duration < 1 || Policy.Duration > 5)
            {
                MessageBox.Show("Choose duration!");
                return false;
            }

            if (Policy.ObjectType.Equals(CAR))
            {
                if (string.IsNullOrEmpty(Car.Brand))
                {
                    MessageBox.Show("Brand is required!");
                    return false;
                }

                if (!Regex.IsMatch(Car.Brand, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
                {
                    MessageBox.Show("Brand: Only letters!");
                    return false;
                }

                if (Car.Year == 0)
                {
                    MessageBox.Show("Year is required!");
                    return false;
                }

                if (Car.Year < 1950 || Car.Year > System.DateTime.Now.Year)
                {
                    MessageBox.Show("Wrong year!");
                    return false;
                }

                if (string.IsNullOrEmpty(Car.VinNumber))
                {
                    MessageBox.Show("VIN number is required!");
                    return false;
                }

                if (!Regex.IsMatch(Car.VinNumber, @"^[\p{L}\p{N}]+$"))
                {
                    MessageBox.Show("VIN number: Only letters and numbers!");
                    return false;
                }

                if (string.IsNullOrEmpty(Car.Engine))
                {
                    MessageBox.Show("Engine is required!");
                    return false;
                }

                if (!Regex.IsMatch(Car.Engine, @"^[\p{L}\p{N}]+$"))
                {
                    MessageBox.Show("Engine: Only letters and numbers!");
                    return false;
                }

                if (Car.Type == null)
                {
                    MessageBox.Show("Choose type of car!");
                    return false;
                }
            }

            if (Policy.ObjectType.Equals(HOUSE))
            {
                if (House.Year == 0)
                {
                    MessageBox.Show("Year is required!");
                    return false;
                }

                if (House.Year < 1900 || House.Year > System.DateTime.Now.Year)
                {
                    MessageBox.Show("Wrong year!");
                    return false;
                }

                if (House.Size == 0)
                {
                    MessageBox.Show("Size is required!");
                    return false;
                }

                if (House.Size < 20 || House.Size > 2000)
                {
                    MessageBox.Show("Wrong size!");
                    return false;
                }

                if (House.Type == null)
                {
                    MessageBox.Show("Choose house type!");
                    return false;
                }

            }

            return true;
        }
    }
}
