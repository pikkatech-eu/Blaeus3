/***********************************************************************************
* File:         AcquisitionResultStatus.cs                                         *
* Contents:     Enum AcquisitionResultStatus                                       *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-18 20:21                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

namespace Blaeus.Library.Domain.Enumerations
{
	/// <summary>
	/// Defines trhe outcome of the acquisition of a GeoLocality.
	/// </summary>
	public enum AcquisitionResultStatus
	{
		/// <summary>
		/// GeoLocality was acquired and successfully inserted into the database.
		/// </summary>
		Success,

		/// <summary>
		/// GeoLocality was acquired, but not inserted into the database.
		/// </summary>
		InsertionFailed,

		/// <summary>
		/// GeoLocality could not be acquired.
		/// </summary>
		Failure
	}
}
