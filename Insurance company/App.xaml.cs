using System.Windows;
using Insurance_company.Models;

namespace Insurance_company
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            using (var db = new InsuranceCompanyEntities())
            {
                db.Database.CreateIfNotExists();
            }
        }

    }
}
