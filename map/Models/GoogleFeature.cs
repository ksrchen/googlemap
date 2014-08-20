using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace map.Models
{
    public class FeatureCollection
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }

    }
    public class Feature
    {
        public string type { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public double[] coordinates { get; set; }
    }

    
}