/***********************************************************************************
* File:         OsmPlaceCategory.cs                                                *
* Contents:     Enum OsmPlaceCategory                                              *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-13 14:58                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// A place, as defined by the OpenStreetMap (OSM) system (https://wiki.openstreetmap.org/wiki/Key:place).
	/// Also defined as a boundary relation describing the extent of an administrative, 
	/// political, or otherwise demarcated area. (https://wiki.openstreetmap.org/wiki/Key:boundary)
	/// </summary>
	public enum OpenStreetMapPlaceCategory
	{
		Continent,					// One of the seven continents: Africa, Antarctica, Asia, Europe, North America, Oceania, South America.
		Country,					// A nation state or other high-level national political/administrative area.
		Region,						// Broad tag for geographic or historical areas with no clear boundary.
		Province,					// A subdivision of a country similar to a state.
		State,						// A large sub-national administrative area.
		County,						// A geographical region of a country.
		District,					// An administrative division managed by local government.
		Archipelago,				// A named group of islands.
		Island,						// A piece of land completely surrounded by water.
		City,						// Large urban settlement.
		Town,						// An centre, between a village and a city in size.
		Village,					// A small settlement, smaller than a town with few facilities.
		Municipality,				// An urban administrative division with corporate status.
		PopulatedPlace,				// A populated place, either existing, or historical, of unknown size and level.
		AdministrativeEntity,		// An undifferentiated administrative division of a country.
		PoliticalEntity,			// An undifferentiated geopolitical entity.
		Other,						// An entity that is not relevant in context.
		Unknown						// Feature is unknown.
	};
}
