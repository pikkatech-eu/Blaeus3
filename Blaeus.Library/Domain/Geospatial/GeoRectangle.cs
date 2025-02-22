/***********************************************************************************
* File:         GeoRectangle.cs                                                    *
* Contents:     Class GeoRectangle                                                 *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-12 11:55                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Factotum.Xml;
using MC = Factotum.Maths.Constants;

[assembly: InternalsVisibleTo("Blaeus.Tests")]
[assembly: InternalsVisibleTo("Blaeus.Tests")]
[assembly: InternalsVisibleTo("Blaeus.Demo")]

namespace Blaeus.Domain.Geospatial
{
	/// <summary>
	/// Geospatial rectangle.
	/// </summary>
	public class GeoRectangle
	{
		/// <summary>
		/// Result of division of the Earth's surface (5.112 10^8 km2) through 129600 (360 * 360 degrees), for the approximate calculation of surfaces.
		/// </summary>
		internal const double SURFACE_FACTOR = 3944.4444;

		/// <summary>
		/// Corresponds to approximately 100m latitude / longitude at the Earth equator.
		/// </summary>
		public static double CoordinatePrecision {get;set;} = 1e-3;

		#region Properties
		/// <summary>
		/// In geographical degrees.
		/// </summary>
		public double East	{get;internal set;}	= 0;

		/// <summary>
		/// In geographical degrees.
		/// </summary>
		public double North	{get;internal set;}	= 0;

		/// <summary>
		/// In geographical degrees.
		/// </summary>
		public double West	{get;internal set;}	= 0;

		/// <summary>
		/// In geographical degrees.
		/// </summary>
		public double South	{get;internal set;}	= 0;

		/// <summary>
		/// Top Left point of the rectangle.
		/// </summary>
		public GeoPoint NorthWest	{get{return new GeoPoint(this.North, this.West);}}

		/// <summary>
		/// Bottom Left point of the rectangle.
		/// </summary>
		public GeoPoint SouthWest	{get{return new GeoPoint(this.South, this.West);}}

		/// <summary>
		/// Bottom Right point of the rectangle.
		/// </summary>
		public GeoPoint SouthEast	{get{return new GeoPoint(this.South, this.East);}}

		/// <summary>
		/// Top Right point of the rectangle.
		/// </summary>
		public GeoPoint NorthEast	{get{return new GeoPoint(this.North, this.East);}}

		/// <summary>
		/// Longitude span, in geographical degrees.
		/// </summary>
		public double LongitudeSpan
		{
			get	{return this.East - this.West;}
		}

		/// <summary>
		/// Latitude span, in geographical degrees.
		/// </summary>
		public double LatitudeSpan
		{
			get	{return this.North - this.South;}
		}

		/// <summary>
		/// Longitudinal width, in km (for Earth).
		/// </summary>
		public double Width
		{
			get
			{
				return this.LongitudeSpan * MC.EARTH_RADIUS * MC.PI / 180;
			}
		}

		/// <summary>
		/// Latitudinal width, in km (for Earth).
		/// </summary>
		public double Height
		{
			get
			{
				return this.LatitudeSpan * MC.EARTH_RADIUS * MC.PI / 180;
			}
		}

		/// <summary>
		/// The central point of the geospatial rectangle.
		/// </summary>
		public GeoPoint Centroid
		{
			get
			{
				return new GeoPoint(0.5 * (this.South + this.North), 0.5 * (this.West + this.East));
			}
		}

		/// <summary>
		/// Approximate formula for small rectangles.
		/// Gets the surface of the rectangle in km2 (for the planet defined in GeoData).
		/// </summary>
		public double Surface
		{
			get
			{
				return Math.Abs(this.East - this.West) * Math.Abs(this.North - this.South) * SURFACE_FACTOR;
			}
		}
		#endregion

		#region Construction
		/// <summary>
		/// Data constructor.
		/// </summary>
		/// <param name="west">The west boundary.</param>
		/// <param name="north">The north boundary.</param>
		/// <param name="east">The east boundary.</param>
		/// <param name="south">The south boundary.</param>
		public GeoRectangle(double west, double north, double east, double south)
		{
			this.West	= west;
			this.North	= north;
			this.East	= east;
			this.South	= south;
		}

		/// <summary>
		/// Guinea bay constructor.
		/// Creates a rectangle of zero dimension.
		/// </summary>
		public GeoRectangle()	{}
		#endregion

		#region Validation
		/// <summary>
		/// Validation concept:
		/// the coordinates must be within their geographically defined values;
		/// additionally the east value must be greater or equal to the west value;
		/// the north value must be greater or equal to the south value;
		/// </summary>
		/// <returns>True, if the above condition holds.</returns>
		public bool IsValid()
		{
			return this.West >= -180 && this.West <= 180 &&
				   this.East >= -180 && this.East <= 180 &&
				   this.North >= -90 && this.North <= 90 &&
				   this.South >= -90 && this.South <= 90 &&
				   this.East >= this.West &&
				   this.North >= this.South;
		}
		#endregion

		#region Containment
		/// <summary>
		/// Containment of a point in the geospatial rectangle.
		/// </summary>
		/// <param name="latitude">The latitude of the point.</param>
		/// <param name="longitude">The longitude of the point.</param>
		/// <returns>True if the point is within the rectangle, with the default precision</returns>
		public bool Contains(double latitude, double longitude)
		{
			if (!this.IsValid())
			{
				return false;
			}

			return longitude >= this.West - CoordinatePrecision	&&
				   longitude <= this.East + CoordinatePrecision	&& 
				   latitude >= this.South - CoordinatePrecision	&& 
				   latitude <= this.North + CoordinatePrecision;
		}

		/// <summary>
		/// Added for convenience.
		/// Containment of a geospatial point in the geospatial rectangle.
		/// </summary>
		/// <param name="point">The point to check for containment.</param>
		/// <returns>True if the point is within the georectangle.</returns>
		public bool Contains(GeoPoint point)
		{
			return this.Contains(point.Latitude, point.Longitude);
		}

		/// <summary>
		/// Checks whether a geospatial rectangle is contained within this geospatial rectangle.
		/// </summary>
		/// <param name="rectangle">The geospatial rectangle to check the containment of to.</param>
		/// <returns>True if the geospatial rectangle is contained within this geospatial rectangle, with default precision.</returns>
		public bool Contains(GeoRectangle rectangle)
		{
			return this.Contains(rectangle.NorthWest)	&& 
				   this.Contains(rectangle.NorthEast)	&&
				   this.Contains(rectangle.SouthWest)	&&
				   this.Contains(rectangle.SouthEast);
		}
		#endregion

		#region String Representation
		/// <summary>
		/// Default string representation, as in "-1.00, 52.5, +1.00, 52.0".
		/// </summary>
		/// <returns>String representation, as defined above.</returns>
		public override string ToString()
		{
			return $"{this.West:F4},{this.North:F4},{this.East:F4},{this.South:F4}";
		}

		/// <summary>
		/// Parses a source string to a geospatial rectangle.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>The instance of geospatial rectangle parsed, if succeeds, otherwise null.</returns>
		public static GeoRectangle Parse(string source)
		{
			try
			{
				string[] cells = source.Split(',');

				if (cells.Length == 4)
				{
					double west		= Double.Parse(cells[0]);
					double north	= Double.Parse(cells[1]);
					double east		= Double.Parse(cells[2]);
					double south	= Double.Parse(cells[3]);

					return new GeoRectangle(west, north, east, south);
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Tries to parse source string to a geospatial rectangle.
		/// </summary>
		/// <param name="source">The source string to parse.</param>
		/// <param name="rectangle">The instance of geospatial rectangle parsed, if succeeds, otherwise undefined.</param>
		/// <returns>True, if parsing succeeds.</returns>
		public static bool TryParse(string source, out GeoRectangle rectangle)
		{
			try
			{
				rectangle = Parse(source);
				return true;
			}
			catch (Exception)
			{
				rectangle = null;
				return false;
			}
		}
		#endregion

		#region XML
		/// <summary>
		/// XElement representation of the instance.
		/// </summary>
		/// <returns>An instance of XElement representing this GeoRectangle instance.</returns>
		public XElement ToXElement()
		{
			XElement x = new XElement("GeoRectangle");
			x.AppendAttribute("West", this.West);
			x.AppendAttribute("North", this.North);
			x.AppendAttribute("East", this.East);
			x.AppendAttribute("South", this.South);

			return x;
		}

		/// <summary>
		/// Creates an instance of GeoRectangle from an XElement.
		/// </summary>
		/// <param name="x">The XElement to create from.</param>
		/// <returns>The instance ot GeoRectangle, if successful.</returns>
		public static GeoRectangle FromXElement(XElement x)
		{
			GeoRectangle result = new GeoRectangle();

			result.West	= x.AttributeValue<double>("West");
			result.North	= x.AttributeValue<double>("North");
			result.East	= x.AttributeValue<double>("East");
			result.South	= x.AttributeValue<double>("South");

			return result;
		}
		#endregion
	}
}
