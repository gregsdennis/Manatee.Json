/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ReferencingSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		ReferencingSerializer
	Purpose:		Tracks references to objects so that they are not serialized
					more than once.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization.Internal
{
	internal class ReferencingSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences { get { return true; } }

		public ReferencingSerializer(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			SerializationPair pair;
			if (serializer.SerializationMap.Contains(obj))
			{
				var guid = serializer.SerializationMap.GetKey(obj);
				pair = serializer.SerializationMap[guid];
				pair.UsageCount++;
				return new JsonObject {{Constants.RefKey, guid.ToString()}};
			}
			if (_innerSerializer.ShouldMaintainReferences)
			{
				var guid = Guid.NewGuid();
				pair = new SerializationPair {Object = obj};
				serializer.SerializationMap.Add(guid, pair);
				pair.Json = _innerSerializer.Serialize(obj, serializer);
				if (pair.UsageCount != 0)
					pair.Json.Object.Add(Constants.DefKey, guid.ToString());
				return pair.Json;
			}
			return _innerSerializer.Serialize(obj, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Object)
			{
				var jsonObj = json.Object;
				SerializationPair pair;
				if (jsonObj.ContainsKey(Constants.DefKey))
				{
#if NET35 || NET35C
					var guid = new Guid(jsonObj[Constants.DefKey].String);
#elif NET4 || NET4C || NET45
					var guid = Guid.Parse(jsonObj[Constants.DefKey].String);
#endif
					jsonObj.Remove(Constants.DefKey);
					pair = new SerializationPair {Json = json};
					serializer.SerializationMap.Add(guid, pair);
					pair.Object = _innerSerializer.Deserialize<T>(json, serializer);
					pair.DeserializationIsComplete = true;
					pair.Reconcile();
					return (T) pair.Object;
				}
				if (jsonObj.ContainsKey(Constants.RefKey))
				{
#if NET35 || NET35C
					var guid = new Guid(jsonObj[Constants.RefKey].String);
#elif NET4 || NET4C || NET45
					var guid = Guid.Parse(jsonObj[Constants.RefKey].String);
#endif
					pair = serializer.SerializationMap[guid];
					return (T) pair.Object;
				}
			}
			return _innerSerializer.Deserialize<T>(json, serializer);
		}
	}
}