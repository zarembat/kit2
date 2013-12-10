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
using Insurance_company.ViewModels;
using Insurance_company.ServiceReference;

namespace Insurance_company.Views
{
    /// <summary>
    /// Interaction logic for panel.xaml
    /// </summary>
    public partial class EmployeePanel : Window
    {

        private EmployeeSet _employee;
        InsuranceCompanyEntities context = new InsuranceCompanyEntities(new Uri("http://localhost:48833/InsuranceCompanyService.svc"));
        public EmployeePanel()
        {
            InitializeComponent();
            Loaded += windowLoaded;
        }

        public EmployeePanel(EmployeeSet employee)
            : this() // Wywołujemy domyślny konstruktor
        {
            this._employee = employee;
        }
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            if (_employee.Role == "User")
                addUserTab.Visibility = Visibility.Collapsed;
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex == 0)
            {
                if (ClientsGrid != null && ClientsGrid.DataContext != null)
                    ((ClientsViewModel)ClientsGrid.DataContext).refresh(context.ClientSet);
            }
            else if (Tabs.SelectedIndex == 1)
            {
                if (PoliciesGrid != null && PoliciesGrid.DataContext != null)
                    ((PoliciesViewModel)PoliciesGrid.DataContext).refresh(context.PolicySet);
            }

            if (Tabs.SelectedIndex == 6)
            {
                numberOfClients.Text = context.ClientSet.Count().ToString();
                numberOfPolicies.Text = context.PolicySet.Count().ToString();
                numberOfHouses.Text = context.HouseSet.Count().ToString();
                numberOfCars.Text = context.CarSet.Count().ToString();
                numberOfEmployees.Text = context.EmployeeSet.Count().ToString();
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
