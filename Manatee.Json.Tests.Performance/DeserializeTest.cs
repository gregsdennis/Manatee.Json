using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JsonSerializer = Manatee.Json.Serialization.JsonSerializer;

namespace Manatee.Json.Tests.Performance
{
	[TestClass]
	[DeploymentItem("Associates.json")]
	public class DeserializeTest
	{
		[TestMethod]
		public void Performance_FullAutoSerialize_Single()
		{
			Console.WriteLine("Time To Beat: 00:00:00.0280016");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var start = DateTime.Now;
			var json = JsonValue.Parse(content);
			var obj = serializer.Deserialize<IEnumerable<Associate>>(json);
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
			start = DateTime.Now;
			obj = JsonConvert.DeserializeObject<IEnumerable<Associate>>(content);
			end = DateTime.Now;
			Console.WriteLine("NewtonSoft: {0}", end - start);
		}
		[TestMethod]
		public void Performance_FullAutoSerialize_10000()
		{
			Console.WriteLine("Time To Beat: 00:00:01.6560947");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			IEnumerable<Associate> obj;
			var start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				var json = JsonValue.Parse(content);
				obj = serializer.Deserialize<IEnumerable<Associate>>(json);
			}
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				obj = JsonConvert.DeserializeObject<IEnumerable<Associate>>(content);
			}
			end = DateTime.Now;
			Console.WriteLine("NewtonSoft: {0}", end - start);
		}
		[TestMethod]
		public void Performance_FullIJsonSerialize_Single()
		{
			Console.WriteLine("Time To Beat: 00:00:00.0230013");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var start = DateTime.Now;
			var json = JsonValue.Parse(content);
			var obj = serializer.Deserialize<IEnumerable<SerializableAssociate>>(json);
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
			foreach (var serializableAssociate in obj)
			{
				Console.WriteLine("\t{0}", serializableAssociate);
			}
			start = DateTime.Now;
			var obj2 = JsonConvert.DeserializeObject<IEnumerable<SerializableAssociate>>(content);
			end = DateTime.Now;
			Console.WriteLine("NewtonSoft: {0}", end - start);
			foreach (var serializableAssociate in obj2)
			{
				Console.WriteLine("\t{0}", serializableAssociate);
			}
		}
		[TestMethod]
		public void Performance_FullIJsonSerialize_10000()
		{
			Console.WriteLine("Time To Beat: 00:00:00.6080608");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			IEnumerable<SerializableAssociate> obj;
			var start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				var json = JsonValue.Parse(content);
				obj = serializer.Deserialize<IEnumerable<SerializableAssociate>>(json);
			}
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				obj = JsonConvert.DeserializeObject<IEnumerable<SerializableAssociate>>(content);
			}
			end = DateTime.Now;
			Console.WriteLine("NewtonSoft: {0}", end - start);
		}
		[TestMethod]
		public void Performance_IJsonSerializeOnly_Single()
		{
			Console.WriteLine("Time To Beat: 00:00:00.0120012");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			IEnumerable<SerializableAssociate> obj;
			var json = JsonValue.Parse(content);
			var start = DateTime.Now;
			obj = serializer.Deserialize<IEnumerable<SerializableAssociate>>(json);
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
		}
		[TestMethod]
		public void Performance_IJsonSerializeOnly_10000()
		{
			Console.WriteLine("Time To Beat: 00:00:00.2130213");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			IEnumerable<SerializableAssociate> obj;
			var json = JsonValue.Parse(content);
			var start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				obj = serializer.Deserialize<IEnumerable<SerializableAssociate>>(json);
			}
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
		}
	}
}
