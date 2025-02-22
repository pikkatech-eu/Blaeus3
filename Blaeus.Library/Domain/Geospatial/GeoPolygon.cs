/***********************************************************************************
* File:         GeoPolygon.cs                                                      *
* Contents:     Class GeoPolygon                                                   *
* Author:       Stanislav "Bav" Koncebovski (stanislav@pikkatech.eu)               *
* Date:         2024-10-12 16:55                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using System.Xml.Linq;
using Factotum.Xml;

namespace Blaeus.Domain.Geospatial
{
	/// <summary>
	/// Geospatial polygon.
	/// </summary>
	public class GeoPolygon
	{
		#region Internal Members
		/// <summary>
		/// List of the polygon's vertices.
		/// </summary>
		internal List<GeoPoint> _vertices = new List<GeoPoint>();
		#endregion

		#region Properties
		/// <summary>
		/// Gets the polygon's vertices as an array.
		/// </summary>
		public GeoPoint[] Vertices
		{
			get
			{
				return this._vertices.ToArray();
			}
		}
		#endregion

		#region Indexers
		/// <summary>
		/// Gets the vertice of the polygon with a given index.
		/// </summary>
		/// <param name="i">The index.</param>
		/// <returns>The vertice under the index.</returns>
		public GeoPoint this[int i]
		{
			get
			{
				return this._vertices[i];
			}

			internal set
			{
				this._vertices[i] = value;
			}
		}
		#endregion

		#region Construction
		/// <summary>
		/// Creates a geospatial polygon from a collection of geospatial points taking them as vertices.
		/// </summary>
		/// <param name="points">The points to transform to vertices.</param>
		public GeoPolygon(IEnumerable<GeoPoint> points)
		{
			this._vertices = points.ToList();
		}

		/// <summary>
		/// Copying constructor.
		/// Creates a verbatim copy of a geospatial polygon.
		/// </summary>
		/// <param name="polygon">The geospatial polygon to copy.</param>
		public GeoPolygon(GeoPolygon polygon) : this(polygon._vertices)
		{

		}

		/// <summary>
		/// Default constructor.
		/// Creates a geospatial polygon with no vertices.
		/// </summary>
		public GeoPolygon()	{}

		/// <summary>
		/// "Lazy" coordinate constructor.
		/// Creates a polygon from a number of double values that are supposed to follow in pairs (lat_1, lon_1), (lat_2, lon_2), ... .
		/// If the number of values is odd, the last value is ignored.
		/// </summary>
		/// <param name="coordinates">An array of double values representing the coordinates of the vertices, arranged in pairs.</param>
		public GeoPolygon(params double[] coordinates)
		{
			for (int i = 0; i < coordinates.Length; i+=2)
			{
				double latitude		= coordinates[i];
				double longitude	= coordinates[i + 1];

				this._vertices.Add(new GeoPoint(latitude, longitude));
			}
		}
		#endregion

		#region Calculations
		/// <summary>
		/// The bounding box of the geospatial polygon.
		/// </summary>
		public GeoRectangle BoundingBox
		{
			get
			{
				double minLat	= Double.MaxValue;
				double maxLat	= Double.MinValue;
				double minLong	= Double.MaxValue;
				double maxLong	= Double.MinValue;

				foreach (GeoPoint point in this._vertices)
				{
					if (point.Latitude < minLat)
					{
						minLat = point.Latitude;
					}

					if (point.Latitude > maxLat)
					{
						maxLat = point.Latitude;
					}

					if (point.Longitude < minLong)
					{
						minLong = point.Longitude;
					}

					if (point.Longitude > maxLong)
					{
						maxLong = point.Longitude;
					}
				}

				return new GeoRectangle(minLong, maxLat, maxLong, minLat);
			}
		}

		/// <summary>
		/// Surface, in km2.
		/// Area of a 2Ds polygon:
		/// \f[
		/// A = \frac{1}{2} \cdot \vert \sum_{i=1}^{N-1} \left( x_i y_{i+1} - y_i x_{i+1} \right) \vert
		/// \f]
		/// Source: Joseph O'Rourke, Computational Geometry in C, ISBN 0521440343, Section (1.9) p.22.
		/// </summary>
		public double Surface
		{
			get
			{
				double sum	= 0;

				for (int i = 0; i < this._vertices.Count - 1; i++)
				{
					sum	+= this._vertices[i].Longitude * this._vertices[i + 1].Latitude - this._vertices[i].Latitude * this._vertices[i + 1].Longitude;
				}

				sum		+= this._vertices[this._vertices.Count - 1].Longitude * this._vertices[0].Latitude - this._vertices[this._vertices.Count - 1].Latitude * this._vertices[0].Longitude;

				return 0.5 * Math.Abs(sum) * GeoRectangle.SURFACE_FACTOR;
			}
		}

		/// <summary>
		/// Check if a probe point is within the polygon.
		/// The routine implements the algorithm as published in Joseph O'Rourke. Computational Geometry in C. ISBN 0-521-44034-2, p. 235.
		/// </summary>
		/// <param name="point">The probe point.</param>
		/// <returns>True if the probe point is within the rectangle.</returns>
		/// <remarks>Tested superficially and seemed to work. Needs more testing.</remarks>
		public bool Contains(GeoPoint point)
		{
			// Working copy of the polygon.
			GeoPolygon P = new GeoPolygon(this);

			int crossings = 0;

			// Shift the working copy of the polygon so that the probe point is the origin.
			for (int i = 0; i < this._vertices.Count; i++)
			{
				P[i] = P[i].Shift(point);
			}

			// For each edge e = (i - 1, i) see if it crosses ray.
			for (int i = 0; i < P._vertices.Count; i++)
			{
				int i1 = (i + P._vertices.Count - 1) % P._vertices.Count;

				if (
						(P[i].Latitude > 0  && P[i1].Latitude <= 0) || 
						(P[i1].Latitude > 0 && P[i].Latitude <= 0)
					)
				{
					// e straddles ray, so compute intersection with ray
					double x = (P[i].Longitude * P[i1].Latitude - P[i1].Longitude * P[i].Latitude) / (P[i1].Latitude - P[i].Latitude);

					// crosses ray if strictly positive intersection
					if (x > 0)
					{ 
						crossings++;
					}
				}
			}

			return crossings % 2 == 1;
		}

		/// <summary>
		/// Containment of a rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle to check for containment in the polygon.</param>
		/// <returns>
		///		True, if all vertices of the rectangle are within the polygon.
		///	</returns>
		///	<remarks>
		///		For non-convex polygons, a rectangle can be not completely contained ba the polygon even if all of its corners are contained.
		///	</remarks>
		public bool Contains(GeoRectangle rectangle)
		{
			return this.Contains(rectangle.NorthEast) && 
				   this.Contains(rectangle.NorthWest) && 
				   this.Contains(rectangle.SouthEast) && 
				   this.Contains(rectangle.SouthWest);
		}

		/// <summary>
		/// Containment of a polygon.
		/// </summary>
		/// <param name="polygon">The probe polygon to check for containment in this polygon.</param>
		/// <returns>True, if all vertices of the probe polygon are within this polygon.</returns>
		///	<remarks>
		///		For non-convex polygons, a polygon can be not completely contained ba the other polygon even if all of its vertices are contained.
		///	</remarks>
		public bool Contains(GeoPolygon polygon)
		{
            foreach (GeoPoint vertex in polygon.Vertices)
            {
                if (this.Contains(vertex))
				{
					return false;
				}
            }

			return true;
        }
		#endregion

		#region XML
		/// <summary>
		/// XElement representation of the instance.
		/// </summary>
		/// <returns>An instance of XElement representing this GeoPolygon instance.</returns>
		public XElement ToXElement()
		{
			XElement x = new XElement("GeoPolygon");

			foreach (GeoPoint vertex in this._vertices)
			{
				x.Add(vertex.ToXElement().ChangeName("Vertex"));
			}

			return x;
		}

		/// <summary>
		/// Creates an instance of GeoPolygon from an XElement.
		/// </summary>
		/// <param name="x">The XElement to create from.</param>
		/// <returns>The instance ot GeoPolygon, if successful.</returns>
		public static GeoPolygon FromXElement(XElement x)
		{
			GeoPolygon polygon = new GeoPolygon();

			foreach (XElement xVertex in x.Elements("Vertex"))
			{
				polygon._vertices.Add(GeoPoint.FromXElement(xVertex));
			}

			return polygon;
		}
		#endregion

		#region JSON
		/// <summary>
		/// Expected JSON format is as downloaded from an OSM query:
		/// {
		///		...
		///		"geometry": {
        ///		"type": "Polygon",
        ///		"coordinates": [
        ///    [
        ///        [
        ///            -0.5103751,
        ///            51.4680873
        ///        ],
		///        ...
		///        ]]}
		/// </summary>
		/// <param name="source">The string to parse to JSON.</param>
		/// <param name="isLatitudeFirst">
		///		If set to true, the first number in a coordinate pair is the latitude, otherwise longitude.
		///		JSON files requested from Openstreetmap have longitude first.
		///	</param>
		/// <returns>Instance of GeoPolygon if successful.</returns>
		public static GeoPolygon FromJson(string source, bool isLatitudeFirst)
		{
			try
			{
				dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(source);
			
				var coordinates = json["geometry"]["coordinates"][0];

				List<GeoPoint>	points = new List<GeoPoint>();

				foreach (var item in coordinates)
				{
					GeoPoint point = new GeoPoint();

					if (isLatitudeFirst)
					{
						point.Latitude	= item[0];
						point.Longitude = item[1];
					}
					else
					{
						point.Latitude	= item[1];
						point.Longitude = item[0];
					}

					points.Add(point);
				}

				GeoPolygon polygon	= new GeoPolygon(points);

				return polygon;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Loads JSON polygon from a JSON file.
		/// The structure of the file must be as defined above.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="isLatitudeFirst">
		///		If set to true, the first number in a coordinate pair is the latitude, otherwise longitude.
		///	</param>
		/// <returns>Instance of GeoPolygon if successful.</returns>
		public static GeoPolygon Load(string fileName, bool isLatitudeFirst)
		{
			using (StreamReader reader = new StreamReader(fileName))
			{
				string source = reader.ReadToEnd();

				return FromJson(source, isLatitudeFirst);
			}
		}

		#endregion

		#region Binary Storage
		/// <summary>
		/// Converts the instance of GeoPolygon to a byte array.
		/// Needed for storage on the database.
		/// </summary>
		/// <returns>The byte array out of the instance of GeoPolygon.</returns>
		public byte[] ToByteArray()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					writer.Write(this._vertices.Count);

					foreach (GeoPoint point in this._vertices)
					{
						writer.Write(point.Latitude);
						writer.Write(point.Longitude);
					}
				}

				return ms.ToArray();
			}
		}

		/// <summary>
		/// Creates an instance of GeoPolygon from a byte array.
		/// Needed for storage on the database.
		/// </summary>
		/// <param name="bytes">The byte array.</param>
		/// <returns>An instance of GeoPolygon, if succeeds.</returns>
		public static GeoPolygon FromByteArray(byte[] bytes)
		{
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				using (BinaryReader reader = new BinaryReader(ms))
				{
					GeoPolygon polygon = new GeoPolygon();

					int count = reader.ReadInt32();

					for (int i = 0; i < count; i++)
					{
						double latitude = reader.ReadDouble();
						double longitude = reader.ReadDouble();

						GeoPoint point = new GeoPoint(latitude, longitude);

						polygon._vertices.Add(point);
					}

					return polygon;
				}
			}
		}
		#endregion

		#region String representations
		/// <summary>
		/// Returns a string containing short string representations of the verices separated by semicolon.
		/// </summary>
		/// <returns>Polygon string, as defined above.</returns>
		public override string ToString()
		{
			string result = "";

			foreach (GeoPoint vertex in this._vertices)
			{
				result += $"{vertex.ToString()};";
			}

			return result.TrimEnd(';');
		}

		/// <summary>
		/// Parses a string to a GeoPolygon.
		/// </summary>
		/// <param name="source">The string to parse.</param>
		/// <returns>The resulting polygon, if successful, otherwise throws an exception.</returns>
		public static GeoPolygon Parse(string source)
		{
			string[] vertexStrings = source.Split(';');

			return new GeoPolygon(vertexStrings.Select(v => GeoPoint.Parse(v)));
		}

		/// <summary>
		/// Tries to parse a string to a GeoPolygon.
		/// </summary>
		/// <param name="source">The string to parse.</param>
		/// <param name="polygon">The resulting polygon, if successful, otherwise null.</param>
		/// <returns>True, if the polygon could be recovered from string.</returns>
		public static bool TryParse(string source, out GeoPolygon polygon)
		{
			try
			{
				polygon = GeoPolygon.Parse(source);
				return true;
			}
			catch (Exception)
			{
				polygon = null;
				return false;
			}
		}
		#endregion
	}
}
