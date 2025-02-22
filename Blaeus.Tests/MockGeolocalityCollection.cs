/***********************************************************************************
* File:         MockGeolocalityCollection.cs                                       *
* Contents:     Class MockGeolocalityCollection                                    *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-16 06:43                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;

namespace Blaeus.Tests
{
	public static class MockGeolocalityCollection
	{
		public static Dictionary<string, GeoLocality>	Localities = new Dictionary<string, GeoLocality>();

		static MockGeolocalityCollection()
		{
			PopulateCollection();
		}

		private static void PopulateCollection()
		{
			GeoLocality londonUK				= new GeoLocality();
			londonUK.Id							= 1;
			londonUK.GeonamesId					= 2643743;
			londonUK.OpenStreetMapRelationId	= 65606;
			londonUK.WikiDataId					= "Q84";

			londonUK.Name						= "London";
			londonUK.Description				= "capital and largest city of England and the United Kingdom";
			londonUK.Point						= new GeoPoint(51.50853, -0.12574);
			londonUK.Elevation					= 5;
			londonUK.GeoNamesFeatureClass		= GeoNamesFeatureClass.P;
			londonUK.GeoNamesFeatureCode		= GeoNamesFeatureCode.PPLC;
			londonUK.OpenStreetMapPlaceCategory	= OpenStreetMapPlaceCategory.City;
			londonUK.Population					= 8961989;
			londonUK.CountryCode				= "GB";
			londonUK.TimeZone					= 0.0;
			londonUK.BoundingBox				= new GeoRectangle{West=-0.5103751, North=51.6918741, East=0.3340155, South=51.2867601};
			//londonUK.Polygon					= new Library.Tools.GeoPolygon
			//																	(
			//																		51.509, -0.08, 51.503, -0.06, 51.51, -0.047, 51.515, 0
			//																	);

			londonUK.Polygon					= GeoPolygon.Load("Data/london.json", false);

			londonUK.AlternativeNames			= new Dictionary<string, string>(){{"fr", "Londres" }, {"it", "Londra"}, {"pl", "Londyn"}};

			londonUK.HistoricNames				= new List<HistoricName>
													{
														new HistoricName{Key="la", Name="Londinium", From=-47, To=408, Source="Wikipedia"}, 
														new HistoricName{Key="sx", Name="Lundenwic", To=650, Source="Wikipedia"} 
													};

			Localities.Add("LondonUK", londonUK);
			//-----------------------------------------------------------------
			// -------------------------------------------

			GeoLocality londonCA				= new GeoLocality();
			londonCA.Id							= 2;
			londonCA.GeonamesId					= 6058560;
			londonCA.OpenStreetMapRelationId	= 7485368;
			londonCA.WikiDataId					= "Q92561";
			londonCA.Name						= "London";
			londonCA.Point						= new GeoPoint(42.98339, -81.23304);
			londonCA.GeoNamesFeatureClass		= GeoNamesFeatureClass.P;
			londonCA.GeoNamesFeatureCode		= GeoNamesFeatureCode.PPL;
			londonCA.OpenStreetMapPlaceCategory	= OpenStreetMapPlaceCategory.City;
			londonCA.Population					= 346765;
			londonCA.CountryCode				= "CA";
			londonCA.TimeZone					= -6.0;

			Localities.Add("LondonCA", londonCA);
			// -------------------------------------------

			GeoLocality londonderry				= new GeoLocality();
			londonderry.Id						= 3;
			londonderry.OpenStreetMapRelationId	= 267762522;
			londonderry.Name					= "Londonderry";
			londonderry.Point					= new GeoPoint(54.9978678, -7.3213056);
			londonderry.GeonamesId				= 2643736;
			londonderry.CountryCode				= "GB";
			londonderry.GeoNamesFeatureClass	= GeoNamesFeatureClass.P;
			londonderry.GeoNamesFeatureCode		= GeoNamesFeatureCode.PPL;
			londonderry.WikiDataId				= "Q192208";
			londonderry.OpenStreetMapPlaceCategory		= OpenStreetMapPlaceCategory.City;
			londonderry.Population				= 83652;
			londonderry.TimeZone				= 0.0;

			Localities.Add("Londonderry", londonderry);
		}
	}
}
