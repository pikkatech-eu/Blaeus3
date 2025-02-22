/***********************************************************************************
* File:         LocalityPopulationComparer.cs                                      *
* Contents:     Class LocalityPopulationComparer                                   *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-09-12 17:09                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Domain;

namespace Blaeus.Library.Gazetteers
{
	public class PopulationComparer : IComparer<GeoLocality>
	{
		public int Compare(GeoLocality x, GeoLocality y)
		{
			if (x.Population < y.Population)
			{
				return 1;
			}
			else if (x.Population > y.Population)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
	}
}
