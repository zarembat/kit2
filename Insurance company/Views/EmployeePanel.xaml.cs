using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Insurance_company.Models;
using Insurance_company.ViewModels;

namespace Insurance_company.Views
{
    /// <summary>
    /// Interaction logic for panel.xaml
    /// </summary>
    public partial class EmployeePanel : Window
    {

        private EmployeeSet _employee;

        public EmployeePanel()
        {
            InitializeComponent();
        }

        public EmployeePanel(EmployeeSet employee) : this() // Wywołujemy domyślny konstruktor
        {
            this._employee = employee;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new InsuranceCompanyEntities())
            {
                if (Tabs.SelectedIndex == 0)
                {
                    if (ClientsGrid != null && ClientsGrid.DataContext != null)
                        ((ClientsViewModel)ClientsGrid.DataContext).refresh(db.ClientSet);
                }
                else if (Tabs.SelectedIndex == 1)
                {
                    if (PoliciesGrid != null && PoliciesGrid.DataContext != null)
                        ((PoliciesViewModel)PoliciesGrid.DataContext).refresh(db.PolicySet);
                }

                else if (Tabs.SelectedIndex == 6)
                {
                    numberOfClients.Text = db.ClientSet.Count().ToString();
                    numberOfPolicies.Text = db.PolicySet.Count().ToString();
                    numberOfHouses.Text = db.HouseSet.Count().ToString();
                    numberOfCars.Text = db.CarSet.Count().ToString();
                    numberOfEmployees.Text = db.EmployeeSet.Count().ToString();
                }
            }
        }

        private void ObjectTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            string value = ObjectTypeComboBox.SelectedValue.ToString();
            if (value.Equals("Car"))
            {
                CarGrid.Visibility = System.Windows.Visibility.Visible;
                HouseGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (value.Equals("House"))
            {
                HouseGrid.Visibility = System.Windows.Visibility.Visible;
                CarGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}
