using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_company.Helpers;
using System.Windows;
using System.Data.Entity.Validation;
using System.Collections.ObjectModel;
using Insurance_company.ServiceReference;
using System.Data.Services.Client;
using System.Text.RegularExpressions;

namespace Insurance_company.ViewModels
{
    class AddPolicyViewModel : BaseViewModel
    {
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        private PolicySet _policy;

        private const string CAR = "Car";
        private const string HOUSE = "House";

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

        public AddPolicyViewModel() {
            setEntities();
        }

        private void setEntities()
        {
            _policy = new PolicySet();
            _car = new CarSet();
            _house = new HouseSet();
            _address = new AdressSet();

            DataServiceQuery<ClientSet> query = (DataServiceQuery<ClientSet>)(from client in context.ClientSet select client);

            try
            {
                query.BeginExecute(OnClientsQueryComplete, query);
            }
            catch (DataServiceQueryException e)
            {
                throw new ApplicationException(
                    "An error occurred during query execution.", e);
            }

        }

        private void OnClientsQueryComplete(IAsyncResult result)
        {
            try
            {
                DataServiceQuery<ClientSet> query = result.AsyncState as DataServiceQuery<ClientSet>;
                Clients = new ObservableCollection<ClientSet>(query.EndExecute(result));
            }
            catch (DataServiceQueryException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void OnPolicySave(object parameter)
        {
            if (!Validation())
                return;
            try {
                DateTime now = DateTime.Now;
                Policy.StartDate = now; // Setting StartDate
                Policy.EndDate = now.AddYears(Policy.Duration); // Setting EndDate
                context.AddToPolicySet(Policy);
                if (Policy.ObjectType.Equals(CAR)) // If we are adding car policy
                {
                    context.AddRelatedObject(Policy, "CarSet", Car);
                    Policy.CarSet.Add(Car);
                    Car.PolicySet = Policy;
                }
                else if (Policy.ObjectType.Equals(HOUSE)) // If we are adding house policy
                {
                    context.AddToAdressSet(Address);
                    context.AddRelatedObject(Address, "HouseSet", House);
                    context.SetLink(House, "PolicySet", Policy);                
                    Policy.HouseSet.Add(House);
                    House.PolicySet = Policy;
                    Address.HouseSet.Add(House);
                    House.AdressSet = Address; // Assign Address to the house
                }
                context.BeginSaveChanges(OnSaveChangesCompleted, null);
            }
            catch (DataServiceClientException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void OnSaveChangesCompleted(IAsyncResult result)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    context.EndSaveChanges(result);
                    MessageBox.Show("Policy added successfully!");
                    // Clear the form:
                    Policy = new PolicySet();
                    Address = new AdressSet();
                    House = new HouseSet();
                    Car = new CarSet();
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
