using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JsonSerializer = Manatee.Json.Serialization.JsonSerializer;

namespace Manatee.Json.Performance
{
	[TestClass]
	[DeploymentItem("Associates.json")]
	public class DeserializeTest
	{
		[TestMethod]
		public void Performance_FullAutoSerialize_Single()
		{
			Console.WriteLine("Time To Beat: 00:00:00.0790045");
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
			Console.WriteLine("Time To Beat: 00:00:02.7321563");
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
			Console.WriteLine("Time To Beat: 00:00:00.0719928");
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
		public void Performance_FullIJsonSerialize_Single2()
		{
			Console.WriteLine("Time To Beat: 00:00:00.0719928");
			var content = File.ReadAllText("Associates.json");
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var start = DateTime.Now;
			var json = JsonValue.Parse(content);
			var obj = serializer.Deserialize<IEnumerable<SerializableAssociate2>>(json);
			var end = DateTime.Now;
			Console.WriteLine("Manatee: {0}", end - start);
			foreach (var serializableAssociate in obj)
			{
				Console.WriteLine("\t{0}", serializableAssociate);
			}
			start = DateTime.Now;
			var obj2 = JsonConvert.DeserializeObject<IEnumerable<SerializableAssociate2>>(content);
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
			Console.WriteLine("Time To Beat: 00:00:02.7321563");
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
	}
}
