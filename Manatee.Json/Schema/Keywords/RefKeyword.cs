using System;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Reference}")]
	public class RefKeyword : IJsonSchemaKeyword, IEquatable<RefKeyword>
	{
		public static RefKeyword Root => new RefKeyword("#");

		private JsonSchema _resolvedRoot;
		private JsonPointer _resolvedFragment;

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
					BaseUri = _resolvedRoot.DocumentPath,
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
		public bool Equals(RefKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Reference, other.Reference);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as RefKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as RefKeyword);
		}
		public override int GetHashCode()
		{
			return Reference != null ? Reference.GetHashCode() : 0;
		}

		private void _ResolveReference(SchemaValidationContext context)
		{
			var documentPath = context.BaseUri;
			var referenceParts = Reference.Split(new[] { '#' }, StringSplitOptions.None);
			var address = string.IsNullOrWhiteSpace(referenceParts[0]) ? documentPath?.OriginalString : referenceParts[0];
			_resolvedFragment = referenceParts.Length > 1 ? JsonPointer.Parse(referenceParts[1]) : new JsonPointer();
			if (!string.IsNullOrWhiteSpace(address))
			{
				if (!Uri.TryCreate(address, UriKind.Absolute, out var absolute))
					address = context.Local.Id + address;

				if (documentPath != null && !Uri.TryCreate(address, UriKind.Absolute, out absolute))
				{
					var uriFolder = documentPath.OriginalString.EndsWith("/") ? documentPath : documentPath.GetParentUri();
					absolute = new Uri(uriFolder, address);
					address = absolute.OriginalString;
				}

				_resolvedRoot = JsonSchemaRegistry.Get(address);
			}
			else
				_resolvedRoot = context.Root;

			_ResolveLocalReference();
		}
		private void _ResolveLocalReference()
		{
			var serializer = new JsonSerializer();
			if (!_resolvedFragment.Any())
			{
				Resolved = _resolvedRoot;
				return;
			}

			var rootJson = _resolvedRoot.ToJson(serializer);
			var target = _resolvedFragment.Evaluate(rootJson);
			if (target.Error == null)
				Resolved = serializer.Deserialize<JsonSchema>(target.Result);
		}
	}
}