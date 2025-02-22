using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;
using Blaeus.Library.Gazetteers;

namespace Blaeus.Tests
{
	[TestFixture]
	public class GeonamesGazetteerTests
	{
		private GeonamesGazetteer _gazetteer = new GeonamesGazetteer();

		[Test]
		public void QueryingExistingToponym_Suceeds()
		{
			string toponym = "London";

			GeonamesGazetteer.UserName = "AlexKonnen";
			GeoLocality[] localities = this._gazetteer.Lookup(toponym);

			Assert.That(localities.Length > 10);
		}

		[Test]
		public void QueryingStupidToponym_ReturnsEmptyArray()
		{
			string toponym = "xZxöä234l";

			GeonamesGazetteer.UserName = "AlexKonnen";
			GeoLocality[] localities = this._gazetteer.Lookup(toponym);

			Assert.That(localities.Length == 0);
		}

		[Test]
		public void QueryingExistingToponym_CitiesRestriction_Suceeds()
		{
			string toponym = "London";

			GeonamesGazetteer.UserName = "AlexKonnen";
			GeonamesGazetteer.CityClass	= GeonamesCityClass.Cities15000;

			GeoLocality[] localities = this._gazetteer.Lookup(toponym);

			Assert.That(localities.Length > 10);
		}

		[Test]
		public void GettingLocalityById_ValidId_Succeeds()
		{
			int geonamesId = 2643743;
			GeonamesGazetteer.UserName		= "AlexKonnen";
			GeonamesGazetteer.QueryStyle	= GeonamesQueryStyle.Full;

			GeoLocality locality	= this._gazetteer.GetLocality(geonamesId.ToString());

			Assert.That(locality != null);
		}

		[Test]
		public void GettingLocalityById_InavlidId_ReturnsNull()
		{
			int geonamesId = 420000000;
			GeonamesGazetteer.UserName		= "AlexKonnen";
			GeonamesGazetteer.QueryStyle	= GeonamesQueryStyle.Full;

			GeoLocality locality			= this._gazetteer.GetLocality(geonamesId.ToString());

			Assert.That(locality == null);
		}
	}
}
