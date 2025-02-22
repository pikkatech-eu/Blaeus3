/***********************************************************************************
* File:         CosineSQLiteFunction.cs                                            *
* Contents:     Class CosineSQLiteFunction                                         *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-02 18:06                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Data.SQLite;

namespace Blaeus.Library.Storage.SqliteFunctions
{
	[SQLiteFunction(Name = "COSINE", Arguments = 2, FuncType = FunctionType.Scalar)]
	public class CosineSQLiteFunction : SQLiteFunction
	{
		public override object Invoke(object[] args)
		{
			string value = args[0].ToString();
			string probe = args[1].ToString();

			return Alison.Library.StringMeasures.Cosine.Similarity(value, probe);
		}
	}
}
