using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ValidationResultsTests
	{
		[Test]
		public void Condenses()
		{
			var initial = new SchemaValidationResults
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

			var expected = new SchemaValidationResults
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
									Keyword = "type",
									RelativeLocation = JsonPointer.Parse("#/allOf/0/type"),
									InstanceLocation = JsonPointer.Parse("#"),
									AdditionalInfo =
										{
											["expected"] = "array",
											["actual"] = "object"
										}
								},
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
				};

			var actual = initial.Condense();

			actual.AssertInvalid(expected);
		}
		[Test]
		public void Flattens()
		{
			var initial = new SchemaValidationResults
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

			var expected = new SchemaValidationResults
				{
					IsValid = false,
					NestedResults =
						{
							new SchemaValidationResults
								{
									IsValid = false,
									Keyword = "allOf",
									RelativeLocation = JsonPointer.Parse("#/allOf"),
									InstanceLocation = JsonPointer.Parse("#"),
								},
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
								},
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
				};

			var actual = initial.Flatten();

			actual.AssertInvalid(expected);
		}
	}
}
