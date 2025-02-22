/***********************************************************************************
* File:         OsmPolygonOutputType.cs                                            *
* Contents:     Enum OsmPolygonOutputType                                          *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 14:23                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Type of polygon output in OpenStreetMap.
	/// </summary>
	public enum OpenStreetMapPolygonOutputType
	{
		Polygon_Geojson,
		Polygon_Kml,
		Polygon_Svg,
		Polygon_Text,
		None
	}
}
