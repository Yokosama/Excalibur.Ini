using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class IniDataParserTest
    {
       private readonly string _data = @"; line comment test1
;line comment test2
[qwer ]
key1 = val[ue1
key2 = value2

// test section comment
// test section comment
[1]
// test property comment
// test property comment
key1 = value1 ; test comment after value
key2 = value2 // test comment ; after value
key3 = value3 \; \// Test comment escape; real comment after value
key4 = value4
[2]
key1 = value1
key2 = value2

; test duplicate section
[1]
key1 = dupsection
key2 = value2
; test dunplicate property
key1 = value1

;test end of file commments1
;test end of file commments2
;test end of file commments3";

        [TestMethod]
        public void TestParse()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");

            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.SectionCount, 4);

            // line comment test
            Assert.AreEqual(iniData.GetSection("qwer").Comments.Count, 2);
            Assert.AreEqual(iniData.GetSection("qwer").Comments[0], " line comment test1");
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyRawValue("key1", ""), " val[ue1");

            // test section comment
            Assert.AreEqual(iniData.GetSection("1").Properties.Length, 4);
            Assert.AreEqual(iniData.GetSection("1").Comments[0], " test section comment");

            // test comment after property value
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key1").Comments.Count, 2);
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key1").CommentAfterValue, " test comment after value");
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key2").CommentAfterValue, " test comment ; after value");
            // test comment escape
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key3").Value, @" value3 \; \// Test comment escape");
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key3").CommentAfterValue, " real comment after value");

            //  test duplicate section
            Assert.AreEqual(iniData.GetSection("1", true).Properties.Length, 3);
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", ""), " dupsection");
            // test dunplicate property
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", "", true), " value1");

            // test end of file comments
            Assert.AreEqual(iniData.EndOfFileComments.Count, 3);
            Assert.AreEqual(iniData.EndOfFileComments[2], "test end of file commments3");
            
        }
    }
}
