using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class IniDataTest
    {
        [TestMethod]
        public void TestSection()
        {
            var data = new IniData();

            var section1 = data.Add("section_test");
            var section2 = new Section("section_test2");
            data.Add(section2);
            var section3 = data.Add("section_test");
            Assert.AreEqual(data.SectionCount, 3);

            section1.Add("1", "11");
            section1.Add("2", "22");
            section3.Add("1", "111");
            Assert.AreEqual(data.GetPropertyRawValue("section_test", "1", ""), "11");
            Assert.AreEqual(data.GetPropertyRawValue("section_test", "1", "", true), "111");

            Assert.AreEqual(data.GetPropertyValue("section_test", "2", 0), 22);
            Assert.AreEqual(data.GetPropertyValue("section_test", "1", 0, true), 111);

            data.SetPropertyValue("section_test", "1", "1111");
            data.SetPropertyValue("section_test", "1", "111111", lastSection: true);

            Assert.AreEqual(data.GetPropertyValue("section_test", "1", 0), 1111);
            Assert.AreEqual(data.GetPropertyValue("section_test", "1", 0, true), 111111);

            var clone = data.Clone();
            clone.SetPropertyValue("section_test", "1", "233", lastSection: true);
            Assert.AreEqual(data.GetPropertyValue("section_test", "1", 0, true), 111111);
            Assert.AreEqual(clone.GetPropertyValue("section_test", "1", 0, true), 233);
        }
    }
}
