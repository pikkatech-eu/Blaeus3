/***********************************************************************************
* File:         SqliteBlaeusDatabase.cs                                            *
* Contents:     Class SqliteBlaeusDatabase                                         *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-13 17:39                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Collections.Specialized;
using System.Data.SQLite;
using Blaeus.Domain.Geospatial;
using Blaeus.Library.Domain;
using Blaeus.Library.Domain.Enumerations;
using Blaeus.Library.Exceptions;
using Blaeus.Library.Extensions;
using Blaeus.Library.Storage.SqliteFunctions;
using Blaeus.Library.Tools;
using DT = Blaeus.Library.Tools.DatabaseTools;

namespace Blaeus.Library.Storage
{
	public class SqliteBlaeusDatabase : IBlaeusDatabase
	{
		#region Database Constants
		private const string TABLE_GEOLOCALITY		= "geolocality";
		private const string TABLE_ALTERNATIVE_NAME = "alternative_name";
		private const string TABLE_HISTORIC_NAME	= "historic_name";

		private const string SQL_CREATE_TABLE_GEOLOCALITY = @"
	CREATE TABLE IF NOT EXISTS ""geolocality"" (
	""id""						INTEGER,
	""geonames_id""				INTEGER,
	""osm_node_id""				INTEGER,
	""osm_relation_id""			INTEGER,
	""wikidata_id""				VARCHAR(32),
	""name""					VARCHAR(128),
	""description""				TEXT,
	""latitude""				REAL,
	""longitude""				REAL,
	""elevation""				INTEGER,
	""population""				INTEGER DEFAULT 0,
	""country_code""			VARCHAR(4),
	""time_zone""				REAL,
	""geonames_feature_class""	VARCHAR(8),
	""geonames_feature_code""	VARCHAR(8),
	""osm_place_category""		VARCHAR(32),
	""bounding_box""			VARCHAR(128),
	""polygon""					BLOB,
	PRIMARY KEY(""id"" AUTOINCREMENT)
)
";

		private const string SQL_CREATE_TABLE_ALTERNATIVE_NAME = @"
	CREATE TABLE  IF NOT EXISTS  ""alternative_name"" (
	""geolocality_id""	INTEGER,
	""language_code""	VARCHAR(4),
	""name""			VARCHAR(128)
)
";
		private const string SQL_CREATE_TABLE_HISTORIC_NAME = @"
	CREATE TABLE  IF NOT EXISTS  ""historic_name"" (
	""geolocality_id""	INTEGER,
	""language_code""	VARCHAR(4),
	""name""			VARCHAR(128),
	""from_year""		INTEGER DEFAULT NULL,
	""to_year""			INTEGER DEFAULT NULL,
	""source""			VARCHAR(256)
)
";
		#endregion

		#region Private variables
		private SQLiteConnection _connection = new SQLiteConnection();
		#endregion

		#region Public Properties
		public static double CosineSimilarityThreshold	{get;set;}	= 0.5;
		#endregion

		#region Public Features
		/// <summary>
		/// Opens the database.
		/// </summary>
		/// <param name="source">The database source, i.o.w. the path to the database file (for Sqlite).</param>
		public void Open(string source)
		{
			SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
			csb.DataSource = source;
			csb.Version = 3;
			csb.UseUTF16Encoding = true;
			string strConnection = csb.ToString();

			this._connection = new SQLiteConnection(strConnection);
			this._connection.Open();

			this.CreateTables();

			this._connection.BindFunction(new CosineSQLiteFunction());
			this._connection.BindFunction(new DaimokSQLiteFunction());
		}

		/// <summary>
		/// Inserts a geolocality.
		/// </summary>
		/// <param name="locality">Geolocality to insert.</param>
		public int InsertGeoLocality(GeoLocality locality)
		{
			if (!locality.IsValid())
			{
				return -1;
			}

			if (!this.CanInsert(locality))
			{
				return -2;
			}

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"INSERT INTO {TABLE_GEOLOCALITY} (id, geonames_id, osm_node_id, osm_relation_id, wikidata_id, name, description, latitude, longitude, " +
					$"elevation, population, country_code, time_zone, geonames_feature_class, geonames_feature_code, osm_place_category, bounding_box, polygon) " +
					$"VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, @polygon)";

				command.Parameters.Add(new SQLiteParameter("id",						null));
				command.Parameters.Add(new SQLiteParameter("geonames_id",				locality.GeonamesId));
				command.Parameters.Add(new SQLiteParameter("osm_node_id",				locality.OpenStreetMapNodeId));
				command.Parameters.Add(new SQLiteParameter("osm_relation_id",			locality.OpenStreetMapRelationId));
				command.Parameters.Add(new SQLiteParameter("wikidata_id",				locality.WikiDataId));
				command.Parameters.Add(new SQLiteParameter("name",						locality.Name));
				command.Parameters.Add(new SQLiteParameter("description",				locality.Description));
				command.Parameters.Add(new SQLiteParameter("latitude",					locality.Point.Latitude));
				command.Parameters.Add(new SQLiteParameter("longitude",					locality.Point.Longitude));
				command.Parameters.Add(new SQLiteParameter("elevation",					locality.Elevation));
				command.Parameters.Add(new SQLiteParameter("population",				locality.Population));
				command.Parameters.Add(new SQLiteParameter("country_code",				locality.CountryCode));
				command.Parameters.Add(new SQLiteParameter("time_zone",					locality.TimeZone));
				command.Parameters.Add(new SQLiteParameter("geonames_feature_class",	locality.GeoNamesFeatureClass));
				command.Parameters.Add(new SQLiteParameter("geonames_feature_code",		locality.GeoNamesFeatureCode));
				command.Parameters.Add(new SQLiteParameter("osm_place_category",		locality.OpenStreetMapPlaceCategory));
				command.Parameters.Add(new SQLiteParameter("bounding_box",				locality.BoundingBox != null ? locality.BoundingBox.ToString() : null));

				SQLiteParameter param = new SQLiteParameter("@polygon", System.Data.DbType.Binary);
				param.Value	= locality.Polygon != null ? locality.Polygon.ToByteArray() : null;
				command.Parameters.Add(param);

				command.ExecuteNonQuery();

				locality.Id = (int)this._connection.LastInsertRowId;
			}

			this.InsertAlternativeNames(locality);
			this.InsertHistoricNames(locality);

			return locality.Id;


		}

		private bool CanInsert(GeoLocality locality)
		{
			if (locality.GeonamesId != null)
			{
				GeoLocality loc = this.SelectGeoLocalityByGeonamesId((int)locality.GeonamesId);

				if (loc != null)
				{
					throw new BlaeusDatabaseException("Geonames ID already in the database.", loc);
				}
			}
			
			if (locality.OpenStreetMapRelationId != null)
			{
				GeoLocality loc = this.SelectGeoLocalityByOpenStreetMapId((int)locality.OpenStreetMapRelationId);

				if (loc != null)
				{
					throw new BlaeusDatabaseException("OpenStreetMap Relation ID already in the database.", loc);
				}
			}

			if (locality.WikiDataId != null)
			{
				GeoLocality loc = this.SelectGeoLocalityByWikidataId(locality.WikiDataId);

				if (loc != null)
				{
					throw new BlaeusDatabaseException("Wikidata ID already in the database.", loc);
				}
			}

			return true;
		}

		/// <summary>
		/// Updates a geolocality by replacig its data.
		/// </summary>
		/// <param name="locality">Geolocality to update. The geolocality Id in the instance must be present in the database.</param>
		public void ReplaceGeoLocality(GeoLocality locality)
		{
			GeoLocality loc = this.SelectGeoLocality(locality.Id);

			if (loc == null)
			{
				return;
			}

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"UPDATE {TABLE_GEOLOCALITY} SET `id` = {locality.Id}, ";
				command.CommandText += this.GetFieldReplacement("geonames_id", locality.GeonamesId, false);
				command.CommandText += this.GetFieldReplacement("osm_node_id", locality.OpenStreetMapNodeId, false);
				command.CommandText += this.GetFieldReplacement("osm_relation_id", locality.OpenStreetMapRelationId, false);
				command.CommandText += this.GetFieldReplacement("wikidata_id", locality.WikiDataId, true);
				command.CommandText += this.GetFieldReplacement("name", locality.Name, true);
				command.CommandText += this.GetFieldReplacement("description", locality.Description, true);
				command.CommandText += this.GetFieldReplacement("latitude", locality.Point.Latitude, false);
				command.CommandText += this.GetFieldReplacement("longitude", locality.Point.Longitude, false);
				command.CommandText += this.GetFieldReplacement("elevation", locality.Elevation, false);
				command.CommandText += this.GetFieldReplacement("population", locality.Population, false);
				command.CommandText += this.GetFieldReplacement("country_code", locality.CountryCode, true);
				command.CommandText += this.GetFieldReplacement("time_zone", locality.TimeZone, false);
				command.CommandText += this.GetFieldReplacement("geonames_feature_class", locality.GeoNamesFeatureClass.ToString(), true);
				command.CommandText += this.GetFieldReplacement("geonames_feature_code", locality.GeoNamesFeatureCode.ToString(), true);
				command.CommandText += this.GetFieldReplacement("osm_place_category", locality.OpenStreetMapPlaceCategory.ToString(), true);
				command.CommandText += this.GetFieldReplacement("bounding_box", locality.BoundingBox.ToString(), true);
				command.CommandText += this.GetFieldReplacement("polygon", locality.Polygon.ToByteArray(), true);

				command.CommandText = command.CommandText.TrimEnd(',', ' ');

				command.CommandText += $" WHERE `id` = {locality.Id}";

				command.ExecuteNonQuery();
			}

			this.ReplaceAlternativeNames(locality);
			this.ReplaceHistoricNames(locality);
		}

		private void ReplaceAlternativeNames(GeoLocality locality)
		{
			foreach (string key in locality.AlternativeNames.Keys)
			{
				using (SQLiteCommand command = this._connection.CreateCommand())
				{
					command.CommandText = $"UPDATE {TABLE_ALTERNATIVE_NAME} SET ";
					command.CommandText += $"`name` = '{locality.AlternativeNames[key].ToDatabaseString()}' ";
					command.CommandText += $" WHERE `geolocality_id` = {locality.Id} AND `language_code` = '{key}'";

					command.ExecuteNonQuery();
				}
			}
		}

		private void ReplaceHistoricNames(GeoLocality locality)
		{
			foreach (HistoricName historic in locality.HistoricNames)
			{
				using (SQLiteCommand command = this._connection.CreateCommand())
				{
					command.CommandText = $"UPDATE {TABLE_HISTORIC_NAME} SET ";
					command.CommandText += $"`name` = '{historic.Name.ToDatabaseString()}', ";
					command.CommandText += this.GetFieldReplacement("from_year", historic.From, false);
					command.CommandText += this.GetFieldReplacement("to_year", historic.To, false);
					command.CommandText += $"`source` = '{historic.Source.ToDatabaseString()}' ";

					command.CommandText += $" WHERE `geolocality_id` = {locality.Id} AND `language_code` = '{historic.Key}'";

					command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Creates a partial string for the replacement of a field in the "UPDATE" statement.
		/// </summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="value">The value of the field, which can be null.</param>
		/// <param name="isString">True, if the value is of string type (requires '').</param>
		/// <returns>The string to add to the replacement statement; if the value is null, an empty string.</returns>
		private string GetFieldReplacement(string fieldName, object value, bool isString)
		{
			if (value != null)
			{
				string valueString = isString ? $"'{value}'" : value.ToString();
				return $"`{fieldName}` = {valueString}, ";
			}
			else
			{
				return "";
			}
		}
		
		/// <summary>
		/// Deletes a geolocality.
		/// </summary>
		/// <param name="id">The Id of the locality to delete.</param>
		public void DeleteGeoLocality(int id)
		{
			GeoLocality loc = this.SelectGeoLocality(id);

			if (loc == null)
			{
				return;
			}

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"DELETE FROM {TABLE_GEOLOCALITY} WHERE `id` = {id}";
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Selects a locality by its ID in Blaeus system.
		/// </summary>
		/// <param name="id">The id to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		public GeoLocality SelectGeoLocality(int id)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_GEOLOCALITY} WHERE `id` = {id}";

				SQLiteDataReader reader = command.ExecuteReader();

				reader.Read();
				
				if (reader.HasRows)
				{
					GeoLocality locality = this.ExtractGeolocality(reader);

					locality.AlternativeNames = this.GetAlternativeNames(id);
					locality.HistoricNames	= this.GetHistoricNames(id);

					return locality;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Selects a locality by its Geonames ID.
		/// </summary>
		/// <param name="geonamesId">The Geonames ID to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		public GeoLocality SelectGeoLocalityByGeonamesId(int geonamesId)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_GEOLOCALITY} WHERE `geonames_id` = {geonamesId}";

				SQLiteDataReader reader = command.ExecuteReader();

				reader.Read();

				if (reader.HasRows)
				{
					GeoLocality locality = this.ExtractGeolocality(reader);

					locality.AlternativeNames = this.GetAlternativeNames(locality.Id);

					return locality;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Selects a locality by its OSM ID.
		/// </summary>
		/// <param name="osmId">The OSM ID to select by.</param>
		/// <param name="useRelationId">If true, the OSM relation Id will be taken</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		public GeoLocality SelectGeoLocalityByOpenStreetMapId(int osmId, bool useRelationId = true)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				string osmIDName = useRelationId ? "osm_relation_id" : "osm_id";
				command.CommandText = $"SELECT * FROM {TABLE_GEOLOCALITY} WHERE `{osmIDName}` = {osmId}";

				SQLiteDataReader reader = command.ExecuteReader();

				reader.Read();

				if (reader.HasRows)
				{
					GeoLocality locality = this.ExtractGeolocality(reader);

					locality.AlternativeNames = this.GetAlternativeNames(locality.Id);

					return locality;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Selects a locality by its Wikidata ID.
		/// </summary>
		/// <param name="wikidataId">The Wikidata ID to select by.</param>
		/// <returns>The locality, if found, otherwise null.</returns>
		public GeoLocality SelectGeoLocalityByWikidataId(string wikidataId)
		{
			if (String.IsNullOrEmpty(wikidataId))
			{
				return null;
			}

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_GEOLOCALITY} WHERE `wikidata_id` = '{wikidataId}'";

				SQLiteDataReader reader = command.ExecuteReader();

				reader.Read();

				if (reader.HasRows)
				{
					GeoLocality locality = this.ExtractGeolocality(reader);

					locality.AlternativeNames = this.GetAlternativeNames(locality.Id);

					return locality;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Selects localities using the SQL WHERE clause.
		/// E.g. to select all localities with the names including "aris" the value would be " `name` LIKE '%aris%'".
		/// </summary>
		/// <param name="sqlWhereCriterion">The fragment of the WHERE clause to select by, see above.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		public GeoLocality[] SelectGeoLocalities(string sqlWhereCriterion)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_GEOLOCALITY} WHERE {sqlWhereCriterion} ORDER BY `population`";

				SQLiteDataReader reader = command.ExecuteReader();

				List<GeoLocality> geoLocalities = new List<GeoLocality>();

				while (reader.Read())
				{
					GeoLocality geoLocality = this.ExtractGeolocality(reader);
					geoLocalities.Add(geoLocality);
				}

				return geoLocalities.ToArray();
			}
		}

		/// <summary>
		/// Selects localities by name likeness.
		/// TODO: **implement taking comparison into account.**
		/// </summary>
		/// <param name="nameToken">The name token to be contained in the localiti nymes.</param>
		/// <param name="comparison">The method to compare toponyms.</param>
		/// <param name="minPopulation">Minimum population to filter by.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		public GeoLocality[] SelectByNameLike(string nameToken, Domain.Enumerations.StringSelectorCriterion comparison, int minPopulation = 0)
		{
			string criterion = "";

			switch (comparison)
			{
				case Domain.Enumerations.StringSelectorCriterion.ProbeIn:
					criterion = $"`name` LIKE '%{nameToken}%' AND `population` >= {minPopulation}";
					break;

				case Domain.Enumerations.StringSelectorCriterion.ProbeContains:
					criterion = $" INSTR('{nameToken}', `name`) > 0";
					break;

				case Domain.Enumerations.StringSelectorCriterion.Soundex:
					criterion = $" SOUNDEX(`name`) = SOUNDEX('{nameToken}')";
					break;

				case Domain.Enumerations.StringSelectorCriterion.Daimok:
					criterion = $" DAIMOK(`name`, '{nameToken}') = 1";
					break;

				case Domain.Enumerations.StringSelectorCriterion.Cosine:
					criterion = $" COSINE(`name`, '{nameToken}') >= {CosineSimilarityThreshold}";
					break;

				default:
					return new GeoLocality[0];
			}

			return this.SelectGeoLocalities(criterion);
		}

		/// <summary>
		/// Selects localities lying within a bounding box.
		/// </summary>
		/// <param name="box">The bounding box to find locaties within.</param>
		/// <param name="minPopulation">Minimum population to filter by.</param>
		/// <returns>Array of localities found or an empty array if nothing found.</returns>
		public GeoLocality[] SelectByBoundingBox(GeoRectangle box, int minPopulation = 0)
		{
			string criterion = $"`latitude` >= {box.South} AND `latitude` <= {box.North} AND AND `longitude` >= {box.West} AND `longitude` <= {box.East} AND `population` >= {minPopulation}";
			return this.SelectGeoLocalities(criterion);
		}

		public GeoLocality[] SelectGeolocalities(int limit = -1)
		{
			string criterion = " TRUE";

			if (limit > 0)
			{
				criterion = $" TRUE LIMIT {limit}";
			}

			return this.SelectGeoLocalities(criterion);
		}
		#endregion

		#region Private Auxiliary
		/// <summary>
		/// Creates an SQL table.
		/// </summary>
		/// <param name="sqlCreateTable">The SQL creation statement.</param>
		private void CreateTable(string sqlCreateTable)
		{
			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText				= sqlCreateTable;
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Creates all database tables.
		/// </summary>
		private void CreateTables()
		{
			this.CreateTable(SQL_CREATE_TABLE_GEOLOCALITY);
			this.CreateTable(SQL_CREATE_TABLE_ALTERNATIVE_NAME);
			this.CreateTable(SQL_CREATE_TABLE_HISTORIC_NAME);
		}

		/// <summary>
		/// Inserts alternative names of a geolocality into the TABLE_ALTERNATIVE_NAME database table.
		/// </summary>
		/// <param name="locality">The geolocality to insert alternative names for.</param>
		private void InsertAlternativeNames(GeoLocality locality)
		{
			if (!locality.IsValid())
			{
				return;
			}

			using (SQLiteCommand command = this._connection.CreateCommand())
			{

				foreach (string key in locality.AlternativeNames.Keys)
				{
					command.CommandText = $"INSERT INTO {TABLE_ALTERNATIVE_NAME} (geolocality_id, language_code, name) VALUES (?, ?, ?)";
					command.Parameters.Add(new SQLiteParameter("geolocality_id", locality.Id));
					command.Parameters.Add(new SQLiteParameter("language_code", key));
					command.Parameters.Add(new SQLiteParameter("name", locality.AlternativeNames[key]));

					command.ExecuteNonQuery();
				}
			}
		}

		private void InsertHistoricNames(GeoLocality locality)
		{
			if (!locality.IsValid())
			{
				return;
			}

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				foreach (HistoricName historic in locality.HistoricNames)
				{
					command.CommandText = $"INSERT INTO {TABLE_HISTORIC_NAME} (geolocality_id, language_code, name, from_year, to_year, source) VALUES (?, ?, ?, ?, ?, ?)";
					command.Parameters.Add(new SQLiteParameter("geolocality_id", locality.Id));
					command.Parameters.Add(new SQLiteParameter("language_code", historic.Key));
					command.Parameters.Add(new SQLiteParameter("name", historic.Name));
					command.Parameters.Add(new SQLiteParameter("from_year", historic.From));
					command.Parameters.Add(new SQLiteParameter("to_year", historic.To));
					command.Parameters.Add(new SQLiteParameter("source", historic.Source));

					command.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Gets the alternative names of a geolocation.
		/// </summary>
		/// <param name="geolocalityId">The id of the geolocality.</param>
		/// <returns>The dictionaty of alternative names.</returns>
		private Dictionary<string, string> GetAlternativeNames(int geolocalityId)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_ALTERNATIVE_NAME} WHERE `geolocality_id` = {geolocalityId}";

				SQLiteDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					NameValueCollection data = reader.GetValues();
					string languageCode = data["language_code"];
					string name	= data["name"];

					result[languageCode] = name;
				}

				return result;
			}
		}

		private List<HistoricName> GetHistoricNames(int geolocalityId)
		{
			List<HistoricName> result	= new List<HistoricName>();

			using (SQLiteCommand command = this._connection.CreateCommand())
			{
				command.CommandText = $"SELECT * FROM {TABLE_HISTORIC_NAME} WHERE `geolocality_id` = {geolocalityId}";

				SQLiteDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					NameValueCollection data = reader.GetValues();

					string languageCode = data["language_code"];
					string name	= data["name"];
					int? from	= DT.GetIntegerValue(data, "from_year");
					int? to		= DT.GetIntegerValue(data, "to_year");
					string source	= data["source"];

					HistoricName historic = new HistoricName{Key=languageCode, Name=name, From=from, To=to, Source=source};
					result.Add(historic);
				}
			}

			return result;
		}


		private GeoLocality ExtractGeolocality(SQLiteDataReader reader)
		{
			GeoLocality locality				= new GeoLocality();

			locality.Id							= reader.GetInt32(reader.GetOrdinal("id"));
			locality.GeonamesId					= reader.SafeInteger("geonames_id");

			locality.OpenStreetMapNodeId		= reader.SafeInteger("osm_node_id");
			locality.OpenStreetMapRelationId	= reader.SafeInteger("relation_id");

			locality.WikiDataId					= reader.GetString(reader.GetOrdinal("wikidata_id"));
			locality.Name						= reader.GetString(reader.GetOrdinal("name"));
			locality.Description				= reader.GetString(reader.GetOrdinal("description"));

			double? latitude					= reader.GetDouble(reader.GetOrdinal("latitude"));
			double? longitude					= reader.GetDouble(reader.GetOrdinal("longitude"));

			if (latitude != null && longitude != null)
			{
				locality.Point = new GeoPoint((double)latitude, (double)longitude);
			}

			locality.Elevation					= reader.GetDouble(reader.GetOrdinal("elevation"));
			locality.Population					= reader.GetInt32(reader.GetOrdinal("population"));

			locality.CountryCode				= reader.GetString(reader.GetOrdinal("country_code"));

			double? timeZone					= reader.SafeDouble("time_zone");

			if (timeZone != null)
			{
				locality.TimeZone = (double)timeZone;
			}

			locality.GeoNamesFeatureClass		= (GeoNamesFeatureClass)Enum.Parse(typeof(GeoNamesFeatureClass), reader.GetString(reader.GetOrdinal("geonames_feature_class")));
			locality.GeoNamesFeatureCode		= (GeoNamesFeatureCode)Enum.Parse(typeof(GeoNamesFeatureCode), reader.GetString(reader.GetOrdinal("geonames_feature_code")));
			locality.OpenStreetMapPlaceCategory = (OpenStreetMapPlaceCategory)Enum.Parse(typeof(OpenStreetMapPlaceCategory), reader.GetString(reader.GetOrdinal("osm_place_category")));

			string text							= reader.SafeString("bounding_box");

			if (!System.String.IsNullOrEmpty(text))
			{
				try
				{
					locality.BoundingBox		= GeoRectangle.Parse(text);
				}
				catch (Exception)
				{
					locality.BoundingBox = null;
				}
			}

			byte[] bytes = reader.SafeBytes("polygon");

			if (bytes != null)
			{
				try
				{
					locality.Polygon			= GeoPolygon.FromByteArray(bytes);
				}
				catch (Exception)
				{
					locality.Polygon = null;
				}
			}

			return locality;
		}
		#endregion
	}
}
