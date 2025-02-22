/***********************************************************************************
* File:         GeoNamesFeatureClass.cs                                            *
* Contents:     Enum GeoNamesFeatureClass                                          *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-25 22:36                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Source: https://www.geonames.org/export/codes.html
	/// </summary>
	public enum GeoNamesFeatureClass
	{
		A, // country, state, region,...
		H, // stream, lake, ...
		L, // parks,area, ...
		P, // city, village,...
		R, // road, railroad 
		S, // spot, building, farm
		T, // mountain,hill,rock,... 
		U, // undersea
		V, // forest,heath,...
		X  // Unknown
	}
}
