using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    class Program
    {
        static void Main(string[] args)
        {
            //export2GoogleMapsEngine();
            export2Service();
        }

        private static void export2DB()
        {
            using (var db = new xChangeEntities())
            {
                int count = 1;
                foreach (var p in db.Properties.AsNoTracking().Where(p => p.StatusID == 10 && p.Address.Latitude != null))
                {
                    using (var dest = new test1Entities1())
                    {
                        if (!dest.Locations.Any(a => a.propertyId == p.PropertyID))
                        {
                            dest.Locations.Add(new Location
                            {
                                address = p.Address.Address1,
                                city = p.Address.CityName,
                                description = p.PropertyName,
                                propertyId = p.PropertyID,
                                location1 = DbGeography.PointFromText(string.Format("POINT({0} {1})", p.Address.Longitude, p.Address.Latitude), 4326),

                            });

                            Console.WriteLine(count++);
                            dest.SaveChanges();
                        }
                    }
                }

            }
        }

        private static void export2Service()
        {
            var client = new HttpClient();
            string baseUrl = "http://kmlservice.azurewebsites.net/";
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var dest = new test1Entities1())
            {
                List<Location> locations = new List<Location>();
                foreach (var item in dest.Locations.AsNoTracking())
                {
                    locations.Add(item);
                    item.Lat = item.location1.Latitude.GetValueOrDefault();
                    item.Lng = item.location1.Longitude.GetValueOrDefault();
                    item.location1 = null;
                    if (locations.Count() >= 50)
                    {
                        var result = client.PostAsJsonAsync<List<Location>>("api/property", locations).Result;
                        result.EnsureSuccessStatusCode();
                        locations.Clear();
                    }
                }

                if (locations.Count() > 0)
                {
                    var result = client.PostAsJsonAsync<List<Location>>("api/property", locations).Result;
                    result.EnsureSuccessStatusCode();
                }
            }
        }

        private static void export2GoogleMapsEngine()
        {

            using (var db = new test1Entities1())
            {
                string query = @"declare @g geography;
SET @g = geography::STPolyFromText('POLYGON ((-116.99398817578128 34.632786418164144, -119.49337782421878 34.632786418164144, -119.49337782421878 33.46768062957883, -116.99398817578128 33.46768062957883, -116.99398817578128 34.632786418164144))', 4326);

select p.propertyId, p.description, p.location 
from [dbo].Location p 
where @g.STContains(p.location)=1 
";
                db.Database.CommandTimeout = 5 * 60;
                var collection = new MapsEngineFeature { features = new List<Feature>() };
                var result = db.Database.SqlQuery<Result>(query, 0);
                foreach (var item in result)
                {
                    var feature = new Feature
                    {
                        type = "Feature",
                        properties = new Dictionary<string, string>(),
                        geometry = new Geometry { type = "Point", coordinates = new double[2] },
                    };

                    feature.properties.Add("gx_id", item.propertyId.ToString());
                    feature.properties.Add("description", item.description);

                    feature.geometry.coordinates[0] = item.location.StartPoint.Longitude.GetValueOrDefault();
                    feature.geometry.coordinates[1] = item.location.StartPoint.Latitude.GetValueOrDefault();

                    collection.features.Add(feature);
                    if (collection.features.Count >= 50)
                    {
                        postCollection(collection);
                        collection.features.Clear();
                    }
                }
                if (collection.features.Count > 0)
                {
                    postCollection(collection);
                }
            }
        }

        private static HttpClient GetClient()
        {
            var client = new HttpClient();
            string baseUrl = "https://www.googleapis.com/";
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", @"Bearer ya29.YQALoEWUvQ6pRCIAAABMCvzS_y9Yzj6VzQQt75O3JC5rlOudYLXQvwmlbjoXliYproekeNsfqbaZykMqxtY");
            return client;
        }

        private static void postCollection(MapsEngineFeature collection)
        {
            using (var client = GetClient())
            {
                var result = client.PostAsJsonAsync<MapsEngineFeature>("mapsengine/v1/tables/09492749347635688939-16143158689603361093/features/batchInsert", collection).Result;
                result.EnsureSuccessStatusCode();
            };      
        }
    }
}
