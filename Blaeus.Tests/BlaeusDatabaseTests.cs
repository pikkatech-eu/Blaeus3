using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;
using Blaeus.Library.Storage;
using Blaeus.Tests;

namespace Blaues2.Tests
{
	[TestFixture]
	public class BlaeusDatabaseTests
	{
		private string _databaseFileName = ".\\Data\\test_blaeus.db3";
		private SqliteBlaeusDatabase _database = null;

		[SetUp]
		public void PrepareTurf()
		{
			Thread.Sleep(500);
			if (File.Exists(this._databaseFileName))
			{
				File.Delete(this._databaseFileName);
			}

			Thread.Sleep(500);
			this._database = new SqliteBlaeusDatabase();
			this._database.Open(this._databaseFileName);
			Thread.Sleep(500);
		}

		[Test]
		public void InsertionOfGeolocality_ValidLocation_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];
			
			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocality(londonUK.Id);

			Assert.That(locality != null);
		}

		[Test]
		public void ReplacementOfGeolocality_NoAlternativeNames_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			string newDescription = "This is a repeated description of the city of London, UK";
			londonUK.Description = newDescription;

			this._database.ReplaceGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocality(londonUK.Id);

			Assert.That(locality != null);

			Assert.That(locality.Description == newDescription);
		}

		[Test]
		public void ReplacementOfGeolocality_WithAlternativeNames_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			string newName	= "C'est la vie";
			londonUK.AlternativeNames["fr"] = newName;

			this._database.ReplaceGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocality(londonUK.Id);

			Assert.That(locality != null);

			Assert.That(locality.AlternativeNames["fr"] == newName);
		}

		[Test]
		public void ReplacementOfGeolocality_WithHistoricNames_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			string newName	= "Lorem Ipsum, ergo sum";
			HistoricName historic = londonUK.HistoricNames.First(h=>h.Key == "la");
			historic.Name	= newName;

			this._database.ReplaceGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocality(londonUK.Id);

			Assert.That(locality != null);

			HistoricName historic2 = locality.HistoricNames.First(h=>h.Key == "la");

			Assert.That(historic2.Name == newName);
		}

		[Test]
		/// <summary>
		/// Using a Geolocality with a non-existent ID, e.g.
		/// </summary>
		public void ReplacementOfGeolocality_InvalidGeolocality_DoesNothing()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			londonUK.Id += 1;

			this._database.ReplaceGeoLocality(londonUK);
		}

		[Test]
		public void DeletionOfGeolocality_ValidID_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);
			this._database.DeleteGeoLocality(londonUK.Id);

			GeoLocality locality = this._database.SelectGeoLocality(londonUK.Id);

			Assert.That(locality == null);
		}

		[Test]
		public void DeletionOfGeolocality_InvalidID_DoesNothing()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);
			this._database.DeleteGeoLocality(londonUK.Id + 1);

			GeoLocality locality = this._database.SelectGeoLocality(londonUK.Id);

			Assert.That(locality != null);
		}

		[Test]
		public void SelectionOfGeolocalityByGeonamesID_ValidGeonamesID_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocalityByGeonamesId((int)londonUK.GeonamesId);

			Assert.That(locality != null);
		}

		[Test]
		public void SelectionOfGeolocalityByGeonamesID_InvalidGeonamesID_ReturnsNull()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocalityByGeonamesId((int)londonUK.GeonamesId + 1);

			Assert.That(locality == null);
		}

		[Test]
		public void SelectionOfGeolocalityByOsmRelationID_ValidOsmRelationID_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocalityByOpenStreetMapId((int)londonUK.OpenStreetMapRelationId);

			Assert.That(locality != null);
		}

		[Test]
		public void SelectionOfGeolocalityByOsmRelationID_InvalidOsmRelationID_ReturnsNull()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocalityByOpenStreetMapId((int)londonUK.OpenStreetMapRelationId + 1);

			Assert.That(locality == null);
		}

		[Test]
		public void SelectionOfGeolocalityByWikidataID_ValidWikidataID_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocalityByWikidataId(londonUK.WikiDataId);

			Assert.That(locality != null);
		}

		[Test]
		public void SelectionOfGeolocalityByWikidataID_InvalidWikidataID_ReturnsNull()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];

			this._database.InsertGeoLocality(londonUK);

			GeoLocality locality = this._database.SelectGeoLocalityByWikidataId("Q100");

			Assert.That(locality == null);
		}

		[Test]
		public void SelectionOfGeolocalitiesByPhoneticSimilarity_UsingLike_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];
			GeoLocality londonCA = MockGeolocalityCollection.Localities["LondonCA"];

			this._database.InsertGeoLocality(londonUK);
			this._database.InsertGeoLocality(londonCA);

			GeoLocality[] localities = this._database.SelectByNameLike("Lon", StringSelectorCriterion.ProbeIn);

			Assert.That(localities.Length == 2);
		}

		[Test]
		public void SelectionOfGeolocalitiesByPhoneticSimilarity_UsingInstr_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];
			GeoLocality londonCA = MockGeolocalityCollection.Localities["LondonCA"];

			this._database.InsertGeoLocality(londonUK);
			this._database.InsertGeoLocality(londonCA);

			GeoLocality[] localities = this._database.SelectByNameLike("aLondonia", StringSelectorCriterion.ProbeContains);

			Assert.That(localities.Length == 2);
		}

		[Test]
		public void SelectionOfGeolocalitiesByPhoneticSimilarity_UsingSoundex_Succeeds()
		{
			Assert.Fail("To be implemented");
		}

		[Test]
		public void SelectionOfGeolocalitiesByPhoneticSimilarity_UsingDaimok_Succeeds()
		{
			GeoLocality cormeiiles	= new GeoLocality();
			cormeiiles.Name			= "Cormeilles";

			GeoLocality kormeiiles	= new GeoLocality();
			kormeiiles.Name			= "Kormeilles";

			this._database.InsertGeoLocality(cormeiiles);
			this._database.InsertGeoLocality(kormeiiles);

			GeoLocality[] localities = this._database.SelectByNameLike("Cormeilles", StringSelectorCriterion.Daimok);
		}

		[Test]
		public void SelectionOfGeolocalitiesByPhoneticSimilarity_UsingCosinus_Succeeds()
		{
			GeoLocality londonUK = MockGeolocalityCollection.Localities["LondonUK"];
			GeoLocality londonCA = MockGeolocalityCollection.Localities["LondonCA"];
			GeoLocality londonderry = MockGeolocalityCollection.Localities["Londonderry"];

			this._database.InsertGeoLocality(londonUK);
			this._database.InsertGeoLocality(londonCA);
			this._database.InsertGeoLocality(londonderry);

			GeoLocality[] localities = this._database.SelectByNameLike("derry", StringSelectorCriterion.Cosine);
		}
	}
}
