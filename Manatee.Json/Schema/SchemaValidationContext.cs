using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Used to track data throughout the validation process.
	/// </summary>
	public class SchemaValidationContext
	{
		private HashSet<string>? _evaluatedPropertyNames;
		private HashSet<string>? _locallyEvaluatedPropertyNames;
		/// <summary>
		/// Gets or sets the local schema at this point in the validation.
		/// </summary>
		public JsonSchema Local { get; set; }
		/// <summary>
		/// Gets or sets the root schema when validation begins.
		/// </summary>
		public JsonSchema Root { get; set; }
		/// <summary>
		/// Gets or sets the recursive anchor (root).
		/// </summary>
		public JsonSchema? RecursiveAnchor { get; set; }
		/// <summary>
		/// Gets or sets the instance being validated.
		/// </summary>
		public JsonValue Instance { get; set; }
		/// <summary>
		/// Gets a list of property names that have been evaluated in this validation pass.
		/// </summary>
		public HashSet<string> EvaluatedPropertyNames => _evaluatedPropertyNames ??= new HashSet<string>();
		/// <summary>
		/// Gets a list of property names that have been evaluated on the current tier of this validation pass.
		/// </summary>
		public HashSet<string> LocallyEvaluatedPropertyNames => _locallyEvaluatedPropertyNames ??= new HashSet<string>();
		/// <summary>
		/// Gets the last array index that has been evaluated in this validation pass.
		/// </summary>
		public int LastEvaluatedIndex { get; set; } = -1;
		/// <summary>
		/// Gets the last array index that has been evaluated on the current tier of this validation pass.
		/// </summary>
		public int LocalTierLastEvaluatedIndex { get; set; } = -1;
		/// <summary>
		/// Gets or sets the base URI at this point in the validation.
		/// </summary>
		public Uri? BaseUri { get; set; }
		/// <summary>
		/// Gets or sets the current instance location.
		/// </summary>
		public JsonPointer InstanceLocation { get; set; }
		/// <summary>
		/// Gets or sets the current schema keyword location relative to the original schema root.
		/// </summary>
		public JsonPointer RelativeLocation { get; set; }
		/// <summary>
		/// Gets or sets the current schema location relative to the current base URI (<see cref="BaseUri"/>).
		/// </summary>
		// TODO: Revisit the nullability of this
		public JsonPointer? BaseRelativeLocation { get; set; }
		/// <summary>
		/// Gets or sets whether the current validation run is for a meta-schema.
		/// </summary>
		public bool IsMetaSchemaValidation { get; set; }
		/// <summary>
		/// Miscellaneous data.  Useful for communicating results between keywords.
		/// </summary>
		/// <remarks>
		/// Use <see cref="IJsonSchemaKeyword.ValidationSequence"/> to ensure that keywords are
		/// processed in the correct order so that the communication occurs properly.
		/// </remarks>
		public Dictionary<string, object> Misc { get; } = new Dictionary<string, object>();

		internal JsonSchemaRegistry LocalRegistry { get; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		internal SchemaValidationContext(JsonSchema root,
										 JsonValue instance,
		                                 JsonPointer? baseRelativeLocation,
		                                 JsonPointer relativeLocation,
		                                 JsonPointer instanceLocation)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			Root = root;
			Instance = instance;
			BaseRelativeLocation = baseRelativeLocation;
			RelativeLocation = relativeLocation;
			InstanceLocation = instanceLocation;
			LocalRegistry = new JsonSchemaRegistry();
		}
		/// <summary>
		/// Creates a new instance of the <see cref="SchemaValidationContext"/> class by copying values from another instance.
		/// </summary>
		public SchemaValidationContext(SchemaValidationContext source)
			: this(source.Root, source.Instance, source.BaseRelativeLocation, source.RelativeLocation, source.InstanceLocation)
		{
			Local = source.Local;
			Root = source.Root;
			RecursiveAnchor = source.RecursiveAnchor;
			Instance = source.Instance;
			EvaluatedPropertyNames.UnionWith(source.EvaluatedPropertyNames);
			LocallyEvaluatedPropertyNames.UnionWith(source.LocallyEvaluatedPropertyNames);
			LastEvaluatedIndex = source.LastEvaluatedIndex;
			LocalTierLastEvaluatedIndex = source.LocalTierLastEvaluatedIndex;
			BaseUri = source.BaseUri;
			InstanceLocation = source.InstanceLocation;
			RelativeLocation = source.RelativeLocation;
			BaseRelativeLocation = source.BaseRelativeLocation;
			IsMetaSchemaValidation = source.IsMetaSchemaValidation;

			LocalRegistry = source.LocalRegistry;
		}

		/// <summary>
		/// Updates the <see cref="EvaluatedPropertyNames"/>, <see cref="LocallyEvaluatedPropertyNames"/>,
		/// <see cref="LastEvaluatedIndex"/>, and <see cref="LocalTierLastEvaluatedIndex"/> properties based
		/// on another context that processed a subschema.
		/// </summary>
		/// <param name="other">Another context object.</param>
		public void UpdateEvaluatedPropertiesAndItemsFromSubschemaValidation(SchemaValidationContext other)
		{
			EvaluatedPropertyNames.UnionWith(other.EvaluatedPropertyNames);
			EvaluatedPropertyNames.UnionWith(other.LocallyEvaluatedPropertyNames);
			if (other.EvaluatedPropertyNames.Any())
				Log.Schema($"Properties [{EvaluatedPropertyNames.ToStringList()}] have now been validated");
			LastEvaluatedIndex = Math.Max(LastEvaluatedIndex, other.LastEvaluatedIndex);
			LastEvaluatedIndex = Math.Max(LastEvaluatedIndex, other.LocalTierLastEvaluatedIndex);
			if (other.EvaluatedPropertyNames.Any())
				Log.Schema($"Indices through [{other.LastEvaluatedIndex}] have now been validated");
		}
	}
}