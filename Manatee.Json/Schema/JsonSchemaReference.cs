using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a reference to a schema.
	/// </summary>
	public class JsonSchemaReference : IJsonSchema
	{
		/// <summary>
		/// Defines a reference to the root schema.
		/// </summary>
		private static readonly Regex _generalEscapePattern = new Regex("%(?<Value>[0-9A-F]{2})", RegexOptions.IgnoreCase);

		private readonly Func<IJsonSchema> _schemaFactory;
		private Uri _documentPath;
		private string _id;
		private string _schema;

		/// <summary>
		/// Defines the reference in respect to the root schema.
		/// </summary>
		public string Reference { get; private set; }
		/// <summary>
		/// Exposes the schema at the references location.
		/// </summary>
		/// <remarks>
		/// The <see cref="_Resolve"/> method must first be called.
		/// </remarks>
		public IJsonSchema Resolved { get; private set; }
		public IJsonSchema Base { get; set; }
		public string Id
		{
			get { return _id ?? (_id = Base?.Id); }
			set
			{
				_id = value;
				if (Base != null)
					Base.Id = value;
			}
		}
		public string Schema
		{
			get { return _schema ?? (_schema = Base?.Schema); }
			set
			{
				_schema = value;
				if (Base != null)
					Base.Schema = value;
			}
		}
		public Uri DocumentPath
		{
			get { return _documentPath ?? (_documentPath = Base?.DocumentPath); }
			set
			{
				_documentPath = value;
				if (Base != null)
					Base.DocumentPath = value;
			}
		}

		internal JsonSchemaReference(Type baseSchemaType)
		{
			_schemaFactory = _GetSchemaFactory(baseSchemaType ?? throw new ArgumentNullException(nameof(baseSchemaType))) ??
			                 throw new ArgumentException($"{nameof(baseSchemaType)} must be either {nameof(JsonSchema04)} or {nameof(JsonSchema06)}.");
		}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaReference"/> class that supports additional schema properties.
		/// </summary>
		/// <param name="reference">The relative (internal) or absolute (URI) path to the referenced type definition.</param>
		/// <param name="baseSchema">An instance of the base schema to use (either <see cref="JsonSchema04"/> or <see cref="JsonSchema06"/>).</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reference"/> or <paramref name="baseSchema"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when <paramref name="reference"/> is empty or whitespace or
		/// if <paramref name="baseSchema"/> is not of type <see cref="JsonSchema04"/> or <see cref="JsonSchema06"/>.</exception>
		public JsonSchemaReference(string reference, IJsonSchema baseSchema)
			: this(reference, baseSchema.GetType())
		{
			Base = baseSchema;
		}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaReference"/> class.
		/// </summary>
		/// <param name="reference">The relative (internal) or absolute (URI) path to the referenced type definition.</param>
		/// <param name="baseSchemaType">The draft version of schema to use as a base when resolving if not defined in the resolved schema.
		/// Must be either <see cref="JsonSchema04"/> or <see cref="JsonSchema06"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reference"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when <paramref name="reference"/> is empty or whitespace or
		/// when <paramref name="baseSchemaType"/> is not <see cref="JsonSchema04"/> or <see cref="JsonSchema06"/>.</exception>
		public JsonSchemaReference(string reference, Type baseSchemaType)
			: this(baseSchemaType)
		{
			Reference = reference ?? throw new ArgumentNullException(nameof(reference));
			
			if (string.IsNullOrWhiteSpace(reference)) throw new ArgumentException($"{nameof(reference)} non-empty and non-whitespace");
		}
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson(null);
			if (Resolved == null || root == null)
				jValue = _Resolve(jValue);
			var refResults = Resolved?.Validate(json, jValue) ??
			                 new SchemaValidationResults(null, "Error finding referenced schema.");
			return new SchemaValidationResults(new[] {refResults});
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Base?.FromJson(json, serializer);
			Reference = json.Object["$ref"].String;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var json = Base?.ToJson(serializer) ?? new JsonObject();
			json.Object["$ref"] = Reference;
			return json;
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchema other)
		{
			var schema = other as JsonSchemaReference;
			return schema != null && schema.Reference == Reference &&
			       Equals(Base, schema.Base);
		}
		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <returns>
		/// true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			return Equals(obj as IJsonSchema);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			return Reference?.GetHashCode() ?? 0;
		}

		private JsonValue _Resolve(JsonValue root)
		{
			var referenceParts = Reference.Split(new[] { '#' }, StringSplitOptions.None);
			var address = string.IsNullOrWhiteSpace(referenceParts[0]) ? DocumentPath?.OriginalString : referenceParts[0];
			var fragment = referenceParts.Length > 1 ? referenceParts[1] : string.Empty;
			var jValue = root;
			if (!string.IsNullOrWhiteSpace(address))
			{
				if (!Uri.TryCreate(address, UriKind.Absolute, out Uri absolute))
				{
					address = Id + address;
				}
				if (DocumentPath != null && !Uri.TryCreate(address, UriKind.Absolute, out absolute))
				{
					var uriFolder = DocumentPath.OriginalString.EndsWith("/") ? DocumentPath : DocumentPath.GetParentUri();
					absolute = new Uri(uriFolder, address);
					address = absolute.OriginalString;
				}
				jValue = JsonSchemaRegistry.Get(address).ToJson(null);
			}
			if (jValue == null) return root;
			if (jValue == "#") throw new ArgumentException("Cannot use a root reference as the base schema.");
 
			Resolved = _ResolveLocalReference(jValue, fragment, string.IsNullOrWhiteSpace(address) ? null : new Uri(address));
			return jValue;
		}
		private IJsonSchema _ResolveLocalReference(JsonValue root, string path, Uri documentPath)
		{
			var properties = path.Split('/').Skip(1).ToList();
			if (!properties.Any()) return JsonSchemaFactory.FromJson(root, _schemaFactory, documentPath);
			var value = root;
			foreach (var property in properties)
			{
				var unescaped = _Unescape(property);
				if (value.Type == JsonValueType.Object)
				{
					if (!value.Object.ContainsKey(unescaped)) return null;
					JsonValue id;
					// There's not really another way to do this well without the reference knowing what
					// version schema it should be using at each step in the path, so we test for both.
					// Since draft-06's '$id' is less likely to be used as a regular JSON property, we
					// check it first.
					if (value.Object.TryGetValue("$id", out id) || value.Object.TryGetValue("id", out id))
					{
						documentPath = Uri.TryCreate(id.String, UriKind.Absolute, out Uri uri)
							               ? uri
							               : new Uri(documentPath, id.String);
					}
					value = value.Object[unescaped];
				}
				else if (value.Type == JsonValueType.Array)
				{
					if (!int.TryParse(unescaped, out int index) || index >= value.Array.Count) return null;
					value = value.Array[index];
				}
			}
			return JsonSchemaFactory.FromJson(value, _schemaFactory, documentPath);
		}
		private static string _Unescape(string reference)
		{
			var unescaped = reference.Replace("~1", "/")
			                .Replace("~0", "~");
			var matches = _generalEscapePattern.Matches(unescaped);
			foreach (Match match in matches)
			{
				var value = int.Parse(match.Groups["Value"].Value, NumberStyles.HexNumber);
				var ch = (char) value;
				unescaped = Regex.Replace(unescaped, match.Value, new string(ch, 1));
			}
			return unescaped;
		}
		private static Func<IJsonSchema> _GetSchemaFactory(Type type)
		{
			if (type == typeof(JsonSchema04))
				return () => new JsonSchema04();
			if (type == typeof(JsonSchema06))
				return () => new JsonSchema06();
			return null;
		}
	}
}