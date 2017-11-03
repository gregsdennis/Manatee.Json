﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Manatee.Study
{
    [TestFixture]
    [Ignore("This test suite is for development purposes only.")]
    public class NewtonsoftCaseStudy
    {
        private class MyClass
        {
            public string Value { get; set; }
        }

        [Test]
        public void SerializeArrayOfObjects()
        {
            var list = new List<object> {1, "string", false, new MyClass {Value = "hello"}};
            var serialized = JsonConvert.SerializeObject(list);

            Console.WriteLine(serialized);

            var backToList = JsonConvert.DeserializeObject<List<object>>(serialized);

            Assert.AreEqual(list, backToList);
        }
    }
}
