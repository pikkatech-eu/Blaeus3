/***********************************************************************************
* File:         BlaeusDatabaseException.cs                                         *
* Contents:     Class BlaeusDatabaseException                                      *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2025-02-12 15:53                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Blaeus.Library.Domain;

namespace Blaeus.Library.Exceptions
{
	public class BlaeusDatabaseException : Exception
	{
		#region Properties
		internal GeoLocality GeoLocality	{get;set;} = null;
		private string _text;
		#endregion

		#region Construction
		public BlaeusDatabaseException(string text, GeoLocality locality = null)
		{
			this._text			= text;
			this.GeoLocality	= locality;
		}

		public BlaeusDatabaseException() : this("Default")	{}
		#endregion

		public override string Message
		{
			get
			{
				if (this.GeoLocality != null)
				{
					return $"{this._text}: GN ID={this.GeoLocality.GeonamesId}, " +
						$"OSM RID={this.GeoLocality.OpenStreetMapRelationId}, " +
						$"WID={this.GeoLocality.WikiDataId}, " +
						$"Name={this.GeoLocality.Name}";
				}
				else
				{
					return this._text;
				}
			}
		}
	}
}
