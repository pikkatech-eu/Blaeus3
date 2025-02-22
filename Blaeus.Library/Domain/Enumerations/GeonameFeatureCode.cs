/***********************************************************************************
* File:         GeonameFeatureCode.cs                                              *
* Contents:     Enum GeoNamesFeatureCode                                           *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-13 15:02                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Source: https://www.geonames.org/export/codes.html
	/// </summary>
	public enum GeoNamesFeatureCode
	{
		PPL,		// populated place	a city, town, village, or other agglomeration of buildings where people live and work
		PPLA,		// seat of a first-order administrative division
		PPLA2,		// seat of a second-order administrative division	
		PPLA3,		// seat of a third-order administrative division	
		PPLA4,		// seat of a fourth-order administrative division	
		PPLA5,		// seat of a fifth-order administrative division	
		PPLC,		// capital of a political entity	
		PPLCH,		// historical capital of a political entity	a former capital of a political entity
		PPLF,		// farm village	a populated place where the population is largely engaged in agricultural activities
		PPLG,		// seat of government of a political entity	
		PPLH,		// historical populated place	a populated place that no longer exists
		PPLL,		// populated locality	an area similar to a locality but with a small group of dwellings or other buildings
		PPLQ,		// abandoned populated place	
		PPLR,		// religious populated place	a populated place whose population is largely engaged in religious occupations
		PPLS,		// populated places	cities, towns, villages, or other agglomerations of buildings where people live and work
		PPLW,		// destroyed populated place	a village, town or city destroyed by a natural disaster, or by war
		PPLX,		// section of populated place	
		STLMT,		// israeli settlement

		ADM1,		// First-order administrative division	a primary administrative division of a country, such as a state in the United States
		ADM1H,		// Historical first-order administrative division	a former first-order administrative division
		ADM2,		// Second-order administrative division	a subdivision of a first-order administrative division
		ADM2H,		// Historical second-order administrative division	a former second-order administrative division
		ADM3,		// Third-order administrative division	a subdivision of a second-order administrative division
		ADM3H,		// Historical third-order administrative division	a former third-order administrative division
		ADM4,		// Fourth-order administrative division	a subdivision of a third-order administrative division
		ADM4H,		// Historical fourth-order administrative division	a former fourth-order administrative division
		ADM5,		// Fifth-order administrative division	a subdivision of a fourth-order administrative division
		ADM5H,		// Historical fifth-order administrative division	a former fifth-order administrative division
		ADMD,		// Administrative division	an administrative division of a country, undifferentiated as to administrative level
		ADMDH,		// Historical administrative division 	a former administrative division of a political entity, undifferentiated as to administrative level
		LTER,		// Leased area	a tract of land leased to another country, usually for military installations
		PCL,		// Political entity
		PCLD,		// Dependent political entity
		PCLF,		// Freely associated state
		PCLH,		// Historical political entity	a former political entity
		PCLI,		// Independent political entity
		PCLIX,		// Section of independent political entity
		PCLS,		// Semi-independent political entity
		PRSH,		// Parish	an ecclesiastical district
		TERR,		// Territory
		ZN,			// Zone
		ZNB,		// Buffer zone	a zone recognized as a buffer between two nations in which military presence is minimal or absent

		AGRC,		// Agricultural colony	a tract of land set aside for agricultural settlement
		AMUS,		// Amusement park	Amusement Park are theme parks, adventure parks offering entertainment, similar to funfairs but with a fix location
		AREA,		// Area	a tract of land without homogeneous character or boundaries
		BSND,		// Drainage basin	an area drained by a stream
		BSNP,		// Petroleum basin	an area underlain by an oil-rich structural basin
		BTL,		// Battlefield	a site of a land battle of historical importance
		CLG,		// Clearing	an area in a forest with trees removed
		CMN,		// Common	a park or pasture for community use
		CNS,		// Concession area	a lease of land by a government for economic development, e.g., mining, forestry
		COLF,		// Coalfield	a region in which coal deposits of possible economic value occur
		CONT,		// Continent	continent: Europe, Africa, Asia, North America, South America, Oceania, Antarctica
		CST,		// Coast	a zone of variable width straddling the shoreline
		CTRB,		// Business center	a place where a number of businesses are located
		DEVH,		// Housing development	a tract of land on which many houses of similar design are built according to a development plan
		FLD,		// Field(s)	an open as opposed to wooded area
		FLDI,		// Irrigated field(s)	a tract of level or terraced land which is irrigated
		GASF,		// Gasfield	an area containing a subterranean store of natural gas of economic value
		GRAZ,		// Grazing area	an area of grasses and shrubs used for grazing
		GVL,		// Gravel area	an area covered with gravel
		INDS,		// Industrial area	an area characterized by industrial activity
		LAND,		// Arctic land	a tract of land in the Arctic
		LCTY,		// Locality	a minor area or place of unspecified or mixed character and indefinite boundaries
		MILB,		// Military base	a place used by an army or other armed service for storing arms and supplies, and for accommodating and training troops, a base from which operations can be initiated
		MNA,		// Mining area	an area of mine sites where minerals and ores are extracted
		MVA,		// Maneuver area	a tract of land where military field exercises are carried out
		NVB,		// Naval base	an area used to store supplies, provide barracks for troops and naval personnel, a port for naval vessels, and from which operations are initiated
		OAS,		// Oasis(-es)	an area in a desert made productive by the availability of water
		OILF,		// Oilfield	an area containing a subterranean store of petroleum of economic value
		PEAT,		// Peat cutting area	an area where peat is harvested
		PRK,		// Park	an area, often of forested land, maintained as a place of beauty, or for recreation
		PRT,		// Port	a place provided with terminal and transfer facilities for loading and discharging waterborne cargo or passengers, usually located in a harbor
		QCKS,		// Quicksand	an area where loose sand with water moving through it may become unstable when heavy objects are placed at the surface, causing them to sink
		RES,		// Reserve	a tract of public land reserved for future use or restricted as to use
		RESA,		// Agricultural reserve	a tract of land reserved for agricultural reclamation and/or development
		RESF,		// Forest reserve	a forested area set aside for preservation or controlled use
		RESH,		// Hunting reserve	a tract of land used primarily for hunting
		RESN,		// Nature reserve	an area reserved for the maintenance of a natural habitat
		RESP,		// Palm tree reserve	an area of palm trees where use is controlled
		RESV,		// Reservation	a tract of land set aside for aboriginal, tribal, or native populations
		RESW,		// Wildlife reserve	a tract of public land reserved for the preservation of wildlife
		RGN,		// Region	an area distinguished by one or more observable physical or cultural characteristics
		RGNE,		// Economic region	a region of a country established for economic development or for statistical purposes
		RGNH,		// Historical region	a former historic area distinguished by one or more observable physical or cultural characteristics
		RGNL,		// Lake region	a tract of land distinguished by numerous lakes
		RNGA,		// Artillery range	a tract of land used for artillery firing practice
		SALT,		// Salt area	a shallow basin or flat where salt accumulates after periodic inundation
		SNOW,		// Snowfield	an area of permanent snow and ice forming the accumulation area of a glacier
		TRB,		// Tribal area	a tract of land used by nomadic or other tribes

		NONE		// unknown
	}
}
