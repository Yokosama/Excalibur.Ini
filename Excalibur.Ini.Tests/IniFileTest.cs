using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class IniFileTest
    {
        [TestMethod]
        public void TestIniFile()
        {
            var iniFile = new IniFile();
            iniFile.Parser.Scheme.CommentStrings.Add("//");
            
            // test load
            var data = iniFile.Load("IniFileTest.ini", Encoding.Default);
            Assert.AreEqual(data.SectionCount, 4);
            Assert.AreEqual(data.Global.GetPropertyRawValue("globalKey2", ""), " globalValue2");

            // test save
            var saveFile = "SaveIniFileTest.ini";
            if (File.Exists(saveFile)) File.Delete(saveFile);

            Assert.IsTrue(iniFile.Save(data, saveFile, Encoding.Default));

            Assert.IsFalse(iniFile.Save(data, saveFile, Encoding.Default));

            Assert.IsTrue(iniFile.Save(data, saveFile, Encoding.Default, true));
        }
    }
}
