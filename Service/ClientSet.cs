//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClientSet
    {
        public ClientSet()
        {
            this.PolicySet = new HashSet<PolicySet>();
        }
    
        public int ClientId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string PESEL { get; set; }
        public int AdressAdressId { get; set; }
    
        public virtual AdressSet AdressSet { get; set; }
        public virtual ICollection<PolicySet> PolicySet { get; set; }
    }
}
