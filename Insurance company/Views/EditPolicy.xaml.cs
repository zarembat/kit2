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

namespace Insurance_company.Views
{
    /// <summary>
    /// Interaction logic for EditPolicy.xaml
    /// </summary>
    public partial class EditPolicy : Window
    {
        public EditPolicy()
        {
            InitializeComponent();
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
