using System;
using System.Linq;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class RefKeyword : IJsonSchemaKeyword
	{
		public static readonly RefKeyword Root = new RefKeyword("#");

		private JsonSchema _resolvedRoot;

		public string Name => "$ref";
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 0;

		public string Reference { get; private set; }

		public JsonSchema Resolved { get; private set; }

		public RefKeyword() { }
		public RefKeyword(string reference)
		{
			Reference = reference;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (Resolved == null)
			{
				_ResolveReference(context);
				if (Resolved == null)
					return new SchemaValidationResults(null, $"Could not find referenced schema: {Reference}");
			}

			var newContext = new SchemaValidationContext
				{
					Instance = context.Instance,
					Root = _resolvedRoot ?? context.Root
				};
			return Resolved.Validate(newContext);
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Reference = json.String;
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Reference;
		}

		private void _ResolveReference(SchemaValidationContext context)
		{
			var root = context.Root;
			var documentPath = context.Local.DocumentPath;
			var referenceParts = Reference.Split(new[] { '#' }, StringSplitOptions.None);
			var address = string.IsNullOrWhiteSpace(referenceParts[0]) ? documentPath?.OriginalString : referenceParts[0];
			var fragment = referenceParts.Length > 1 ? referenceParts[1] : string.Empty;
			var schema = root;
			if (!string.IsNullOrWhiteSpace(address))
			{
				if (!Uri.TryCreate(address, UriKind.Absolute, out Uri absolute))
				{
					address = context.Local.Id + address;
				}
				if (documentPath != null && !Uri.TryCreate(address, UriKind.Absolute, out absolute))
				{
					var uriFolder = documentPath.OriginalString.EndsWith("/") ? documentPath : documentPath.GetParentUri();
					absolute = new Uri(uriFolder, address);
					address = absolute.OriginalString;
				}
				schema = JsonSchemaRegistry.Get(address);
				_resolvedRoot = schema;
			}

			//if (schema == null)
			//{
			//	Resolved = schema;
			//	return;
			//}
			//if (schema == "#") throw new ArgumentException("Cannot use a root reference as the base schema.");

			_ResolveLocalReference(schema, fragment, string.IsNullOrWhiteSpace(address) ? null : new Uri(address));
		}
		private void _ResolveLocalReference(JsonSchema root, string path, Uri documentPath)
		{
			var serializer = new JsonSerializer();
			var pointer = JsonPointer.Parse(path);
			if (!pointer.Any())
			{
				Resolved = root;
				return;
			}

			var rootJson = root.ToJson(serializer);
			var target = pointer.Evaluate(rootJson);
			if (target.Error == null)
				Resolved = serializer.Deserialize<JsonSchema>(target.Result);
		}
	}
}