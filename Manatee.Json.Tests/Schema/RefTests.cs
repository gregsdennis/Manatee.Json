using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	public class RefTests
	{
		[Test]
		public void RefResolvesToInternallyStoredSchema()
		{
			var source = new JsonSchema()
				.Id("http://schema.org/source")
				.AllOf(new JsonSchema().Ref("http://schema.org/target"));

			var target = new JsonSchema()
				.Id("http://schema.org/target")
				.Property("test", true)
				.Required("test");

			JsonSchemaRegistry.Register(source);
			JsonSchemaRegistry.Register(target);

			try
			{
				var instance = new JsonObject {["test"] = "literally anything"};

				var results = source.Validate(instance);

				results.AssertValid();
			}
			finally
			{
				JsonSchemaRegistry.Unregister(source);
				JsonSchemaRegistry.Unregister(target);
			}
		}

		[Test]
		public void RecursiveRefResolvesOuterAnchor_Simple()
		{
			var source = new JsonSchema()
				.Id("http://schema.org/source")
				.RecursiveAnchor(true)
				.OneOf(new JsonSchema()
					       .Property("nested", new JsonSchema()
						                 .Ref("http://schema.org/target"))
					       .Required("nested"),
				       new JsonSchema()
					       .Property("test", true)
					       .Required("test"));

			var target = new JsonSchema()
				.Id("http://schema.org/target")
				.RecursiveAnchor(true)
				.AllOf(new JsonSchema().RecursiveRefRoot());

			JsonSchemaRegistry.Register(source);
			JsonSchemaRegistry.Register(target);

			try
			{
				var instance = new JsonObject
					{
						["nested"] = new JsonObject
							{
								["test"] = "literally anything"
							}
					};

				var results = source.Validate(instance);

				results.AssertValid();
			}
			finally
			{
				JsonSchemaRegistry.Unregister(source);
				JsonSchemaRegistry.Unregister(target);
			}
		}

		[Test]
		public void RecursiveRefResolvesOuterAnchor_BrokenChain()
		{
			var source = new JsonSchema()
				.Id("http://schema.org/source")
				.RecursiveAnchor(true)
				.Title("source")
				.OneOf(new JsonSchema()
					       .Property("nested", new JsonSchema()
						                 .Ref("http://schema.org/middle"))
					       .Required("nested"),
				       new JsonSchema()
					       .Property("test", true)
					       .Required("test"));

			var middle = new JsonSchema()
				.Id("http://schema.org/middle")
				.Title("middle")
				.OneOf(new JsonSchema()
					       .Ref("http://schema.org/target"),
				       new JsonSchema()
					       .Property("test", false)
					       .Required("test"));

			var target = new JsonSchema()
				.Id("http://schema.org/target")
				.RecursiveAnchor(true)
				.Title("target")
				.AllOf(new JsonSchema().RecursiveRefRoot());

			JsonSchemaRegistry.Register(source);
			JsonSchemaRegistry.Register(middle);
			JsonSchemaRegistry.Register(target);

			try
			{
				var instance = new JsonObject
					{
						["nested"] = new JsonObject
							{
								["test"] = "literally anything"
							}
					};

				var results = source.Validate(instance);

				results.AssertValid();
			}
			finally
			{
				JsonSchemaRegistry.Unregister(source);
				JsonSchemaRegistry.Unregister(middle);
				JsonSchemaRegistry.Unregister(target);
			}
		}

		[Test]
		public void RecursiveRefResolvesSelf()
		{
			var source = new JsonSchema()
				.Id("http://schema.org/source")
				.RecursiveAnchor(true)
				.OneOf(new JsonSchema()
					       .Property("nested", new JsonSchema()
						                 .Ref("http://schema.org/target"))
					       .Required("nested"),
				       new JsonSchema()
					       .Property("source", true)
					       .Required("source"));

			var target = new JsonSchema()
				.Id("http://schema.org/target")
				.OneOf(new JsonSchema()
					       .Property("target", new JsonSchema().RecursiveRefRoot())
					       .Required("target"),
				       new JsonSchema()
					       .Property("test", true)
					       .Required("test"));

			JsonSchemaRegistry.Register(source);
			JsonSchemaRegistry.Register(target);

			try
			{
				var instance = new JsonObject
					{
						["nested"] = new JsonObject
							{
								["target"] = new JsonObject
									{
										["test"] = "literally anything"
									}
							}
					};

				var results = source.Validate(instance);

				results.AssertValid();
			}
			finally
			{
				JsonSchemaRegistry.Unregister(source);
				JsonSchemaRegistry.Unregister(target);
			}
		}
	}
}
