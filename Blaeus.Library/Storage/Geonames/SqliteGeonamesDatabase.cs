/***********************************************************************************
* File:         SqliteGeonamesDatabase.cs                                          *
* Contents:     Class SqliteGeonamesDatabase                                       *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-14 10:20                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Collections.Specialized;
using System.Data.SQLite;
using System.Globalization;
using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Dictionaries;
using Blaeus.Library.Domain.Enumerations;
using Blaeus.Library.Extensions;

namespace Blaeus.Library.Storage.Geonames
{
	/// <summary>
	/// Future geonames database engine.
	/// TODO: 
	/// (1) create Geonames databases using consistent original Geonames data model; 
	/// (2) implement IGeoNamesDatabase using that data model.
	/// </summary>
	public class SqliteGeonamesDatabase : IGeoNamesDatabase
	{
		#region Private members
		private SQLiteConnection _connection				= null;
		#endregion

		#region Constants
		private static readonly string TABLE_NAME	= "`geolocality`";

		private const string SQL_CREATE_TABLE_GEOLOCALITY = @"
		CREATE TABLE IF NOT EXISTS ""geolocality"" (
			""geonames_id""	INTEGER,
			""name""	VARCHAR(128),
			""asciiname""	VARCHAR(128),
			""alternatenames"" TEXT,
			""latitude"" FLOAT,
			""longitude"" FLOAT,
			""feature_class"" VARCHAR(8),
			""feature_code""	VARCHAR(8),
			""country_code""  VARCHAR(8),
			""cc2"" VARCHAR(128),
			""admin1_code"" VARCHAR(16),
			""admin2_code"" VARCHAR(16),
			""admin3_code"" VARCHAR(16),
			""admin4_code"" VARCHAR(16),
			""population""	INTEGER,
			""elevation""	INTEGER,
			""dem"" VARCHAR(16),
			""time_zone"" VARCHAR(32),
			""modification_date"" VARCHAR(32),
    
			PRIMARY KEY(""geonames_id"" )
		)
		";

		private readonly string[] INCLUDE_FEATURE_CLASSES = new string[] {"P"};
		#endregion

		#region Public static properties
		public static bool IncludeOnlyDefinedFeatureClasses	{get;set;} = true;
		#endregion

		public void Open(string source)
		{
			SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
			csb.DataSource = source;
			csb.Version = 3;
			csb.UseUTF16Encoding = true;
			string strConnection = csb.ToString();

			this._connection = new SQLiteConnection(strConnection);
			this._connection.Open();
		}

		public GeoLocality Select(int geonamesId)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_NAME} WHERE `geonames_id` = {geonamesId} ORDER BY `population` DESC";

				SQLiteDataReader reader = command.ExecuteReader();

				reader.Read();

				if (reader.HasRows)
				{
					NameValueCollection data = reader.GetValues();
					GeoLocality locality = this.ExtractGeoLocality(data);
					return locality;
				}
				else
				{
					return null;
				}
			}
		}

		public GeoLocality[] Select(string sqlWhereCriterion, int limit = 0)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_NAME} WHERE {sqlWhereCriterion}";

				if (IncludeOnlyDefinedFeatureClasses)
				{
					command.CommandText += " AND `feature_class` IN (";

					foreach (string fc in INCLUDE_FEATURE_CLASSES)
					{
						command.CommandText += $"'{fc}',";
					}

					command.CommandText = command.CommandText.TrimEnd(',');

					command.CommandText += ") ";
				}
				
				command.CommandText += $" ORDER BY `population` DESC";

				if (limit > 0)
				{
					command.CommandText += $" LIMIT {limit}";
				}

				SQLiteDataReader reader = command.ExecuteReader();

				List<GeoLocality> localities = new List<GeoLocality>();

				while (reader.Read())
				{
					NameValueCollection data = reader.GetValues();
					GeoLocality locality = this.ExtractGeoLocality(data);
					localities.Add(locality);
				}

				return localities.ToArray();
			}
		}

		public GeoLocality[] SelectByNameLike(string nameToken, int minPopulation = 0)
		{
			return this.Select($"`name` LIKE '%{nameToken}%' AND `population` >= {minPopulation}");
		}

		/// <summary>
		/// Selects localities lying within a bounding box.
		/// </summary>
		/// <param name="box">The bounding box to find locaties within.</param>
		/// <param name="minPopulation">Minimum population to filter by.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		public GeoLocality[] SelectByBoundingBox(GeoRectangle box, int minPopulation = 0)
		{
			string criterion = $"`latitude` >= {box.South} AND `latitude` <= {box.North} AND `longitude` >= {box.West} AND `longitude` <= {box.East} AND `population` >= {minPopulation}";
			return this.Select(criterion);
		}

		private GeoLocality ExtractGeoLocality(NameValueCollection data)
		{
			GeoLocality locality			= new GeoLocality();

			locality.GeonamesId				= Int32.Parse(data["geonames_id"]);
			locality.Name					= data["asciiname"];
			
			locality.Point.Latitude			= Double.Parse(data["latitude"], CultureInfo.InvariantCulture);
			locality.Point.Longitude		= Double.Parse(data["longitude"], CultureInfo.InvariantCulture);
			locality.CountryCode			= data["country_code"];
			locality.Elevation				= data.GetDoubleValue("elevation") ?? 0;
			locality.Population				= data.GetIntegerValue("population") ?? 0;
			locality.GeoNamesFeatureCode	= (GeoNamesFeatureCode)Enum.Parse(typeof(GeoNamesFeatureCode), data["feature_code"]);
			locality.GeoNamesFeatureClass	= (GeoNamesFeatureClass)Enum.Parse(typeof(GeoNamesFeatureClass), data["feature_class"]);

			string[] alternativeNames		= data["alternatenames"].Split(',');

            for (int i = 0; i < alternativeNames.Length; i++)
            {
                locality.AlternativeNames.Add($"f{i}", alternativeNames[i]);
            }

			string timeZone					= data["time_zone"];

			if (TimeZoneDictionary.TimeZones.ContainsKey(timeZone))
			{
				locality.TimeZone =  TimeZoneDictionary.TimeZones[timeZone];
			}

            return locality;
		}
	}
}
