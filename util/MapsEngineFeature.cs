using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    class MapsEngineFeature
    {
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
