/***********************************************************************************
* File:         NameValueCollectionRxtensions.cs                                   *
* Contents:     Class NameValueCollectionRxtensions                                *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-16 12:53                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Collections.Specialized;

namespace Blaeus.Library.Extensions
{
	public static class NameValueCollectionRxtensions
	{
		/// <summary>
		/// Gets a safe integer value from an instance of NameValueCollection. 
		/// </summary>
		/// <param name="data">The instance of NameValueCollection to extract the integer value from.</param>
		/// <param name="key">The key in the collection.</param>
		/// <returns>The integer value, if it is set, otherwise null.</returns>
		public static int? GetIntegerValue(this NameValueCollection data, string key)
		{
			string value = data[key];

			if(String.IsNullOrEmpty(value))
			{
				return null;
			}

			try
			{
				return Int32.Parse(value);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a safe double value from an instance of NameValueCollection. 
		/// </summary>
		/// <param name="data">The instance of NameValueCollection to extract the double value from.</param>
		/// <param name="key">The key in the collection.</param>
		/// <returns>The double value, if it is set, otherwise null.</returns>
		public static double? GetDoubleValue(this NameValueCollection data, string key)
		{
			string value = data[key];

			if(String.IsNullOrEmpty(value))
			{
				return null;
			}

			try
			{
				return Double.Parse(value);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
