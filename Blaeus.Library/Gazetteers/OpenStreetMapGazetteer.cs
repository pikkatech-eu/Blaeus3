/***********************************************************************************
* File:         OpenStreetMapGazetteer.cs                                          *
* Contents:     Class OpenStreetMapGazetteer                                       *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-14 14:46                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Net;
using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;
using Blaeus.Library.Extensions;
using Factotum.Logging;
using Factotum.Text;
using Newtonsoft.Json.Linq;

namespace Blaeus.Library.Gazetteers
{
	/// <summary>
	/// Openstreetmap gazetteer.
	/// Performs lookup for GeoLocalities using the Openstreetmap REST API.
	/// Look at https://nominatim.org/release-docs/latest/api/Search/ for parameters.
	/// </summary>
	public class OpenStreetMapGazetteer : IGazetteer
	{
		#region Private Constants
		/// <summary>
		/// Basis URL for toponym search.
		/// </summary>
		private static readonly string SEARCH_URL	= "https://nominatim.openstreetmap.org/search?";

		/// <summary>
		/// Basis URL for requests by Openstreetmap Relation ID.
		/// </summary>
		private static readonly string GET_URL		= "https://nominatim.openstreetmap.org/details.php?";
		#endregion

		#region Static Properties
		/// <summary>
		/// Output format.
		/// </summary>
		public static OpenStreetMapOutputFormat	OutputFormat	{get;set;}	= OpenStreetMapOutputFormat.JsonV2;

		/// <summary>
		/// When set to true, include a full list of names for the result. These may include language variants, older names, references and brand.
		/// </summary>
		public static bool IncludeNameDetails					{get;set;}	= false;

		/// <summary>
		/// To obtain a more fine-grained selection for places. 
		/// A featureType of settlement selects any human inhabited feature from 'state' down to 'neighbourhood'.
		/// </summary>
		public static OpenStreetMapFeatureType FeatureType		{get;set;} = OpenStreetMapFeatureType.City;
		
		/// <summary>
		/// Type of polygon output.
		/// </summary>
		public static OpenStreetMapPolygonOutputType	PolygonOutputType	{get;set;}	= OpenStreetMapPolygonOutputType.Polygon_Geojson;

		/// <summary>
		/// The parameter describes the tolerance in degrees with which the geometry may differ from the original geometry. Topology is preserved in the geometry. 
		/// (https://nominatim.org/release-docs/latest/api/Search/)
		/// </summary>
		public static double PolygonThreshold					{get;set;}	= GeoRectangle.CoordinatePrecision;
		#endregion

		#region Public Features
		/// <summary>
		/// Performs lookup for localities based on a toponym.
		/// </summary>
		/// <param name="toponym">The toponym to look for.</param>
		/// <returns>Array of Geolocalities found.</returns>
		public GeoLocality[] Lookup(string toponym)
		{
			string url = $"{SEARCH_URL}";
			url += $"{FeatureType.ToString().ToLower()}={toponym}";
			url += $"&format={OutputFormat.ToString().ToLower()}";
			if (IncludeNameDetails)
			{
				url += "&namedetails=1";
			}

			if (PolygonOutputType != OpenStreetMapPolygonOutputType.None)
			{
				url += $"&{PolygonOutputType.ToString().ToLower()}=1";

				url += $"&polygon_threshold={PolygonThreshold}";
			}

			using (HttpClient client = new HttpClient())
            {
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
				request.Headers.UserAgent.ParseAdd("Other");

				HttpResponseMessage response = client.Send(request);

                using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string s = reader.ReadToEnd();

					dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(s);

                    List<GeoLocality> result = new List<GeoLocality>();

                    foreach (var item in json)
                    {
                        GeoLocality locality = this.FromJsonObject(item);

                        if (locality != null)
                        {
							result.Add(locality);
                        }
                    }

                    return result.ToArray();
                }
            }
		}

		/// <summary>
		/// Fetches a locality by its id.
		/// </summary>
		/// <param name="id">The id to fetch.</param>
		/// <returns>The locality fetched; null if the id was invalid.</returns>
		public GeoLocality GetLocality(string id)
		{
			string url = $"{GET_URL}osmtype=R&osmid={id}&format=json&polygon_geojson=1";

			using (HttpClient client = new HttpClient())
			{
				try
				{
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
					request.Headers.UserAgent.ParseAdd("Other");

					HttpResponseMessage response = client.Send(request);

					using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
					{
						GeoLocality locality = new GeoLocality();

						string s = reader.ReadToEnd();

						dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(s);

						if (json["error"] != null)
						{
							Logger.Error(json.ToString());

							return null;
						}

						locality.OpenStreetMapRelationId = Int32.Parse(id);
						locality.Name = json["localname"];

						JObject names = (JObject)json["names"];

						locality.CountryCode = json["country_code"];

						JObject extratags = (JObject)json["extratags"];

						try
						{
							locality.WikiDataId = (string)extratags["wikidata"];
						}
						catch (Exception)
						{
						}

						var population = extratags["population"];
						if (population != null)
						{
							locality.Population = Int32.Parse((string)extratags["population"]);
						}

						JObject centroid			= (JObject)json["centroid"];
							
						JArray coordinates			= (JArray)centroid["coordinates"];
						locality.Point.Latitude		= (double)coordinates[1];
						locality.Point.Longitude	= (double)coordinates[0];

						JObject geometry			= (JObject)json["geometry"];

						List<GeoPoint> points		= new List<GeoPoint>();

						coordinates					= (JArray)geometry["coordinates"];

						int depth = coordinates.GetArrayDepth();

						JArray coordinateArray = null;

						if (depth == 3)
						{
							coordinateArray	= (JArray)coordinates[0];
						}
						else if (depth == 4)
						{
							coordinateArray	= (JArray)coordinates[0][0];
						}

						foreach (var item in coordinateArray)
						{
							GeoPoint point			= GeoPoint.Parse(item.ToString());

							point.SwapCoordinates();

							points.Add(point);
						}

						locality.Polygon			= new GeoPolygon(points);

						return locality;
					}
				}
				catch (WebException wx)
				{
					return null;
				}
			}
		}
		#endregion

		#region Private Auxiliary
		/// <summary>
		/// Retrieves a GeoLocality from its Json object.
		/// TODO: only supports retrieving of polygon from GeoJson format.
		/// TODO: how the alternative names should be handled?
		/// </summary>
		/// <param name="jObject">The Json object to retrieve from.</param>
		/// <returns>The GeoLocality instance retrieved, if successful, otherwise null.</returns>
		private GeoLocality FromJsonObject(JObject jObject)
		{
			GeoLocality locality		= new GeoLocality();

			locality.OpenStreetMapNodeId	= Int32.Parse(jObject["place_id"].ToString());
			locality.OpenStreetMapRelationId	= Int32.Parse(jObject["osm_id"].ToString());

			double latitude	= Double.Parse(jObject["lat"].ToString());
			double longitude	= Double.Parse(jObject["lon"].ToString());

			locality.Point	= new GeoPoint(latitude, longitude);

			locality.Name	= jObject["name"].ToString();

			string addressType	= jObject["addresstype"].ToString();

			locality.OpenStreetMapPlaceCategory = (OpenStreetMapPlaceCategory)Enum.Parse(typeof(OpenStreetMapPlaceCategory), addressType.Capitalize());

			var boundingBox = jObject["boundingbox"];

			double south = Double.Parse(boundingBox[0].ToString());
			double north = Double.Parse(boundingBox[1].ToString());
			double west = Double.Parse(boundingBox[2].ToString());
			double east = Double.Parse(boundingBox[3].ToString());

			locality.BoundingBox	= new GeoRectangle(west, north, east, south);

			var geojson	= jObject["geojson"];
			var coordinates = geojson["coordinates"][0];

			List<GeoPoint> points = new List<GeoPoint>();

			foreach (var coordinateItem in coordinates)
			{
				string s = coordinateItem.ToString();

				longitude = Double.Parse(coordinateItem[0].ToString());
				latitude = Double.Parse(coordinateItem[1].ToString());

				GeoPoint point	= new GeoPoint(latitude, longitude);

				points.Add(point);
			}

			locality.Polygon	= new GeoPolygon(points);

			return locality;
		}

		#endregion
	}
}
