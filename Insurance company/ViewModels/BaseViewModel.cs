using Insurance_company.Helpers;
using System.Collections.ObjectModel;
using System;

namespace Insurance_company.ViewModels
{
    public class BaseViewModel : NotificationObject
    {

        protected static Uri svcUri = new Uri("http://localhost:48833/InsuranceCompanyService.svc");

    }
}