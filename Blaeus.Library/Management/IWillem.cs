/***********************************************************************************
* File:         IWillem.cs                                                         *
* Contents:     Interface IWillem                                                  *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-16 12:59                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Acquisition;
using Blaeus2.Library.Acquisition;
using Blaeus2.Library.Gazetteers;

namespace Blaeus2.Library.Management
{
	/// <summary>
	/// Willem is the Blaeus server manager.
	/// </summary>
	public interface IWillem
	{
		#region Properties
		Dictionary<string, IAcquisitionWorkflow>	AcquisitionWorkflows		{get;}
		IAcquisitionWorkflow						ActiveAcquisitionWorkflow	{get;set;}

		IAcquisitionAttributes						AcquisitionAttributes		{get;set;}
		#endregion

		/// <summary>
		/// Carries out an acquisition action.
		/// </summary>
		/// <param name="attributes">The attributes to proceed.</param>
		/// <param name="AcquisitionWorkflowName"></param>
		/// <returns>Protocol containing the results of the acquisition.</returns>
		AcquisitionProtocol AcquireGeolocalities(string AcquisitionWorkflowName, IAcquisitionAttributes attributes);

		/// <summary>
		/// Creates an instance of GeonamesAcquisitionAttributes using a local Geonames database.
		/// </summary>
		/// <param name="databaseFileName">The file name of the local Geonames database.</param>
		/// <param name="criterion">The WHERE criterion to create.</param>
		void CreateGeonamesAcquisitionAttributes(string databaseFileName, string criterion);

		/// <summary>
		/// Carries out editing of a geolocality already present on the database.
		/// The geolocality has been previously selected in a GUI control and transferred to the manager.
		/// </summary>
		void EditGeolocality();

		/// <summary>
		/// Carries out deletion of a geolocality already present on the database.
		/// The geolocality (or its ID) has been previously selected in a GUI control and transferred to the manager.
		/// </summary>
		void DeleteGeolocality();

		/// <summary>
		/// Carries out exporting of a number of geolocations to an external devise (file or stream).
		/// </summary>
		/// <param name="definition">
		///		The definition string contains the external device, 
		///		the format of the export and the condition determining 
		///		which geolocations should be exported.
		/// </param>
		void ExportGeolocalities(string definition);

		event Action<string> Notification;
	}
}
