using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Serialization
{
	[TestFixture]
	public class SchemaAttributeTests
	{
		[Schema(@"SchemaAttribute.json")]
		public class DefinedByFile
		{
			public string MyString { get; set; }
			public int MyInt { get; set; }

			public override bool Equals(object obj)
			{
				return Equals(obj as DefinedByFile);
			}
			protected bool Equals(DefinedByFile other)
			{
				if (ReferenceEquals(this, other)) return true;
				return string.Equals(MyString, other.MyString) && MyInt == other.MyInt;
			}
			public override int GetHashCode()
			{
				unchecked
				{
					return ((MyString != null ? MyString.GetHashCode() : 0) * 397) ^ MyInt;
				}
			}
		}

		[Schema(nameof(Definition))]
		public class DefinedByProperty
		{
			public static IJsonSchema Definition =>
				new JsonSchema06
					{
						Schema = JsonSchema06.MetaSchema.Schema,
						Properties = new Dictionary<string, IJsonSchema>
							{
								["MyString"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
								["MyInt"] = new JsonSchema06
									{
										Type = JsonSchemaTypeDefinition.Integer,
										Minimum = 10
									}
							}
					};

			public string MyString { get; set; }
			public int MyInt { get; set; }

			public override bool Equals(object obj)
			{
				return Equals(obj as DefinedByProperty);
			}
			protected bool Equals(DefinedByProperty other)
			{
				if (ReferenceEquals(this, other)) return true;
				return string.Equals(MyString, other.MyString) && MyInt == other.MyInt;
			}
			public override int GetHashCode()
			{
				unchecked
				{
					return ((MyString != null ? MyString.GetHashCode() : 0) * 397) ^ MyInt;
				}
			}
		}

		[Schema(nameof(Definition))]
		public class DefinedByConcreteProperty
		{
			public static JsonSchema06 Definition =>
				new JsonSchema06
					{
						Schema = JsonSchema06.MetaSchema.Schema,
						Properties = new Dictionary<string, IJsonSchema>
							{
								["MyString"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
								["MyInt"] = new JsonSchema06
									{
										Type = JsonSchemaTypeDefinition.Integer,
										Minimum = 10
									}
							}
					};

			public string MyString { get; set; }
			public int MyInt { get; set; }

			public override bool Equals(object obj)
			{
				return Equals(obj as DefinedByConcreteProperty);
			}
			protected bool Equals(DefinedByConcreteProperty other)
			{
				if (ReferenceEquals(this, other)) return true;
				return string.Equals(MyString, other.MyString) && MyInt == other.MyInt;
			}
			public override int GetHashCode()
			{
				unchecked
				{
					return ((MyString != null ? MyString.GetHashCode() : 0) * 397) ^ MyInt;
				}
			}
		}

		[Schema(@"not-actually-a-file.json")]
		public class SchemaFileNotFound
		{
			public string MyString { get; set; }
			public int MyInt { get; set; }
		}

		[Schema(nameof(NotASchema))]
		public class SchemaPropertyNotFound
		{
			public static int NotASchema { get; }

			public string MyString { get; set; }
			public int MyInt { get; set; }
		}

		[OneTimeSetUp]
		public static void Setup()
		{
			const string baseUri = "http://localhost:1234/";

			JsonSchemaOptions.Download = uri =>
				{
					var localPath = uri.Replace(baseUri, string.Empty);

					if (!Uri.TryCreate(localPath, UriKind.Absolute, out Uri newPath))
					{
						var remotesPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\").AdjustForOS();
						newPath = new Uri(new Uri(remotesPath), localPath);
					}

					return File.ReadAllText(newPath.LocalPath);
				};
		}

		[Test]
		public void SchemaInFilePasses()
		{
			var json = new JsonObject
				{
					["MyString"] = "some string",
					["MyInt"] = 15
				};
			var expected = new DefinedByFile
				{
					MyString = "some string",
					MyInt = 15
				};

			Directory.SetCurrentDirectory(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Files"));
			var actual = new JsonSerializer().Deserialize<DefinedByFile>(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SchemaIsFileThrows()
		{
			Assert.Throws<JsonSerializationException>(() =>
				{
					var json = new JsonObject
						{
							["MyString"] = "some string",
							["MyInt"] = 9
						};

					new JsonSerializer().Deserialize<DefinedByFile>(json);
				});
		}

		[Test]
		public void SchemaInPropertyPasses()
		{
			var json = new JsonObject
				{
					["MyString"] = "some string",
					["MyInt"] = 15
				};
			var expected = new DefinedByProperty
			{
					MyString = "some string",
					MyInt = 15
				};

			var actual = new JsonSerializer().Deserialize<DefinedByProperty>(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SchemaInConcretePropertyPasses()
		{
			var json = new JsonObject
				{
					["MyString"] = "some string",
					["MyInt"] = 15
				};
			var expected = new DefinedByConcreteProperty
			{
					MyString = "some string",
					MyInt = 15
				};

			var actual = new JsonSerializer().Deserialize<DefinedByConcreteProperty>(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SchemaIsPropertyThrows()
		{
			Assert.Throws<JsonSerializationException>(() =>
				{
					var json = new JsonObject
						{
							["MyString"] = "some string",
							["MyInt"] = 9
						};

					new JsonSerializer().Deserialize<DefinedByProperty>(json);
				});
		}

		[Test]
		public void SchemaFileNotFoundThrows()
		{
			Assert.Throws<JsonSerializationException>(() =>
				{
					var json = new JsonObject
						{
							["MyString"] = "some string",
							["MyInt"] = 9
						};

					new JsonSerializer().Deserialize<SchemaFileNotFound>(json);
				});
		}

		[Test]
		public void SchemaPropertyWrongType()
		{
			Assert.Throws<JsonSerializationException>(() =>
				{
					var json = new JsonObject
						{
							["MyString"] = "some string",
							["MyInt"] = 9
						};

					new JsonSerializer().Deserialize<SchemaPropertyNotFound>(json);
				});
		}
	}
}
