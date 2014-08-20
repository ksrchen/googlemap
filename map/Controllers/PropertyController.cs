using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;
using map.Models;

namespace map.Controllers
{
    public class PropertyController : ApiController
    {
        public HttpResponseMessage Get(decimal top, decimal left, decimal bottom, decimal right, string format = null)
        {
            return format == "KML" ? GetKML(top, left, bottom, right) : GetGeoJSON(top, left, bottom, right);
        }


        private HttpResponseMessage GetGeoJSON(decimal top, decimal left, decimal bottom, decimal right)
        {
            try
            {
                Models.FeatureCollection collection = new Models.FeatureCollection { type = "FeatureCollection", features = new List<Feature>() };

                string query = string.Format(@"
declare @g geography;
SET @g = geography::STPolyFromText('POLYGON (({0} {2}, {1} {2}, {1} {3}, {0} {3}, {0} {2}))', 4326);

select top 200  p.propertyId, p.description, p.location 
from [dbo].Location p 
where @g.STContains(p.location)=1 
", left, right, top, bottom);
                using (var db = new test1Entities())
                {
                    db.Database.CommandTimeout = 5 * 60;

                    var result = db.Database.SqlQuery<Result>(query, 0);
                    foreach (var item in result)
                    {
                        var feature = new Feature
                        {
                            type = "Feature",
                            properties = new Dictionary<string, string>(),
                            geometry = new Geometry { type = "Point", coordinates = new double[2] },
                        };

                        feature.properties.Add("propertyID", item.propertyId.ToString());
                        feature.properties.Add("propertyName", item.description);

                        feature.geometry.coordinates[0] = item.location.StartPoint.Longitude.GetValueOrDefault();
                        feature.geometry.coordinates[1] = item.location.StartPoint.Latitude.GetValueOrDefault();

                        collection.features.Add(feature);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, collection);
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        private HttpResponseMessage GetKML(decimal top, decimal left, decimal bottom, decimal right)
        {
            try
            {
                Models.FeatureCollection collection = new Models.FeatureCollection { type = "FeatureCollection", features = new List<Feature>() };

                string query = string.Format(@"
declare @g geography;
SET @g = geography::STPolyFromText('POLYGON (({0} {2}, {1} {2}, {1} {3}, {0} {3}, {0} {2}))', 4326);

select top 200  p.propertyId, p.description, p.location 
from [dbo].Location p 
where @g.STContains(p.location)=1 
", left, right, top, bottom);

                XmlDocument xDoc = new XmlDocument();
                XmlDeclaration xDec = xDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement rootNode = xDoc.CreateElement("kml");
                rootNode.SetAttribute("xmlns", @"http://www.opengis.net/kml/2.2");
                xDoc.InsertBefore(xDec, xDoc.DocumentElement);
                xDoc.AppendChild(rootNode);
                XmlElement docNode = xDoc.CreateElement("Document");
                rootNode.AppendChild(docNode);

                using (var db = new test1Entities())
                {
                    db.Database.CommandTimeout = 5 * 60;

                    var result = db.Database.SqlQuery<Result>(query, 0);
                    foreach (var item in result)
                    {
                        XmlElement placeNode = xDoc.CreateElement("Placemark");
                        docNode.AppendChild(placeNode);

                        XmlElement idNode = xDoc.CreateElement("id");
                        XmlText idText = xDoc.CreateTextNode(item.propertyId.ToString());
                        placeNode.AppendChild(idNode);
                        idNode.AppendChild(idText);

                        XmlElement descNode = xDoc.CreateElement("description");
                        XmlText descText = xDoc.CreateTextNode(item.description);
                        placeNode.AppendChild(descNode);
                        descNode.AppendChild(descText);

                        XmlElement pointNode = xDoc.CreateElement("Point");
                        placeNode.AppendChild(pointNode);

                        XmlElement coordNode = xDoc.CreateElement("coordinates");
                        XmlText coordText = xDoc.CreateTextNode(string.Format("{0},{1}", 
                            item.location.StartPoint.Longitude.GetValueOrDefault(),
                            item.location.StartPoint.Latitude.GetValueOrDefault()));
                        pointNode.AppendChild(coordNode);
                        coordNode.AppendChild(coordText);
                    }
                }

                return new HttpResponseMessage
                {
                    Content = new StringContent(
                        xDoc.InnerXml,
                        Encoding.UTF8,
                        "application/xml"),
                };
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

    }
}
