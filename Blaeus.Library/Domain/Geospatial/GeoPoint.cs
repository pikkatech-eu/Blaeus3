/***********************************************************************************
* File:         GeoPoint.cs                                                        *
* Contents:     Class GeoPoint                                                     *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-12 11:23                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Factotum.Xml;
using EF = Factotum.Maths.ElementaryFunctions;
using MC = Factotum.Maths.Constants;

namespace Blaeus.Domain.Geospatial
{
	/// <summary>
	/// Geospatial point.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GeoPoint
	{
		#region Static constants
		private static readonly Regex RX_DMS			= new Regex(@"(?'deg'[+|-]?[0-9]{1,3})°\s*((?'min'[0-9]{1,2})')?\s*((?'sec'[.0-9]*)"")?(?'card'[NSEW])?");
		private static readonly Regex RX_CARDINAL_POINT = new Regex(@"(?'value'[.0-9]+)\s*(?'card'[NSEW])");
		#endregion

		/// <summary>
		/// Number of decimals to use for output strings.
		/// </summary>
		public static int Decimals {get;set;}  = 4;

		#region Properties
		/// <summary>
		/// In degrees, from -90.0 to 90.0 .
		/// </summary>
		public double Latitude	{get;set;} = 0;

		/// <summary>
		/// In degrees, from -180.0 to 180.0 .
		/// </summary>
		public double Longitude	{get;set;} = 0;
		#endregion

		#region Construction
		public GeoPoint(double latitude, double longitude)
		{
			this.Latitude	= latitude;
			this.Longitude	= longitude;
		}

		public GeoPoint(GeoPoint point): this(point.Latitude, point.Longitude)	{}

		/// <summary>
		/// Default (Guinea Bay) constructor.
		/// </summary>
		public GeoPoint()	{}
		#endregion

		/// <summary>
		/// TODO: add comments
		/// </summary>
		/// <param name="tuple"></param>
		public static implicit operator GeoPoint((double Latitude, double Longitude) tuple) => new GeoPoint(tuple.Latitude, tuple.Longitude);

		/// <summary>
		/// Sometimes coordinates are given in pairs (lat, lon), sometimes as (lon, lat).
		/// To correct the latter, use this method.
		/// </summary>
		internal void SwapCoordinates()
		{
			double latitude = this.Latitude;
			this.Latitude	= this.Longitude;
			this.Longitude	= latitude;
		}

		#region String Representations
		/// <summary>
		/// 
		/// </summary>
		/// <param name="format">
		///		Case-insensitive. Supported are three output formats.
		///		"DEGREES" / "DEG" / "D": result contains coordinates in degree measure, as in "30°15'23.1234".
		///		"GEOGRAPHIC" / "GEO" / "G": result contains coordinates in degree measure, with compass points instead of signs, as in "30°15'23.1234 S".
		///		"NUMERIC" / "NUM" / "N" / anything else: result contaons coordinates in numeric form as in "30.2564".
		///		If contains "C", the compass points will be used to indicate the signs ("30°15'23.1234 S"), otherwise the number form will be used.
		///		If contains "P", the resulting string will be taken into round parentheses like "(51.08, 1,42)".
		///		If contains "Q", the resulting string will be taken into round brackets like "[51.08, 1,42]".
		/// </param>
		/// <param name="brackets">
		///		Defines optional brackets. Case-insensitive.
		///		"ROUND" / "R" : the result is taken into round brackets, as in "(51.08, 1,42)".
		///		"SQUARE" / "S" : the result is taken into square brackets, as in "[51.08, 1,42]".
		///		"NONE" / "N" / default: the result will have no brackets, as in "51.08, 1,42".
		///	</param>
		/// <returns>GeoPoint represented as string.</returns>
		public string ToString(string format, string brackets = "")
		{
			string latitude		= this.Latitude.ToString();
			string longitude	= this.Longitude.ToString();

			switch (format.ToUpper())
			{
				case "DEGREES":
				case "DMS":
				case "D":
					latitude	= ToDegreeForm(this.Latitude, false, false);
					longitude	= ToDegreeForm(this.Longitude, false, true);
					break;

				case "GEOGRAPHIC":
				case "GEO":
				case "G":
					latitude	= ToDegreeForm(this.Latitude, true, false);
					longitude	= ToDegreeForm(this.Longitude, true, true);
					break;

				case "NUMERIC":
				case "NUM":
				case "N":
				default:
					break;
			}

			string result = $"{latitude},{longitude}";

			switch (brackets.ToUpper())
			{
				case "ROUND":
				case "R":
					return $"({result})";

				case "SQUARE":
				case "S":
					return $"[{result}]";

				case "NONE":
				case "N":
				default:
					return result;
			}
		}

		/// <summary>
		/// Default string representation.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ToString("N");
		}

		/// <summary>
		/// Parses a source string to a GeoPoint.
		/// Supported are the string representations mentioned with ToString(format, brackets).
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>An instant of GeoPoint, if successful</returns>
		/// <exception cref="ArgumentException">Thrown if the source string is not in a supported format.</exception>
		public static GeoPoint Parse(string source)
		{
			source = source.TrimStart(new char[]{'('}).TrimEnd(new char[]{')'}).Trim();
			source	= source.TrimStart('[').TrimEnd(']');

			string[] components = source.Split(',');

			if (components.Length != 2)
			{
				throw new ArgumentException("The string does not represent geographic coordinates.");
			}

			try
			{
				string latitude		= components[0].Trim();
				string longitude	= components[1].Trim();

				GeoPoint point		= new GeoPoint();

				if (latitude.Contains("°"))
				{
					// DMS measure
					(int Degree, int Minute, double Second) value = ResolveAngle(latitude);
					point.Latitude = FromDegreeForm(value);
				}
				else
				{
					char lastChar = latitude[^1];

					if ("NSEW".Contains(lastChar))
					{
						latitude = latitude[..^1];
					}

					double d = Double.Parse(latitude);

					if (lastChar == 'S')
					{
						d = -d;
					}

					point.Latitude = d;
				}
			
				if (longitude.Contains("°"))
				{
					// DMS measure
					(int Degree, int Minute, double Second) value = ResolveAngle(longitude);
					point.Longitude = FromDegreeForm(value);
				}
				else
				{
					char lastChar = longitude[^1];

					if ("NSEW".Contains(lastChar))
					{
						longitude = longitude[..^1];
					}

					double d = Double.Parse(longitude);

					if (lastChar == 'W')
					{
						d = -d;
					}

					point.Longitude = d;
				}

				return point;
			}
			catch 
			{
				throw new ArgumentException("The string does not represent geographic coordinates.");
			}
		}

		/// <summary>
		/// Tries to parse a source string.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="point">The instance of GeoPoint, if successful, otherwise undefinde.</param>
		/// <returns>True, if parsing succeeds.</returns>
		public static bool TryParse(string source, out GeoPoint point)
		{
			try
			{
				point = Parse(source);
				return true;
			}
			catch (Exception)
			{
				point = new GeoPoint();
				return false;
			}
		}
		#endregion

		#region Calculations
		/// <summary>
		/// Direction, in degrees, from GeoPoint poin1 to GeoPoint point2 [RDM (12.4)].
		/// </summary>
		/// <param name="point1">The first GeoPoint.</param>
		/// <param name="point2">The second GeoPoint.</param>
		/// <returns>Direction from point1 to point2, in degrees.</returns>
		public static double Direction(GeoPoint point1, GeoPoint point2)
		{
			double denominator = EF.Cosd(point1.Latitude) * EF.Tand(point2.Latitude) - 
								 EF.Sind(point1.Latitude) * EF.Cosd(point1.Longitude - point2.Longitude); // (RDM 12.4))

			if (Math.Abs(denominator) < MC.EPSILON)
			{
				return 0;
			}
			else
			{
				double x = EF.Sind(point2.Longitude - point1.Longitude) / denominator;

				if (denominator < 0)
				{
					return MC.RADIAN * EF.Arctan(x, 2) % (2 * MC.PI);
				}
				else
				{
					return MC.RADIAN * EF.Arctan(x, 1) % (2 * MC.PI);
				}
			}
		}

		/// <summary>
		/// Great-circle distance between two GeoPoints (https://en.wikipedia.org/wiki/Great-circle_distance).
		/// Uses the haversine formula (https://en.wikipedia.org/wiki/Haversine_formula).
		/// Source: https://rosettacode.org/wiki/Haversine_formula .
		/// </summary>
		/// <param name="point1">The first GeoPoint.</param>
		/// <param name="point2">The second GeoPoint.</param>
		/// <returns>Distance between the GeoPoints, km.</returns>
		public static double Distance(GeoPoint point1, GeoPoint point2)
		{
			double dLat = point2.Latitude - point1.Latitude;
			double dLon = point2.Longitude - point1.Longitude;
			double lat1 = point1.Latitude;
			double lat2 = point2.Latitude;

			double a = EF.Sind(dLat / 2) * EF.Sind(dLat / 2) + EF.Sind(dLon / 2) * EF.Sind(dLon / 2) * EF.Cosd(lat1) * EF.Cosd(lat2);
			double c = 2 * Math.Asin(Math.Sqrt(a));

			return MC.EARTH_RADIUS * 2 * Math.Asin(Math.Sqrt(a));
		}

		/// <summary>
		/// Direction, in degrees, from this GeoPoint to GeoPoint point.
		/// </summary>
		/// <param name="point">The point to calculate direction to.</param>
		/// <returns>Direction from this GeoPoint to point, in degrees.</returns>
		public double DirectionTo(GeoPoint point)
		{
			return Direction(this, point);
		}

		/// <summary>
		/// Great-circle distance between this GeoPoint and GeoPoint point.
		/// Added for convenience. Calculates the distance using the haversine formula.
		/// </summary>
		/// <param name="point">The GeoPoint to calculate the distance to.</param>
		/// <returns>Distance to the GeoPoint point, km.</returns>
		public double Distance(GeoPoint point)
		{
			return Distance(this, point);
		}
		#endregion

		#region Internal Functionality
		/// <summary>
		///		Shifts the point by values given as a GeoPoint.
		/// </summary>
		/// <param name="p">GeoPoint containing the shift amounts.</param>
		/// <returns>The point with the coordinates shifted.</returns>
		internal GeoPoint Shift(GeoPoint p)
		{
			return new GeoPoint(this.Latitude - p.Latitude, this.Longitude - p.Longitude);
		}
		#endregion

		#region XML
		/// <summary>
		/// XElement representation of the instance.
		/// </summary>
		/// <returns>An instance of XElement representing this GeoPoint instance.</returns>
		public XElement ToXElement()
		{
			XElement x = new XElement("GeoPoint");
			x.Add(new XAttribute("Latitude", this.Latitude));
			x.Add(new XAttribute("Longitude", this.Longitude));

			return x;
		}

		/// <summary>
		/// Creates an instance of GeoPoint from an XElement.
		/// </summary>
		/// <param name="x">The XElement to create from.</param>
		/// <returns>The instance ot GeoPoint, if successful.</returns>
		public static GeoPoint FromXElement(XElement x)
		{
			GeoPoint point = new GeoPoint();

			point.Latitude	= x.AttributeValue<double>("Latitude");
			point.Longitude	= x.AttributeValue<double>("Longitude");

			return point;
		}
		#endregion

		#region Internal Auxiliary
		/// <summary>
		/// Converts the value of an angle given in the DMS form, into a number.
		/// </summary>
		/// <param name="value">The DMS value of the angle.</param>
		/// <returns>The value of the angle, degrees.</returns>
		internal static double FromDegreeForm((int Degree, int Minute, double Second) value)
		{
			double result = Math.Abs(value.Degree) + (double)value.Minute / 60 + value.Second / 3600;
			if (value.Degree < 0)
			{
				result = -result;
			}

			return result;
		}

		/// <summary>
		/// Converts an angle into a string representing it in DMS form.
		/// </summary>
		/// <param name="angle">The angle to convert, degrees.</param>
		/// <param name="useCompassPoints">If set to true, uses compass points "NSEW" instead of the signs.</param>
		/// <param name="isLongitude">If set to true, the value is a longitude.</param>
		/// <returns>The string representing of the angle in DMS form</returns>
		internal static string ToDegreeForm(double angle, bool useCompassPoints, bool isLongitude)
		{
			double alpha	= Math.Abs(angle) % 360;
			int degree		= (int)alpha;
			alpha			-= degree;
			int minute		= (int)(60 * alpha);
			alpha			*= 60;
			alpha			-= minute;

			double second	= alpha * 60;

			second			= Math.Round(second, Decimals);

			if (useCompassPoints)
			{
				string sign	= "";

				if (isLongitude)
				{
					sign	= angle > 0 ? "E" : "W";
				}
				else
				{
					sign	= angle > 0 ? "N" : "S";
				}

				return $"{Math.Abs(degree)}°{minute}'{second}\"{sign}";
			}
			else
			{
				if (angle < 0)
				{
					degree = -degree;
				}

				return $"{degree}°{minute}'{second}\"";
			}
		}

		/// <summary>
		/// Resolves a string containing an angle in DMS form, to a tuple (Degree, Minute, Second).
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>The tuple (Degree, Minute, Second) if successful, otherwise throws an exception.</returns>
		internal static (int Degree, int Minute, double Second) ResolveAngle(string source)
		{
			Match m			 = RX_DMS.Match(source);
			string strDegree = m.Groups["deg"].Value;
			string strMinute = m.Groups["min"].Value;
			string strSecond = m.Groups["sec"].Value;
			string strCard	 = m.Groups["card"].Value;

			try
			{
				int degree = Int32.Parse(strDegree);
				int minute = 0;
				double second = 0;

				if (strMinute != null)
				{
					minute = Int32.Parse(strMinute);
				}

				if (strSecond != null)
				{
					second = Double.Parse(strSecond);
				}

				if (strCard != null)
				{
					if (strCard == "S" || strCard == "W")
					{
						degree = -Math.Abs(degree);
					}
				}

				return (degree, minute, second);
			}
			catch (Exception)
			{
				throw;
			}
		}
		#endregion
	}
}
