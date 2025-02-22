/***********************************************************************************
* File:         GeoLocality.cs                                                     *
* Contents:     Class GeoLocality                                                  *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-12 18:27                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain.Enumerations;
using Factotum.Xml;

[assembly: InternalsVisibleTo("Blaues.Tests")]

namespace Blaeus.Library.Domain
{
	public class GeoLocality
	{
		#region Properties
		/// <summary>
		/// Internal ID within the Blaeus system.
		/// </summary>
		public int Id											{get;set;}

		/// <summary>
		/// Geonames ID, if known, otherwise null.
		/// </summary>
		public int? GeonamesId									{get;set;} = null;

		/// <summary>
		/// OSM ID, if known, otherwise null.
		/// </summary>
		public int? OpenStreetMapNodeId							{get;set;} = null;

		/// <summary>
		/// OSM Relation ID, if known, otherwise null (more useful as the former one).
		/// </summary>
		public int? OpenStreetMapRelationId						{get;set;} = null;

		/// <summary>
		/// WikiData ID, if known, otherwise  empty string.
		/// </summary>
		public string WikiDataId								{get;set;} = "";

		/// <summary>
		/// The locatity's name as written with Latin characters (inclusively diacritics).
		/// </summary>
		/// <example>"Tehran" (spelling follows GeoNames.)</example>
		public string Name										{get;set;} = "";

		/// An optional description of the locality.
		/// </summary>
		public string Description								{get;set;}	= "";

		public GeoPoint	Point									{get;set;} = new GeoPoint();

		/// <summary>
		/// The elevation of the locality, m a.s.l.
		/// </summary>
		public double Elevation									{get; set;}

		/// <summary>
		/// The population of the place, if available.
		/// </summary>
		public int Population									{get; set;}

		/// <summary>
		/// The Alpha-2 country code, lower case.
		/// </summary>
		public string CountryCode								{get;set;}	= "";

		/// <summary>
		/// Time Zone, without taking into consideration the Day Saving Times.
		/// </summary>
		public double TimeZone									{get;set;}	= 0;

		/// <summary>
		/// GeoNamesFeatureClass, if relevant, otherwise remains unknown.
		/// </summary>
		public GeoNamesFeatureClass GeoNamesFeatureClass		{get; set;} = GeoNamesFeatureClass.X;

		/// <summary>
		/// GeoNamesFeatureCode, if relevant, otherwise remains unknown.
		/// </summary>
		public GeoNamesFeatureCode GeoNamesFeatureCode			{get; set;} = GeoNamesFeatureCode.NONE;

		/// <summary>
		/// OsmPlaceCategory, if relevant, otherwise remains unknown.
		/// </summary>
		public OpenStreetMapPlaceCategory OpenStreetMapPlaceCategory	{get; set;}	= OpenStreetMapPlaceCategory.Unknown;

		/// <summary>
		/// The bounding box of the locality (optional, if available).
		/// </summary>
		public GeoRectangle BoundingBox							{get;set;}	= null;

		/// <summary>
		/// The polygon of the locality (optional, if available).
		/// </summary>
		public GeoPolygon Polygon								{get;set;}	= null;

		/// <summary>
		/// Dictionary of alternative names.
		/// Key: ISO 639-2, 2-letter code of a language, lower case.
		/// </summary>
		/// <example>{"fa", "طهران"}</example>
		public Dictionary<string, string>	AlternativeNames	{get;internal set;} = new Dictionary<string, string>();

		public List<HistoricName>			HistoricNames		{get;internal set;}	= new List<HistoricName>();
		#endregion

		#region Validation
		/// <summary>
		/// Validation concept.
		/// A GeoLocality is valid, if its ID is greater than zero and its name is not emopty.
		/// </summary>
		/// <returns>True, if the validation concept holds.</returns>
		public bool IsValid()
		{
			return !String.IsNullOrEmpty(this.Name);
		}
		#endregion

		#region XML
		public XElement ToXElement()
		{
			XElement x = new XElement("GeoLocality");

			x.AppendAttribute("Id", this.Id);
			x.AppendAttribute("GeonamesId", this.GeonamesId);
			x.AppendAttribute("OpenStreetMapNodeId", this.OpenStreetMapNodeId);
			x.AppendAttribute("OpenStreetMapRelationId", this.OpenStreetMapRelationId);
			x.AppendAttribute("WikiDataId", this.WikiDataId);

			x.AppendAttribute("Name", this.Name);

			XElement xAlternativeNames = new XElement("AlternativeNames");
			x.Add(xAlternativeNames);

			foreach (string key in this.AlternativeNames.Keys)
			{
				XElement xAlternativeName = new XElement("Name");
				xAlternativeName.AppendAttribute("Code", key);
				xAlternativeName.AppendAttribute("Value", this.AlternativeNames[key]);
				xAlternativeNames.Add(xAlternativeName);
			}

			x.AppendElement("Description", this.Description);

			if (this.Point != null)
			{
				x.Add(this.Point.ToXElement().Rename("Coordinates"));
			}

			x.AppendElement("Elevation", this.Elevation);

			x.AppendElement("GeoNamesFeatureCode", this.GeoNamesFeatureCode);
			x.AppendElement("OpenStreetMapPlaceCategory", this.OpenStreetMapPlaceCategory);
			x.AppendElement("Population", this.Population);
			x.AppendElement("CountryCode", this.CountryCode);

			if (this.BoundingBox != null)
			{
				x.Add(this.BoundingBox.ToXElement().Rename("BoundingBox"));
			}
			
			if (this.Polygon != null)
			{
				x.Add(this.Polygon.ToXElement().Rename("Polygon"));
			}

			return x;
		}

		public static GeoLocality FromXElement(XElement x)
		{
			GeoLocality gl				= new GeoLocality();

			gl.Id						= x.AttributeValue<int>("Id");
			gl.GeonamesId				= x.AttributeValueNullable<int>("GeonamesId");
			gl.OpenStreetMapNodeId			= x.AttributeValueNullable<int>("OpenStreetMapNodeId");
			gl.OpenStreetMapRelationId	= x.AttributeValueNullable<int>("OpenStreetMapRelationId");
			gl.WikiDataId				= x.AttributeValue<string>("WikiDataId");
			gl.Name						= x.AttributeValue<string>("Name");

			XElement xAlternativeNames	= x.Element("AlternativeNames");

			if (xAlternativeNames != null)
			{
				foreach (XElement xName in xAlternativeNames.Elements("Name"))
				{
					string code = xName.AttributeValue<string>("Code");
					string name = xName.AttributeValue<string>("Name");

					gl.AlternativeNames.Add(code, name);
				}
			}

			gl.Description				= x.ElementValue<string>("Description");

			XElement xCoordinates		= x.Element("Coordinates");

			if (xCoordinates != null)
			{
				gl.Point = GeoPoint.FromXElement(xCoordinates);
			}

			gl.Elevation				= x.ElementValue<int>("Elevation");

			gl.GeoNamesFeatureCode		= (GeoNamesFeatureCode)x.ElementEnum(typeof(GeoNamesFeatureCode), "GeoNamesFeatureCode", GeoNamesFeatureCode.NONE);
			gl.OpenStreetMapPlaceCategory			= (OpenStreetMapPlaceCategory)x.ElementEnum(typeof(OpenStreetMapPlaceCategory), "OpenStreetMapPlaceCategory", OpenStreetMapPlaceCategory.Unknown);

			gl.Population				= x.ElementValue<int>("Population");

			gl.CountryCode				= x.ElementValue<string>("CountryCode");

			XElement xBoundingBox		= x.Element("BoundingBox");

			if (xBoundingBox != null)
			{
				gl.BoundingBox			= GeoRectangle.FromXElement(xBoundingBox);
			}

			XElement xPolygon			= x.Element("Polygon");

			if (xPolygon != null)
			{
				gl.Polygon = GeoPolygon.FromXElement(xPolygon);
			}

			return gl;
		}
		#endregion

		#region String Representation
		public override string ToString()
		{
			return $"[{this.Id}] (GN:{this.GeonamesId}, OSM:{this.OpenStreetMapRelationId}, WD:{this.WikiDataId}): {this.Name}, {this.CountryCode}, {this.Point}. {this.GeoNamesFeatureCode}, P={this.Population}";
		}
		#endregion
	}
}
