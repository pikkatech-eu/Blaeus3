/***********************************************************************************
* File:         GeonameCityClass.cs                                                *
* Contents:     Enum GeonameCityClass                                              *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 00:06                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Geonames' city classes for the limitations in queries.
	/// </summary>
	public enum GeonamesCityClass
	{
		/// <summary>
		/// Cities parameter will be ignored
		/// </summary>
		None,

		/// <summary>
		/// Will take places only with population >= 1000
		/// </summary>
		Cities1000,

		/// <summary>
		/// Will take places only with population >= 5000
		/// </summary>
		Cities5000,

		/// <summary>
		/// Will take places only with population >= 15000
		/// </summary>
		Cities15000
	}
}
