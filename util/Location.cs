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
    
    public partial class Location
    {
        public int propertyId { get; set; }
        public System.Data.Entity.Spatial.DbGeography location1 { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string description { get; set; }
        public string propertyType { get; set; }
        public Nullable<decimal> Price { get; set; }
    }
}
