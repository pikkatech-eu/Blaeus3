/***********************************************************************************
* File:         IGeoNamesDatabase.cs                                               *
* Contents:     Interface IGeoNamesDatabase                                        *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-14 10:13                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Domain;

namespace Blaeus.Library.Storage.Geonames
{
	public interface IGeoNamesDatabase
	{
		/// <summary>
		/// Opens the database.
		/// </summary>
		/// <param name="source">The database source, i.o.w. the path to the database file (for Sqlite).</param>
		void Open(string source);

		/// <summary>
		/// Selects a locality by its Geonames ID.
		/// </summary>
		/// <param name="geonamesId">The Geonames ID to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		GeoLocality Select(int geonamesId);

		/// <summary>
		/// Selects localities using the SQL WHERE clause.
		/// E.g. to select all localities with the names including "aris" the value would be " `name` LIKE '%aris%'".
		/// </summary>
		/// <param name="sqlWhereCriterion">The fragment of the WHERE clause to select by, see above.</param>
		/// <param name="limit">Optional limit. If set to 0, is ignored.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		GeoLocality[] Select(string sqlWhereCriterion, int limit = 0);

		/// <summary>
		/// Selects localities by name likeness.
		/// </summary>
		/// <param name="nameToken">The name token to be contained in the localiti nymes.</param>
		/// <param name="minPopulation">Minimum population to filter by.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		GeoLocality[] SelectByNameLike(string nameToken, int minPopulation = 0);
	}
}
