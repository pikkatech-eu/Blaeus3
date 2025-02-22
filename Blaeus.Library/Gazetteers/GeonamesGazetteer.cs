/***********************************************************************************
* File:         GeonamesGazetteer.cs                                               *
* Contents:     Class GeonamesGazetteer                                            *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-14 14:32                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Net;
using System.Xml.Linq;
using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;
using Factotum.Xml;

namespace Blaeus.Library.Gazetteers
{
	/// <summary>
	/// Geonames gazetteer.
	/// Performs lookup for GeoLocalities using the Geonames REST API.
	/// </summary>
	public class GeonamesGazetteer : IGazetteer
	{
		#region Static constants
		/// <summary>
		/// Basis URL for toponym search.
		/// </summary>
		private static readonly string SEARCH_URL	= "http://api.geonames.org/search?";

		/// <summary>
		/// Basis URL for requests by Geonames ID.
		/// </summary>
		private static readonly string GET_URL		= "http://api.geonames.org/get?";
		#endregion

		#region Static Properties
		/// <summary>
		/// Geonames requires a registered UserName. "AlexKonnen" is one of them.
		/// </summary>
		public static string UserName	{get;set;}	= "";

		/// <summary>
		/// Password seems not to be necessary, here just for completeness.
		/// Maybe ignored.
		/// </summary>
		public static string Password	{get;set;}	= "";

		/// <summary>
		/// If set to true, it uses the "name" search parameters, otherwise "name_startsWith".
		/// </summary>
		public static bool UseFullName	{get;set;}	= true;

		/// <summary>
		/// The feature class to query for. Default: X (unknown, will be ignored).
		/// </summary>
		public static GeoNamesFeatureClass	FeatureClass	{get;set;}	= GeoNamesFeatureClass.X;

		/// <summary>
		/// The feature code to query for. Default: NONE (will be ignored).
		/// </summary>
		public static GeoNamesFeatureCode	FeatureCode		{get;set;}	= GeoNamesFeatureCode.NONE;

		/// <summary>
		/// The city class to use, if any.
		/// Seems not to work properly.
		/// </summary>
		public static GeonamesCityClass		CityClass		{get;set;}	= GeonamesCityClass.None;

		/// <summary>
		/// Output style.
		/// </summary>
		public static GeonamesOutputFormat	OutputType		{get;set;}	= GeonamesOutputFormat.Xml;

		/// <summary>
		/// The query style.
		/// </summary>
		public static GeonamesQueryStyle	QueryStyle		{get;set;}	= GeonamesQueryStyle.Long;

		/// <summary>
		/// Order responce. Relevance or population are functional; elevation practically not.
		/// </summary>
		public static GeonamesResponseOrder	ResponseOrder	{get;set;}	= GeonamesResponseOrder.Population;

		/// <summary>
		/// Include bounding box info, regardelss of style setting. 
		/// </summary>
		public static bool					IncludeBox		{get;set;}	= true;
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

			if (UseFullName)
			{
				url += $"name={toponym}";
			}
			else
			{
				url += $"name_startsWith={toponym}";
			}

			if (FeatureClass != GeoNamesFeatureClass.X)
			{
				url += $"&featureClass={FeatureClass}";
			}

			if (FeatureCode != GeoNamesFeatureCode.NONE)
			{
				url += $"&featureCode={FeatureCode}";
			}

			if (CityClass != GeonamesCityClass.None)
			{
				url += $"&featureCode={CityClass}".ToLower();
			}

			url += $"&type={OutputType}".ToLower();

			url += $"&style={QueryStyle}".ToLower();

			url += $"&orderby={ResponseOrder}".ToLower();

			if (IncludeBox)
			{
				url += $"&inclBbox=true";
			}

			url += $"&userName={UserName}";

			using (HttpClient client = new HttpClient())
			{
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
				HttpResponseMessage response = client.Send(request);

				using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
				{
					string s = reader.ReadToEnd();

					XElement x = XElement.Parse(s);

					List<GeoLocality> localities = new List<GeoLocality>();

					foreach (XElement xItem in x.Elements("geoname"))
					{
						string featureClass = xItem.ElementValue<string>("fcl");

						if (String.IsNullOrEmpty(featureClass) || !(featureClass == "A" || featureClass == "P"))
						{
							continue;
						}

						GeoLocality locality = this.FromXElement(xItem);

						localities.Add(locality);
					}

					localities.Sort(new PopulationComparer());

					return localities.ToArray();
				}
			}
		}

		/// <summary>
		/// Gets a locality by its Geonames ID.
		/// </summary>
		/// <param name="id">The ID converted to string.</param>
		/// <returns>The value of GeoLocality, if found, otherwise null.</returns>
		public GeoLocality GetLocality(string id)
		{
			string url = $"{GET_URL}geonameId={id}&style={QueryStyle}&username={UserName}";

			using (HttpClient client = new HttpClient())
			{
				try
				{
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
					HttpResponseMessage response = client.Send(request);

					using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
					{
						string s = reader.ReadToEnd();

						XElement x = XElement.Parse(s);

						GeoLocality locality = this.FromXElement(x);

						return locality;
					}
				}
				catch (WebException wx)
				{
					// returns 404 if the id is not valid.
					return null;
				}
			}
		}
		#endregion

		#region Private Auxiliary
		/// <summary>
		/// Retrieves GeoLocality from an XElement.
		/// TODO: how the alternative names should be handled?
		/// </summary>
		/// <param name="x">The XElement to retrieve from.</param>
		/// <returns>An instance of GeoLocality, if successful, othersise null.</returns>
		private GeoLocality FromXElement(XElement x)
		{
			GeoLocality locality			= new GeoLocality();

			locality.Name					= x.ElementValue<string>("toponymName");
			locality.GeonamesId				= x.ElementValue<int>("geonameId");
			locality.Point.Latitude			= x.ElementValue<double>("lat");
			locality.Point.Longitude		= x.ElementValue<double>("lng");
			locality.CountryCode			= x.ElementValue<string>("countryCode");
			locality.Population				= x.ElementValue<int>("population");
			object oEnum					= x.ElementEnum(typeof(GeoNamesFeatureCode), "fcode", GeoNamesFeatureCode.NONE);
			locality.GeoNamesFeatureCode	= oEnum != null ? (GeoNamesFeatureCode)oEnum : GeoNamesFeatureCode.NONE;
			oEnum							= x.ElementEnum(typeof(GeoNamesFeatureClass), "fcl", GeoNamesFeatureClass.X);
			locality.GeoNamesFeatureClass	= oEnum != null ? (GeoNamesFeatureClass)oEnum : GeoNamesFeatureClass.X;

			XElement xBoundingBox			= x.Element("bbox");

			if (xBoundingBox != null)
			{
				double west		= xBoundingBox.ElementValue<double>("west");
				double north	= xBoundingBox.ElementValue<double>("north");
				double east		= xBoundingBox.ElementValue<double>("east");
				double south	= xBoundingBox.ElementValue<double>("south");

				locality.BoundingBox	= new GeoRectangle(west, north, east, south);
			}

			if (locality.IsValid())
			{
				return locality;
			}
			else
			{
				return null;
			}
		}
		#endregion
	}
}
