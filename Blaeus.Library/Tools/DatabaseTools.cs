/***********************************************************************************
* File:         DatabaseTools.cs                                                   *
* Contents:     Class DatabaseTools                                                *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-03 09:46                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaeus.Library.Tools
{
	public static class DatabaseTools
	{
		/// <summary>
		/// Gets a safe integer value from an instance of NameValueCollection. 
		/// </summary>
		/// <param name="data">The instance of NameValueCollection to extract the integer value from.</param>
		/// <param name="key">The key in the collection.</param>
		/// <returns>The integer value, if it is set, otherwise null.</returns>
		public static int? GetIntegerValue(NameValueCollection data, string key)
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
		public static double? GetDoubleValue(NameValueCollection data, string key)
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

		/// <summary>
		/// Binds a user-defined SQLite function to a SQLite connection.
		/// </summary>
		/// <param name="connection">Connection to bind to.</param>
		/// <param name="function">Function to bind.</param>
		public static void BindFunction(this SQLiteConnection connection, SQLiteFunction function)
		{
			var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
			connection.BindFunction(attributes[0], function);
		}
	}
}
