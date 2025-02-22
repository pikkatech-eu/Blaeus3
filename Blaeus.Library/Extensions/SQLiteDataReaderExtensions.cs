/***********************************************************************************
* File:         SQLiteDataReaderExtensions.cs                                      *
* Contents:     Class SQLiteDataReaderExtensions                                   *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-16 12:16                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Data.SQLite;

namespace Blaeus.Library.Extensions
{
	public static class SQLiteDataReaderExtensions
	{
		public static int? SafeInteger(this SQLiteDataReader reader, string fieldName)
		{
			return reader.IsDBNull(reader.GetOrdinal(fieldName)) ? (int?)null : reader.GetInt32(reader.GetOrdinal(fieldName));
		}

		public static double? SafeDouble(this SQLiteDataReader reader, string fieldName)
		{
			return reader.IsDBNull(reader.GetOrdinal(fieldName)) ? (int?)null : reader.GetDouble(reader.GetOrdinal(fieldName));
		}

		public static string SafeString(this SQLiteDataReader reader, string fieldName)
		{
			return reader.IsDBNull(reader.GetOrdinal(fieldName)) ? null : reader.GetString(reader.GetOrdinal(fieldName));
		}

		public static byte[] SafeBytes(this SQLiteDataReader reader, string fieldName)
		{
			if (reader.IsDBNull(reader.GetOrdinal(fieldName)))
			{
				return null;
			}
			else
			{
				return (byte[])reader[fieldName];
			}
		}
	}
}
