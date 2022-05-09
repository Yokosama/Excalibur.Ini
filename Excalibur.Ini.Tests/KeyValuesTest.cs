using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class KeyValuesTest
    {
        [TestMethod]
        public void TestKeyValues()
        {
            KeyValues<TestItem> kvs = new KeyValues<TestItem>
            {
                { "1", new TestItem { Key = "1", Value = "111" } },
                { "2", new TestItem { Key = "2", Value = "222" } },
                { "3", new TestItem { Key = "3", Value = "333" } },
                { "4", new TestItem { Key = "4", Value = "444" } },
                { "4", new TestItem { Key = "4", Value = "4444" } }
            };

            Assert.AreEqual(kvs.Length, 5);
            List<string> keys = new List<string> { "1", "2", "3", "4" };
            foreach (var kvItem in kvs)
            {
                if (!keys.Contains(kvItem.Key))
                {
                    Assert.Fail();
                }
            }

            kvs.Add("5", new TestItem { Key = "5", Value = "555" });
            Assert.AreEqual(kvs.Length, 6);

            var item = kvs.Find("4");
            Assert.AreEqual(item.Value, "444");

            item = kvs.FindLast("4");
            Assert.AreEqual(item.Value, "4444");

            var items = kvs.FindAll("4");
            Assert.AreEqual(items.Count, 2);

            Assert.IsTrue(kvs.ContainsKey("4"));
            Assert.IsFalse(kvs.ContainsKey("6"));

            var copyKvs = kvs.Clone();
            item = copyKvs.Find("5");
            copyKvs.Remove("5", item);
            Assert.IsFalse(copyKvs.ContainsKey("5"));

            copyKvs.RemoveFirst("4");
            Assert.AreEqual(copyKvs.Count("4"), 1);

            copyKvs.Add("4", new TestItem { Key = "4", Value = "44444" });
            copyKvs.Add("4", new TestItem { Key = "4", Value = "444444" });
            Assert.AreEqual(copyKvs.Count("4"), 3);

            copyKvs.RemoveLast("4");
            Assert.AreEqual(copyKvs.FindLast("4").Value, "44444");

            copyKvs.RemoveAll("4");
            Assert.IsFalse(copyKvs.ContainsKey("4"));

            copyKvs.Clear();
            Assert.AreEqual(copyKvs.Length, 0);
        }

        public class TestItem : ICloneable<TestItem>
        {
            public string Key { get; set; } 
            public string Value { get; set; } 

            public TestItem Clone()
            {
                return new TestItem { Key = Key, Value = Value };
            }
        }
    }
}
