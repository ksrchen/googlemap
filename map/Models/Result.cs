using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace map.Models
{
    public class Result
    {
        public int propertyId { get; set; }
        public string description { get; set; }
        public  DbGeography  location { get; set; }
        //public decimal Latitude { get; set; }
        //public decimal Longitude { get; set; }
        //public string Address1 { get; set; }
        //public string CityName { get; set; }
        //public string StateProvCode { get; set; }
        //public string PostalZipCode { get; set; }
    }
}