//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace util
{
    using System;
    using System.Collections.Generic;
    
    public partial class Property
    {
        public int PropertyID { get; set; }
        public int StatusID { get; set; }
        public int AddressID { get; set; }
        public Nullable<System.DateTime> DateApproved { get; set; }
        public System.DateTime DateEntered { get; set; }
        public Nullable<System.DateTime> DateLastModified { get; set; }
        public string ParcelTaxNum { get; set; }
        public string PropertyDesc { get; set; }
        public string PropertyName { get; set; }
        public string PropertySoundexCode { get; set; }
        public System.Guid msrepl_tran_version { get; set; }
    
        public virtual Address Address { get; set; }
    }
}
