using System;
using System.Collections.Generic;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Benchmark
{
	public class EmojiResponse
	{
		public List<LocalEmoji> Trello { get; set; }
	}

	public class LocalEmoji
	{
		// functions as ID
		public string Unified { get; set; }
		public string Native { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public List<string> ShortNames { get; set; }
		public string Text { get; set; }
		public List<string> Texts { get; set; }
		public string Category { get; set; }
		public int SheetX { get; set; }
		public int SheetY { get; set; }
		public string Tts { get; set; }
		public List<string> Keywords { get; set; }
		public Dictionary<string, LocalEmoji> SkinVariations { get; set; }
	}

}