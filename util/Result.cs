using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    public class Result
    {
        public int propertyId { get; set; }
        public string description { get; set; }
        public DbGeography location { get; set; }
    }
}
