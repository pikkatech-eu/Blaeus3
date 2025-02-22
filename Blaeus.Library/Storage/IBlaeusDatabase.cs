/***********************************************************************************
* File:         IBlaeusDatabase.cs                                                 *
* Contents:     Interface IBlaeusDatabase                                          *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-13 16:59                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;

namespace Blaeus.Library.Storage
{
	public interface IBlaeusDatabase
	{
		/// <summary>
		/// Opens the database.
		/// </summary>
		/// <param name="source">The database source, i.o.w. the path to the database file (for Sqlite).</param>
		void Open(string source);

		/// <summary>
		/// Inserts a geolocality.
		/// </summary>
		/// <param name="locality">Geolocality to insert.</param>
		int InsertGeoLocality(GeoLocality locality);

		/// <summary>
		/// Updates a geolocality by replacig its data.
		/// </summary>
		/// <param name="locality">Geolocality to update. The geolocality Id in the instance must be present in the database.</param>
		void ReplaceGeoLocality(GeoLocality locality);

		/// <summary>
		/// Deletes a geolocality.
		/// </summary>
		/// <param name="id">The Id of the locality to delete.</param>
		void DeleteGeoLocality(int id);

		/// <summary>
		/// Selects a locality by its ID in Blaeus system.
		/// </summary>
		/// <param name="id">The id to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		GeoLocality SelectGeoLocality(int id);

		/// <summary>
		/// Selects a locality by its Geonames ID.
		/// </summary>
		/// <param name="geonamesId">The Geonames ID to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		GeoLocality SelectGeoLocalityByGeonamesId(int geonamesId);

		/// <summary>
		/// Selects a locality by its OSM ID.
		/// </summary>
		/// <param name="osmId">The OSM ID to select by.</param>
		/// <param name="useRelationId">If true, the OSM relation Id will be taken</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		GeoLocality SelectGeoLocalityByOpenStreetMapId(int osmId, bool useRelationId = true);

		/// <summary>
		/// Selects a locality by its Wikidata ID.
		/// </summary>
		/// <param name="wikidataId">The Wikidata ID to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		GeoLocality SelectGeoLocalityByWikidataId(string wikidataId);

		/// <summary>
		/// Selects localities using the SQL WHERE clause.
		/// E.g. to select all localities with the names including "aris" the value would be " `name` LIKE '%aris%'".
		/// </summary>
		/// <param name="sqlWhereCriterion">The fragment of the WHERE clause to select by, see above.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		GeoLocality[] SelectGeoLocalities(string sqlWhereCriterion);

		/// <summary>
		/// Selects localities by name likeness.
		/// </summary>
		/// <param name="nameToken">The name token to be contained in the locality names.</param>
		/// <param name="comparison">Method to compare toponyms.</param>
		/// <param name="minPopulation">Minimum population to filter by.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		GeoLocality[] SelectByNameLike(string nameToken, StringSelectorCriterion comparison, int minPopulation = 0);

		/// <summary>
		/// Selects localities lying within a bounding box.
		/// </summary>
		/// <param name="box">The bounding box to find locaties within.</param>
		/// <param name="minPopulation">Minimum population to filter by.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		GeoLocality[] SelectByBoundingBox(GeoRectangle box, int minPopulation = 0);

		/// <summary>
		/// Selects the upper portion of geolocations.
		/// </summary>
		/// <param name="limit">The number of localities to select.</param>
		/// <returns>Array of localities selected .</returns>
		GeoLocality[] SelectGeolocalities(int limit);
	}
}
