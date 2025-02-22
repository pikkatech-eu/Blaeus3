/***********************************************************************************
* File:         HistoricName.cs                                                    *
* Contents:     Class HistoricName                                                 *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-26 12:07                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain
{
	/// <summary>
	/// Specify a historic name of a GeoLocality.
	/// E.g. the city of Lviv was called Lwów in Polish from 1918 to 1939 and Lemberg in German from 1772 to 1918.
	/// </summary>
	public class HistoricName
	{
		#region Properties
		/// <summary>
		/// The key of the instance, normally the 2-character ISO code of the language.
		/// </summary>
		/// <example>"pl" for the Polish language.</example>
		public string	Key		{get;set;}	= "";

		/// <summary>
		/// The name of the place.
		/// </summary>
		/// <example>"Lwów"</example>
		public string	Name	{get;set;} = "";

		/// <summary>
		/// Gregorian year, the beginning of the time interval in which the place held the historic name.
		/// If set to null, the beginning of the interval is not known.
		/// </summary>
		/// <example>1918</example>
		public int?		From	{get;set;} = null;

		/// <summary>
		/// Gregorian year, the end of the time interval in which the place held the historic name.
		/// If set to null, the end of the interval is not known.
		/// </summary>
		/// <example>1939</example>
		public int?		To		{get;set;} = null;

		/// <summary>
		/// The source of the information, e.g. a link to a Wikipedia article.
		/// </summary>
		/// <example>https://www.wikidata.org/wiki/Q36036</example>
		public string	Source	{get;set;} = "";
		#endregion
	}
}
