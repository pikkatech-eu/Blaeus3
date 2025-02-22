/***********************************************************************************
* File:         JsonExtensions.cs                                                  *
* Contents:     Class JsonExtensions                                               *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-11 20:27                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Blaeus.Library.Extensions
{
	public static class JsonExtensions
	{
		public static int GetArrayDepth(this JToken token)
		{
			if (token is not JArray array)
			{
				return 0; // Not an array
			}

			int maxDepth = 0;

			foreach (JToken child in array)
			{
				maxDepth = Math.Max(maxDepth, GetArrayDepth(child));
			}

			return maxDepth + 1;
		}
	}
}
