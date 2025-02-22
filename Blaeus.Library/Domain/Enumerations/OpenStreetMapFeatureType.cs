/***********************************************************************************
* File:         OsmFeatureType.cs                                                  *
* Contents:     Enum OsmFeatureType                                                *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 14:20                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// OpenStreetMap feature type.
	/// </summary>
	public enum OpenStreetMapFeatureType
	{
		Country,
		State,
		City,

		/// <summary>
		/// Selects any human inhabited feature from 'state' down to 'neighbourhood'.
		/// </summary>
		Settlement,
		Undefined
	}
}
