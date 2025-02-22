/***********************************************************************************
* File:         OpenStreetMapGazetteerTests.cs                                     *
* Contents:     Class OpenStreetMapGazetteerTests                                  *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 14:47                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Domain;
using Blaeus.Library.Gazetteers;

namespace Blaeus.Tests
{
	[TestFixture]
	public class OpenStreetMapGazetteerTests
	{
		private OpenStreetMapGazetteer _gazetteer = new OpenStreetMapGazetteer();

		[Test]
		public void QueryingExistingToponym_NoParameters_Suceeds()
		{
			string toponym = "London";
			OpenStreetMapGazetteer.PolygonThreshold		= 0.05;
			GeoLocality[] localities = this._gazetteer.Lookup(toponym);

			Assert.That(localities.Length > 10);
		}

		[Test]
		public void QueryingExistingToponym_IncludeNameDetails_Suceeds()
		{
			string toponym = "London";

			OpenStreetMapGazetteer.IncludeNameDetails	= true;
			OpenStreetMapGazetteer.PolygonThreshold		= 0.05;

			GeoLocality[] localities = this._gazetteer.Lookup(toponym);

			Assert.That(localities.Length > 10);
		}

		[Test]
		public void QueryById_ValidId_Succeeds()
		{
			string id = "2032280";
			GeoLocality locality = this._gazetteer.GetLocality(id);
		}

		[Test]
		public void QueryById_InvalidId_ReturnsEmptyArray()
		{
			string id = "1";
			GeoLocality locality = this._gazetteer.GetLocality(id);

			Assert.That(locality == null);
		}
	}
}
