/***********************************************************************************
* File:         WikidataGazetteer.cs                                               *
* Contents:     Class WikidataGazetteer                                            *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-14 15:05                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Net;
using System.Xml.Linq;
using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Management;
using Newtonsoft.Json.Linq;

namespace Blaeus.Library.Gazetteers
{
	/// <summary>
	/// Wikidata gazetteer.
	/// Performs lookup for GeoLocalities using the Wikidata REST API.
	/// </summary>
	public class WikidataGazetteer : IGazetteer
	{
		#region Private constants
		/// <summary>
		/// Basis URL for toponym search.
		/// </summary>
		private const string SEARCH_URL		= "https://www.wikidata.org/w/api.php?action=wbsearchentities";

		/// <summary>
		/// Basis URL for requests by Openstreetmap Relation ID.
		/// </summary>
		private const string GET_URL		= "http://www.wikidata.org/entity/";

		/// <summary>
		/// Defines the default output format.
		/// </summary>
		private const string OUTPUT_TYPE	= "json";

		/// <summary>
		/// Basis URL to lookup Wikidata ID ba Geonames ID.
		/// </summary>
		private const string URL_WIKIDATA_ID_BY_GEONAMES = "https://query.wikidata.org/sparql?query=SELECT ?item WHERE { ?item wdt:P1566 \"XXXX\". }&format=json";
		
		/// <summary>
		/// Basis source URL for the historic names' sources.
		/// </summary>
		private const string SOURCE_URL		= "https://www.wikidata.org/wiki/";
		#endregion

		#region Properties
		/// <summary>
		/// Maximum rows to lookup in a toponym request.
		/// </summary>
		public static int MaxRows	{get;set;} = 10;
		#endregion

		/// <summary>
		/// When using Wikidata REST API, you normally only become very restricted information on places, including theier Wikidata IDs.
		/// Using an ID, one obtains many details more.
		/// </summary>
		/// <param name="toponym">Toponym to lookup.</param>
		/// <returns>Array of Geolocalities found.</returns>
		public GeoLocality[] Lookup(string toponym)
		{
			string url = $"{SEARCH_URL}&search={toponym}&format={OUTPUT_TYPE}&language=en&uselang=en&type=item&limit={MaxRows}";

			using (HttpClient client = new HttpClient())
            {
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
				request.Headers.UserAgent.ParseAdd("Other");

				HttpResponseMessage response = client.Send(request);

                using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string s = reader.ReadToEnd();

                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(s);

					JArray search = (JArray)json["search"];

                    List<GeoLocality> result = new List<GeoLocality>();

					foreach (JObject item in search)
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
		/// Fetches a locality by its wikidataId.
		/// </summary>
		/// <param name="wikidataId">The wikidataId to fetch.</param>
		/// <returns>The locality fetched; null if the wikidataId was invalid.</returns>
		public GeoLocality GetLocality(string wikidataId)
		{
			string url = $"{GET_URL}{wikidataId}";

			using (HttpClient client = new HttpClient())
            {
				try
				{
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
					request.Headers.UserAgent.ParseAdd("Other");

					HttpResponseMessage response = client.Send(request);
					
					using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
					{
						string s = reader.ReadToEnd();

						dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(s);

						GeoLocality locality	= new GeoLocality();
						locality.WikiDataId		= wikidataId;

						JObject entities = (JObject)json["entities"];
						JObject entity = (JObject)entities[wikidataId];

						JObject labels = (JObject)entity["labels"];

						try
						{
							locality.Name			= labels.First.First["value"]?.ToString();
						}
						catch (Exception)
						{
						}

						try
						{
							JObject descriptions	= (JObject)entity["descriptions"];
							locality.Description	= (string)descriptions["en"]["value"];
						}
						catch (Exception)
						{
						}

						JObject claims				= (JObject)entity["claims"];
						JArray p625					= (JArray)claims["P625"];

						if (p625 != null)
						{
							JObject coordinates		= (JObject)p625[0]["mainsnak"]["datavalue"]["value"];

							locality.Point				= new GeoPoint();
							locality.Point.Latitude		= (double)coordinates["latitude"];
							locality.Point.Longitude	= (double)coordinates["longitude"];

							JArray p2044				= (JArray)claims["P2044"];

							if (p2044 != null)
							{
								string elevation		= (string)p2044[0]["mainsnak"]["datavalue"]["value"]["amount"];
								locality.Elevation		= Double.Parse(elevation);
							}

							JArray p1082				= (JArray)claims["P1082"];

							if (p1082 != null)
							{
								string population	= (string)p1082[p1082.Count - 1]["mainsnak"]["datavalue"]["value"]["amount"];
								locality.Population	= Int32.Parse(population);
							}
						
							JArray p1566			= (JArray)claims["P1566"];

							if (p1566 != null)
							{
								string geonameId	= (string)p1566[0]["mainsnak"]["datavalue"]["value"];
								locality.GeonamesId	= Int32.Parse(geonameId);
							}
						}

						// p402 contains OSM relation ID
						JArray p402					= (JArray)claims["P402"];

						if (p402 != null)
						{
							string osmRelationId	= (string)p402[0]["mainsnak"]["datavalue"]["value"];
							locality.OpenStreetMapRelationId = Int32.Parse(osmRelationId);
						}

						locality.HistoricNames		= this.GetHistoricNames(claims, wikidataId);
						locality.AlternativeNames	= this.GetAlternativeNames(labels);

						return locality;
					}
					
				}
				catch (WebException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Requests For Wikidata ID using Geonames ID as the parameter.
		/// Knowing the Wikidata ID you may query Wikidata for all its infos, e.g. the historical names.
		/// You can also obtain the Openstreetmap ID, using the latter you may obtain the bounding box and the polygon.
		/// </summary>
		/// <param name="geonamesId">The Geonames ID to rwquest for.</param>
		/// <returns>The Wikidata ID, if successfull, otherwise null.</returns>
		public string GetWikidataIdByGeonamesId(int geonamesId)
		{
			string url = URL_WIKIDATA_ID_BY_GEONAMES.Replace("XXXX", geonamesId.ToString());

			using (HttpClient client = new HttpClient())
            {
				try
				{
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
					request.Headers.UserAgent.ParseAdd("Other");

					HttpResponseMessage response = client.Send(request);

					using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
					{
						string s = reader.ReadToEnd();

						dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(s);

						string wikidata = json["results"]["bindings"][0]["item"]["value"].ToString();

						return wikidata.Split('/')[^1];
					}
				}
				catch
				{
					return null;
				}
			}
		}

		#region Private Auxiliary
		/// <summary>
		/// Builds a Locality instance from a Json object.
		/// </summary>
		/// <param name="item">The Json object to build from.</param>
		/// <returns>Locality build, if successful, otherwise null.</returns>
		private GeoLocality FromJsonObject(JObject item)
		{
			try
			{
				string wikidataId = (string)item["wikidataId"];

				return this.GetLocality(wikidataId);
			}
			catch(Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the historic names for a GeoLocality.
		/// </summary>
		/// <param name="claims">The Json object to extract historic names from.</param>
		/// <param name="wikidataID">WikidataID to use for the source entriy.</param>
		/// <returns>List of historic names.</returns>
		private List<HistoricName> GetHistoricNames(JObject claims, string wikidataID)
		{
			JArray entries = (JArray)claims["P1448"];

			List<HistoricName> historicNames	= new List<HistoricName>();

			if (entries == null)
			{
				return historicNames;
			}

			foreach (var entry in entries)
			{
				try
				{
					string text		= entry["mainsnak"]["datavalue"]["value"]["text"].ToString();
					string language	= entry["mainsnak"]["datavalue"]["value"]["language"].ToString();

					string dateFrom = null;
					string dateTo	= null;

					try
					{
						 dateFrom	= entry["qualifiers"]["P580"][0]["datavalue"]["value"]["time"].ToString();
					}
					catch {}

					try
					{
						dateTo		= entry["qualifiers"]["P582"][0]["datavalue"]["value"]["time"].ToString();
					}
					catch {}

					if (dateFrom == null && dateTo == null)
					{
						continue;
					}

					HistoricName historic	= new HistoricName();
					historic.Key			= language;
					historic.Name			= text;
					historic.Source			= $"{SOURCE_URL}{wikidataID}";
					
					if (dateFrom != null)
					{
						historic.From		= Int32.Parse(dateFrom[1..5]);	// +1772-00-00T00:00:00Z
					}
					
					if (dateTo != null)
					{
						historic.To		= Int32.Parse(dateTo[1..5]);
					}

					historicNames.Add(historic);
				}
				catch (Exception)
				{
					continue;
				}
			}

			return historicNames;
		}

		/// <summary>
		/// Gets the alternative names for a GeoLocality.
		/// </summary>
		/// <param name="labels">The Json object to extract alternative names from.</param>
		/// <returns>Dictionary of alternative names.</returns>
		private Dictionary<string, string> GetAlternativeNames(JObject labels)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			foreach (var label in labels)
			{
				string key = label.Key;
				string value = label.Value["value"].ToString();

				if (Settings.Instance.Languages.Contains(key))
				{
					result[key]	= value;
				}
			}

			return result;
		}
		#endregion
	}
}
