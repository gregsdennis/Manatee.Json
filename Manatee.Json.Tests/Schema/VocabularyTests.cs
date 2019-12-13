using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class VocabularyTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			JsonOptions.LogCategory = LogCategory.Schema;
		}

	}
}
