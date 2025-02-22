/***********************************************************************************
* File:         WikidataGazetterTests.cs                                           *
* Contents:     Class WikidataGazetterTests                                        *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 16:17                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Domain;
using Blaeus.Library.Gazetteers;

namespace Blaeus.Tests
{
	[TestFixture]
	public class WikidataGazetterTests
	{
		private WikidataGazetteer _gazetteer = new WikidataGazetteer();

		[Test]
		public void QueryById_ValidId_Succeeds()
		{
			string id = "Q36036";
			GeoLocality locality = this._gazetteer.GetLocality(id);

			Assert.That(locality != null);
		}

		[Test]
		public void QueryForWikidataIdBaGeonamesID_ValidGeonamesId_Succeeds()
		{
			int geonamesId = 2643743;

			string wikidataId = this._gazetteer.GetWikidataIdByGeonamesId(geonamesId);

			Assert.That(wikidataId != null);
		}

		[Test]
		public void QueryForWikidataIdBaGeonamesID_InvalidGeonamesId_ReturnsNull()
		{
			int geonamesId = 264374300;

			string wikidataId = this._gazetteer.GetWikidataIdByGeonamesId(geonamesId);

			Assert.That(wikidataId == null);
		}
	}
}
