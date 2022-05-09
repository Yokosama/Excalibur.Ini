using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class PropertyTest
    {
        [TestMethod]
        public void TestProperty()
        {
            var property = new Property("key");

            Assert.IsNotNull(property);
            Assert.AreEqual(property.Key, "key");
            Assert.IsTrue(string.IsNullOrEmpty(property.Value));
            Assert.AreEqual(property.Comments.Count, 0);

            property.Value = "333";

            var clone = property.Clone();
            Assert.AreEqual(clone.Key, "key");
            Assert.AreEqual(clone.Value, "333");
        }
    }
}
