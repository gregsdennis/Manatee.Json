namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Available formatting options for serializing DateTime objects.
	/// </summary>
	public enum DateTimeSerializationFormat
	{
		/// <summary>
		/// Output conforms to ISO 8601 formatting: YYYY-MM-DDThh:mm:ss.sTZD (e.g. 1997-07-16T19:20:30.45+01:00)
		/// </summary>
		Iso8601,
		/// <summary>
		/// Output is a string in the format "/Date([ms])/", where [ms] is the number of milliseconds
		/// since January 1, 1970 UTC.
		/// </summary>
		JavaConstructor,
		/// <summary>
		/// Output is a numeric value representing the number of milliseconds since January 1, 1970 UTC.
		/// </summary>
		Milliseconds,
		/// <summary>
		/// Output is formatted using the <see cref="JsonSerializerOptions.CustomDateTimeSerializationFormat"/> property.
		/// </summary>
		Custom,
	}
}
