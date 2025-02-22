/***********************************************************************************
* File:         Willem.cs                                                          *
* Contents:     Class Willem                                                       *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-17 09:29                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Acquisition;
using Blaeus2.Library.Acquisition;
using Blaeus2.Library.Domain;
using Blaeus2.Library.Storage.Geonames;
using Factotum.Logging;

namespace Blaeus2.Library.Management
{
	public class Willem : IWillem
	{
		#region Singletonium
		private static Lazy<Willem> _instance = new Lazy<Willem>(() => new Willem());
		public static Willem Instance
		{
			get	{return _instance.Value;}
		}

		private Willem()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		public Dictionary<string, IAcquisitionWorkflow>	AcquisitionWorkflows		{get;internal set;}	= new Dictionary<string, IAcquisitionWorkflow>();
		public IAcquisitionWorkflow						ActiveAcquisitionWorkflow	{get;set;}
		public IAcquisitionAttributes					AcquisitionAttributes		{get;set;}
		#endregion

		#region Public features
		/// <summary>
		/// Carries out an acquisition action.
		/// </summary>
		/// <param name="attributes">The attributes to proceed.</param>
		/// <param name="AcquisitionWorkflowName"></param>
		/// <returns>Protocol containing the results of the acquisition.</returns>
		public AcquisitionProtocol AcquireGeolocalities(string AcquisitionWorkflowName, IAcquisitionAttributes attributes)
		{
			IAcquisitionWorkflow workflow	= this.AcquisitionWorkflows[AcquisitionWorkflowName];
			return workflow.Acquire(attributes);
		}

		public AcquisitionProtocol AcquireGeolocalities()
		{
			return this.ActiveAcquisitionWorkflow.Acquire(this.AcquisitionAttributes);
		}

		/// <summary>
		/// Creates an instance of GeonamesAcquisitionAttributes using a local Geonames database.
		/// </summary>
		/// <param name="databaseFileName">The file name of the local Geonames database.</param>
		/// <param name="criterion">The WHERE criterion to create.</param>
		public void CreateGeonamesAcquisitionAttributes(string databaseFileName, string criterion)
		{
			if (this.ActiveAcquisitionWorkflow is GeonamesAcquisitionWorkflow)
			{
				SqliteGeonamesDatabase database		= new SqliteGeonamesDatabase();

				string databasePath					= Path.Combine(Settings.GeonamesDatabaseFolder, databaseFileName);
				database.Open(databasePath);

				GeoLocality[] localities			= database.Select(criterion);

				this.AcquisitionAttributes			= new GeonamesAcquisitionAttributes();
				this.AcquisitionAttributes.Sources	= localities.Select(loc => loc.GeonamesId.ToString()).ToArray();
			}
		}

		/// <summary>
		/// Carries out editing of a geolocality already present on the database.
		/// The geolocality has been previously selected in a GUI control and transferred to the manager.
		/// </summary>
		public void EditGeolocality()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Carries out deletion of a geolocality already present on the database.
		/// The geolocality (or its ID) has been previously selected in a GUI control and transferred to the manager.
		/// </summary>
		public void DeleteGeolocality()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Carries out exporting of a number of geolocations to an external devise (file or stream).
		/// </summary>
		/// <param name="definition">
		///		The definition string contains the external device, 
		///		the format of the export and the condition determining 
		///		which geolocations should be exported.
		/// </param>
		public void ExportGeolocalities(string definition)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Private Auxiliary
		private void Initialize()
		{
			Logger.Open("Willem");

			Settings.Instance.Load();

			this.AcquisitionWorkflows.Add("Geonames", new GeonamesAcquisitionWorkflow());
			this.ActiveAcquisitionWorkflow	= this.AcquisitionWorkflows["Geonames"];

			this.ActiveAcquisitionWorkflow.WorkflowStep += this.OnWorkflowStep;
		}

		private void OnWorkflowStep(string message)
		{
			this.Notification?.Invoke(message);
		}
		#endregion

		public event Action<string> Notification;
	}
}
