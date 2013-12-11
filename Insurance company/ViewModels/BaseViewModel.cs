using Insurance_company.Helpers;
using System.Collections.ObjectModel;
using System;
using Insurance_company.ServiceReference;
using System.Windows;
using System.Text.RegularExpressions;

namespace Insurance_company.ViewModels
{
    public class BaseViewModel : NotificationObject
    {

        protected static Uri svcUri = new Uri("http://localhost:48833/InsuranceCompanyService.svc");

        

    }
}