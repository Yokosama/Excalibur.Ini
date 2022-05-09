using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class SectionTest
    {
        [TestMethod]
        public void TestSection()
        {
            var section = new Section("newsection");

            Assert.IsNotNull(section);
            Assert.AreEqual(section.Name, "newsection");
            Assert.AreEqual(section.Properties.Length, 0);
            Assert.AreEqual(section.Comments.Count, 0);

            section.Add(new Property("1", "11"));
            section.Add("2", "22");
            section.Add("2", "222");

            Assert.AreEqual(section.Count, 3);
            var v1 = section.GetPropertyRawValue("2", "");
            var v2 = section.GetPropertyRawValue("2", "", true);
            Assert.AreEqual(v1, "22");
            Assert.AreEqual(v2, "222");

            var v3 = section.GetPropertyValue("2", 0);
            var v4 = section.GetPropertyValue("2", 0, true);
            Assert.AreEqual(v3, 22);
            Assert.AreEqual(v4, 222);

            var v5 = section.GetPropertyRawValue("1", "");
            Assert.AreEqual(v5, "11");

            section.SetPropertyValue("1", "55");
            v5 = section.GetPropertyRawValue("1", "");
            Assert.AreEqual(v5, "55");

            var clone = section.Clone();
            Assert.AreEqual(clone.Count, 3);

            clone.Comments.Add("comment test");
            Assert.AreEqual(clone.Comments.Count, 1);

            clone.Properties.Find("1").Comments.Add("comment test");

            clone.ClearComments();
            Assert.AreEqual(clone.Comments.Count, 0);
            Assert.AreEqual(clone.Properties.Find("1").Comments.Count, 0);

            clone.ClearProperties();
            Assert.AreEqual(clone.Properties.Length, 0);

            clone.Add(new Property("1", "11"));
            clone.Clear();
            Assert.AreEqual(clone.Count, 0);
        }
    }
}
