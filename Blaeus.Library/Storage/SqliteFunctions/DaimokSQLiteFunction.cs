/***********************************************************************************
* File:         DaimokSQLiteFunction.cs                                            *
* Contents:     Class DaimokSQLiteFunction                                         *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-02 18:54                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Data.SQLite;
using Alison.Library.Encoders;

namespace Blaeus.Library.Storage.SqliteFunctions
{
	[SQLiteFunction(Name = "DAIMOK", Arguments = 2, FuncType = FunctionType.Scalar)]
	public class DaimokSQLiteFunction : SQLiteFunction
	{
		public override object Invoke(object[] args)
		{
			string value = args[0].ToString();
			string probe = args[1].ToString();

			string[] valueCodes = DaitchMokotoff.Encode(value).Split(',');
			string[] probeCodes = DaitchMokotoff.Encode(probe).Split(',');

			foreach (string valueCode in valueCodes)
			{
				foreach (string probeCode in probeCodes)
				{
					if (probeCode == valueCode)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
