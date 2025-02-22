/***********************************************************************************
* File:         GeonamesResponseOrder.cs                                           *
* Contents:     Enum GeonamesResponseOrder                                         *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 00:15                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Geonames response sorting order
	/// </summary>
	public enum GeonamesResponseOrder
	{
		None,
		Population,
		Elevation,	// less useful
		Relevance
	}
}
