namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides implementers the option to set a preferred method for serialization.
	/// </summary>
	public interface IJsonSerializable
	{
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		void FromJson(JsonValue json, JsonSerializer serializer);
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		JsonValue ToJson(JsonSerializer serializer);
	}
}