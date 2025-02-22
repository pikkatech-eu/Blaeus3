/***********************************************************************************
* File:         Settings.cs                                                        *
* Contents:     Class Settings                                                     *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-17 09:28                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Factotum.Xml;
[assembly:InternalsVisibleTo("Wiremu")]


namespace Blaeus.Library.Management
{
	public class Settings
	{
		#region Default Values
		internal const string COMPANY_NAME							= "pikkatech.eu";
		internal const string PRODUCT_NAME							= "Blaeus";

		internal static readonly string APPLICATION_FOLDER_NAME		= Path.Combine
																					(
																						Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
																						COMPANY_NAME, 
																						PRODUCT_NAME
																					);
		internal const string DATABASE_FOLDER_NAME					= "Databases";
		internal const string GEONAMES_DATABASE_FOLDER_NAME			= "Geonames";
		internal readonly static string DatabaseFolderPath			= $"{APPLICATION_FOLDER_NAME}\\{DATABASE_FOLDER_NAME}";
		internal readonly static string GeonamesDatabaseFolder		= $"{DatabaseFolderPath}\\{GEONAMES_DATABASE_FOLDER_NAME}";
		internal const string CONFIGURATION_FOLDER_NAME				= "Configuration";
		internal const string CONFIGURATION_FILE_NAME				= "blaeus.xml";
		internal readonly static string ConfigurationFolder			= Path.Combine(APPLICATION_FOLDER_NAME, CONFIGURATION_FOLDER_NAME);
		internal readonly static string ConfigurationFilePath		= Path.Combine(ConfigurationFolder, CONFIGURATION_FILE_NAME);

		/// <summary>
		/// Default set of working languages.
		/// </summary>
		internal static readonly string[] DEFAULT_LANGUAGES			= {"en", "de", "fr", "es", "it", "pl", "uk", "eo"};

		internal const string DEFAULT_GEONAMES_USER_NAME			= "AlexKonnen";

		internal const string DEFAULT_ACQUISITION_WORKFLOW_NAME		= "Geonames";

		internal const string DEFAULT_LOCAL_GEONAMES_DB_FILE_NAME	= "geolocalities_1000000.db3";

		internal const string DEFAULT_LOCAL_GEONAMES_CRITERION		= "TRUE";
		#endregion

		#region Singletonium
		private static Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());
		public static Settings Instance
		{
			get	{return _instance.Value;}
		}

		private Settings()
		{
			this.Initialize();
		}

		internal void Copy(Settings settings)
		{
			this.Languages						= settings.Languages;
			this.GeonamesUserName				= settings.GeonamesUserName;
			this.AcquisitionWorkflowName		= settings.AcquisitionWorkflowName;
			this.LocalGeonamesDatabaseFileName	= settings.LocalGeonamesDatabaseFileName;
			this.LocalGeonamesCriterion			= settings.LocalGeonamesCriterion;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Working languages.
		/// </summary>
		public List<string>	Languages	{get;internal set;}	= DEFAULT_LANGUAGES.ToList();

		public string GeonamesUserName	{get;set;}	= DEFAULT_GEONAMES_USER_NAME;

		// public string LocalGeoNamesDatabaseFilePath	{get;set;}

		public string AcquisitionWorkflowName	{get;set;}	= DEFAULT_ACQUISITION_WORKFLOW_NAME;

		public string LocalGeonamesDatabaseFileName	{get;set;} = DEFAULT_LOCAL_GEONAMES_DB_FILE_NAME;

		public string LocalGeonamesCriterion	{get;set;} = DEFAULT_LOCAL_GEONAMES_CRITERION;
		#endregion

		#region Initialization
		public void Initialize()
		{
			if (!Directory.Exists(APPLICATION_FOLDER_NAME))
			{
				Directory.CreateDirectory(APPLICATION_FOLDER_NAME);
			}

			if (!Directory.Exists(DatabaseFolderPath))
			{
				Directory.CreateDirectory(DatabaseFolderPath);
			}

			if (!Directory.Exists(GeonamesDatabaseFolder))
			{
				Directory.CreateDirectory(GeonamesDatabaseFolder);
				// when installing, you will have to copy a few geonames databases will have to be copied
			}

			if (!Directory.Exists(ConfigurationFolder))
			{
				Directory.CreateDirectory(ConfigurationFolder);
			}
		}
		#endregion

		#region XML
		public XElement ToXElement()
		{
			XElement x = new XElement("Blaeus.Settings");

			x.AppendElements<string>(this.Languages, "Languages");
			x.AppendElement("AcquisitionWorkflowName",			this.AcquisitionWorkflowName);
			x.AppendElement("LocalGeonamesDatabaseFileName",	this.LocalGeonamesDatabaseFileName);
			x.AppendElement("LocalGeonamesCriterion",			this.LocalGeonamesCriterion);

			return x;
		}

		public void FromXElement(XElement x)
		{
			this.Languages					= x.ListValue<string>("Languages");
			this.AcquisitionWorkflowName	= x.ElementValue<string>
																			(
																				"AcquisitionWorkflowName", 
																				DEFAULT_ACQUISITION_WORKFLOW_NAME
																			);

			this.LocalGeonamesDatabaseFileName		= x.ElementValue<string>("LocalGeonamesDatabaseFileName", DEFAULT_LOCAL_GEONAMES_DB_FILE_NAME);
			this.LocalGeonamesCriterion				= x.ElementValue<string>("LocalGeonamesCriterion", DEFAULT_LOCAL_GEONAMES_CRITERION);
		}
		#endregion

		#region I/O
		public void Save()
		{
			XElement x = this.ToXElement();
			x.Save(ConfigurationFilePath);
		}

		public void Load()
		{
			try
			{
				XElement x = XElement.Load(ConfigurationFilePath);
				this.FromXElement(x);
			}
			catch (Exception)
			{
			}
		}
		#endregion
	}
}
