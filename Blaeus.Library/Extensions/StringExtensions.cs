/***********************************************************************************
* File:         StringExtensions.cs                                                *
* Contents:     Class StringExtensions                                             *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-01-28 13:03                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Replaces entries of "'" to "''" to use in databse operations.
		/// </summary>
		/// <param name="source">Source string, possibly woth single apostrophes.</param>
		/// <returns>String with single apostrophes replaced.</returns>
		public static string ToDatabaseString(this string source)
		{
			return source.Replace("'", "''");
		}
	}
}
