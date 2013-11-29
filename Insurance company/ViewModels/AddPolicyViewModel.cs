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

namespace Insurance_company.ViewModels
{
    class AddPolicyViewModel : BaseViewModel
    {
        private ServiceReference.PolicySet _policy;

        private const string CAR = "Car";
        private const string HOUSE = "House";

        public ServiceReference.PolicySet Policy
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

        private ServiceReference.HouseSet _house;
        public ServiceReference.HouseSet House
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

        private ServiceReference.AdressSet _address;
        public ServiceReference.AdressSet Address
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

        private ServiceReference.CarSet _car;
        public ServiceReference.CarSet Car
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

        public ICommand SavePolicyCommand { get { return new DelegateCommand(OnPolicySave); } }

        public AddPolicyViewModel() {
            setEntities();
        }

        private void setEntities()
        {
            _policy = new ServiceReference.PolicySet();
            _car = new ServiceReference.CarSet();
            _house = new ServiceReference.HouseSet();
            _address = new ServiceReference.AdressSet();

            Task.Factory.StartNew(() =>
            {
                //using (var db = new ServiceReference.InsuranceCompanyEntities())
                //{;
                //Clients = new ObservableCollection<ServiceReference.ClientSet>(db.ClientSet);
                //}
            });

        }

        private async Task savePolicy()
        {
            //using (var db = new ServiceReference.InsuranceCompanyEntities())
            //{
            //    DateTime now = DateTime.Now;
            //    Policy.StartDate = now; // Setting StartDate
            //    Policy.EndDate = now.AddYears(Policy.Duration); // Setting EndDate
            //    db.PolicySet.Add(Policy);
            //    if (Policy.ObjectType.Equals(CAR)) // If we are adding car policy
            //    {
            //        db.CarSet.Add(Car);
            //    }
            //    else if (Policy.ObjectType.Equals(HOUSE)) // If we are adding house policy
            //    {
            //        db.AdressSet.Add(Address);
            //        House.AdressSet = Address; // Assign Address to the house
            //        db.HouseSet.Add(House);
            //    }
            //    try
            //    {
            //        await db.SaveChangesAsync();   // Save changes to the database                 
            //    }
            //    catch (DbEntityValidationException e)
            //    {
            //        MessageBox.Show("Adding a new policy caused an error: " + e.Message);
            //    }
            //}
        }

        private void OnPolicySave(object parameter)
        {

            Task.Factory.StartNew(() => savePolicy()).ContinueWith(t =>
            {
                MessageBox.Show("Policy added successfully!");
                // Clear the form:
                Policy = new ServiceReference.PolicySet();
                Address = new ServiceReference.AdressSet();
                House = new ServiceReference.HouseSet();
                Car = new ServiceReference.CarSet();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

        }
    }
}
