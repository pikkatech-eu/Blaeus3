/***********************************************************************************
* File:         OsmOutputFormat.cs                                                 *
* Contents:     Enum OsmOutputFormat                                               *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 14:17                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Type of OpenStreetMap output format.
	/// </summary>
	public enum OpenStreetMapOutputFormat
	{
		Xml,
		Json,
		JsonV2,
		GeoJson,
		GeoCodeJson
	}
}
