/***********************************************************************************
* File:         IGazetteer.cs                                                      *
* Contents:     Interface IGazetteer                                               *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-09-12 11:49                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Domain;

namespace Blaeus.Library.Gazetteers
{
	/// <summary>
	/// Defines functionalöity common to all gazetteers.
	/// </summary>
	public interface IGazetteer
	{
		/// <summary>
		/// Performs lookup for localities based on a toponym.
		/// </summary>
		/// <param name="toponym">The toponym to look for.</param>
		/// <returns>Array of Geolocalities found.</returns>
		GeoLocality[] Lookup(string toponym);

		/// <summary>
		/// Gets a locality by its ID in the corresponding system.
		/// </summary>
		/// <param name="id">The ID as string (converted to string if the ID is an integer, as in Geonames).</param>
		/// <returns>The value of GeoLocality, if found, otherwise null.</returns>
		GeoLocality GetLocality(string id);
	}
}
