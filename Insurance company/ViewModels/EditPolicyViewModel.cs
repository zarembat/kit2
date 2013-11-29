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
    class EditPolicyViewModel : BaseViewModel
    {

        private string ObjectType;
        private const string CAR = "Car";
        private const string HOUSE = "House";

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

        private ServiceReference.HouseSet _house;
        public ServiceReference.HouseSet House
        {
            get { return _house; }
            set
            {
                if (value != _house)
                {
                    _house = value;
                    RaisePropertyChanged(() => "_house");
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
                    RaisePropertyChanged(() => "_address");
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
                    RaisePropertyChanged(() => "_car");
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

        public EditPolicyViewModel()
        {

        }

        public EditPolicyViewModel(ServiceReference.PolicySet policy)
        {
            _policy = policy;
            var LoadPolicyTask = Task.Factory.StartNew(() =>
            {
                //using (var db = new InsuranceCompanyEntities())
                //{

                //    Clients = new ObservableCollection<ClientSet>(db.ClientSet);
                //    ObjectType = _policy.ObjectType;
                //    if (_policy.ObjectType.Equals(CAR)) // Object is a car
                //    {
                //        var car = db.CarSet.Where(c => c.Policy_PolicyId == policy.PolicyId).FirstOrDefault(); // Looking for a car in the database
                //        if (car == null)
                //            MessageBox.Show("No car...");
                //        else
                //        {
                //            _car = car;
                //        }
                //    }
                //    else if (_policy.ObjectType.Equals(HOUSE)) // Object is a house
                //    {
                //        var house = db.HouseSet.Where(h => h.Policy_PolicyId == policy.PolicyId).FirstOrDefault(); // Looking for a house
                //        var address = db.AdressSet.Where(a => a.AdressId == house.AdressSet.AdressId).FirstOrDefault(); // Getting house's address
                //        if (house == null || address == null)
                //            MessageBox.Show("No house...");
                //        else
                //        {
                //            _house = house;
                //            _address = address;
                //        }
                //    }
                //}
            });
            LoadPolicyTask.Wait();
        }

        private void OnPolicySave(object parameter)
        {

            Task.Factory.StartNew(() =>
            {
                //using (var db = new InsuranceCompanyEntities())
                //{
                //    var policy = db.PolicySet.Find(Policy.PolicyId);
                //    db.Entry(policy).CurrentValues.SetValues(Policy);

                //    if (ObjectType.Equals(CAR))
                //    {

                //        var car = db.CarSet.Find(Car.ObjectId); // Getting old entry
                //        db.Entry(car).CurrentValues.SetValues(Car); // Updating

                //    }
                //    if (ObjectType.Equals(House))
                //    {

                //        var house = db.HouseSet.Find(House.ObjectId);
                //        db.Entry(house).CurrentValues.SetValues(House);

                //        var address = db.AdressSet.Find(Address.AdressId);
                //        db.Entry(address).CurrentValues.SetValues(Address);

                //    }

                //    db.SaveChanges(); // Saving changes to the database
                //}
            }).ContinueWith(t => {
                MessageBox.Show("Saved!");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }


    }
}
