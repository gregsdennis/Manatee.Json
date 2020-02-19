using System;
using Manatee.Json.Tests.Common;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[SetUpFixture]
	public class OptionsConfigurator
	{

		[OneTimeSetUp]
		public void Setup()
		{
			if (string.Equals(Environment.GetEnvironmentVariable("LOCAL_TEST"), "true", StringComparison.InvariantCultureIgnoreCase))
				JsonOptions.Log = new ConsoleLog();
		}
	}
}