using System;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class AllOfSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnAnyInvalid()
		{
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Verbose;

			var schema = new JsonSchema()
				.AllOf(new JsonSchema().Type(JsonSchemaType.Array),
				       new JsonSchema().Type(JsonSchemaType.Number));

			var json = new JsonObject();

			var expected = new SchemaValidationResults
				{
					IsValid = false,
					RelativeLocation = JsonPointer.Parse("#"),
					InstanceLocation = JsonPointer.Parse("#"),
					NestedResults =
						{
							new SchemaValidationResults
								{
									IsValid = false,
									Keyword = "allOf",
									RelativeLocation = JsonPointer.Parse("#/allOf"),
									InstanceLocation = JsonPointer.Parse("#"),
									NestedResults =
										{
											new SchemaValidationResults
												{
													IsValid = false,
													RelativeLocation = JsonPointer.Parse("#/allOf/0"),
													InstanceLocation = JsonPointer.Parse("#"),
													NestedResults =
														{
															new SchemaValidationResults
																{
																	IsValid = false,
																	Keyword = "type",
																	RelativeLocation = JsonPointer.Parse("#/allOf/0/type"),
																	InstanceLocation = JsonPointer.Parse("#"),
																	AdditionalInfo =
																		{
																			["expected"] = "array",
																			["actual"] = "object"
																		}
																}
														}
												},
											new SchemaValidationResults
												{
													IsValid = false,
													RelativeLocation = JsonPointer.Parse("#/allOf/1"),
													InstanceLocation = JsonPointer.Parse("#"),
													NestedResults =
														{
															new SchemaValidationResults
																{
																	IsValid = false,
																	Keyword = "type",
																	RelativeLocation = JsonPointer.Parse("#/allOf/1/type"),
																	InstanceLocation = JsonPointer.Parse("#"),
																	AdditionalInfo =
																		{
																			["expected"] = "number",
																			["actual"] = "object"
																		}
																}
														}
												}
										}
								}
						}
				};		

			var results = schema.Validate(json);

			results.AssertInvalid(expected);
		}

		[Test]
		public void ValidateReturnsValidOnAllValid()
		{
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Verbose;

			var schema = new JsonSchema()
				.AllOf(new JsonSchema()
					       .Type(JsonSchemaType.Number)
					       .Minimum(10),
				       new JsonSchema()
					       .Type(JsonSchemaType.Number)
					       .Maximum(20));

			var json = (JsonValue) 15;

			var expected = new SchemaValidationResults
				{
					IsValid = true,
					RelativeLocation = JsonPointer.Parse("#"),
					InstanceLocation = JsonPointer.Parse("#"),
					NestedResults =
						{
							new SchemaValidationResults
								{
									IsValid = true,
									RelativeLocation = JsonPointer.Parse("#/allOf"),
									InstanceLocation = JsonPointer.Parse("#"),
									Keyword = "allOf",
									NestedResults =
										{
											new SchemaValidationResults
												{
													IsValid = true,
													RelativeLocation = JsonPointer.Parse("#/allOf/0"),
													InstanceLocation = JsonPointer.Parse("#"),
													NestedResults =
														{
															new SchemaValidationResults
																{
																	IsValid = true,
																	RelativeLocation = JsonPointer.Parse("#/allOf/0/type"),
																	InstanceLocation = JsonPointer.Parse("#"),
																	Keyword = "type"
																},
															new SchemaValidationResults
																{
																	IsValid = true,
																	RelativeLocation = JsonPointer.Parse("#/allOf/0/minimum"),
																	InstanceLocation = JsonPointer.Parse("#"),
																	Keyword = "minimum"
																}
														}
												},
											new SchemaValidationResults
												{
													IsValid = true,
													RelativeLocation = JsonPointer.Parse("#/allOf/1"),
													InstanceLocation = JsonPointer.Parse("#"),
													NestedResults =
														{
															new SchemaValidationResults
																{
																	IsValid = true,
																	RelativeLocation = JsonPointer.Parse("#/allOf/1/type"),
																	InstanceLocation = JsonPointer.Parse("#"),
																	Keyword = "type"
																},
															new SchemaValidationResults
																{
																	IsValid = true,
																	RelativeLocation = JsonPointer.Parse("#/allOf/1/maximum"),
																	InstanceLocation = JsonPointer.Parse("#"),
																	Keyword = "maximum"
																}
														}
												}
										}
								}
						}
				};

			var results = schema.Validate(json);

			results.AssertValid(expected);
		}
	}
}