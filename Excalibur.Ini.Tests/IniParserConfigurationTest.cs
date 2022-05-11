using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class IniParserConfigurationTest
    {
       private readonly string _data = @"; test global properties
globalKey1 = globalValue1
globalKey2 = globalValue2
; line comment test1  

;line comment test2
[qwer ] // test comment after section name 
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
        public void TestCaseInsensitive()
        {
            var parser = new IniDataParser();
            parser.Configuration.CaseInsensitive = true;
            parser.Scheme.CommentStrings.Add("//");

            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer"), iniData.GetSection("qWeR"));
        }

        [TestMethod]
        public void TestAllowKeysWithoutSection()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.AllowKeysWithoutSection = true;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.Global.Count, 2);

            parser.Configuration.AllowKeysWithoutSection = false;
            Assert.ThrowsException<IniParsingException>(() => { iniData = parser.Parse(_data); });
        }

        [TestMethod]
        public void TestDuplicateSectionsBehaviour()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.DuplicateSectionsBehaviour = IniParserConfiguration.DuplicateBehaviour.AllowAndRepeat;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSections("1").Count, 2);

            parser.Configuration.DuplicateSectionsBehaviour = IniParserConfiguration.DuplicateBehaviour.AllowAndKeepFirstValue;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.Sections.FindAll("1").Count, 1);
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", ""), " value1 ");

            parser.Configuration.DuplicateSectionsBehaviour = IniParserConfiguration.DuplicateBehaviour.AllowAndKeepLastValue;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.Sections.FindAll("1").Count, 1);
            Assert.AreEqual(iniData.GetSection("1").GetPropertyRawValue("key1", ""), " dupsection");

            parser.Configuration.SkipInvalidLines = false;
            parser.Configuration.DuplicateSectionsBehaviour = IniParserConfiguration.DuplicateBehaviour.DisallowAndStopWithError;
            Assert.ThrowsException<IniParsingException>(() => { iniData = parser.Parse(_data); });
        }

        [TestMethod]
        public void TestDuplicatePropertiesBehaviour()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.DuplicateBehaviour.AllowAndRepeat;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("1", true).Properties.Length, 3);
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", ""), " dupsection");
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", "", true), " value1");

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.DuplicateBehaviour.AllowAndKeepFirstValue;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("1", true).Properties.Length, 2);
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", "", true), " dupsection");

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.DuplicateBehaviour.AllowAndKeepLastValue;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("1", true).Properties.Length, 2);
            Assert.AreEqual(iniData.GetSection("1", true).GetPropertyRawValue("key1", ""), " value1");

            parser.Configuration.SkipInvalidLines = false;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.DuplicateBehaviour.DisallowAndStopWithError;
            Assert.ThrowsException<IniParsingException>(() => { iniData = parser.Parse(_data); });
        }

        [TestMethod]
        public void TestTrimSections()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.TrimSections = true;
            var iniData = parser.Parse(_data);
            Assert.IsNotNull(iniData.GetSection("qwer"));

            parser.Configuration.TrimSections = false;
            iniData = parser.Parse(_data);
            Assert.IsNotNull(iniData.GetSection("qwer "));
        } 
        
        [TestMethod]
        public void TestTrimPropertiesKey()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.TrimPropertiesKey = true;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyValue("key1", ""), " val[ue1");
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyValue("key1 ", ""), "");

            parser.Configuration.TrimPropertiesKey = false;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyValue("key1", ""), "");
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyValue("key1 ", ""), " val[ue1");
        }

        [TestMethod]
        public void TestTrimPropertiesValue()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.TrimPropertiesValue = true;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyValue("key1", ""), "val[ue1");

            parser.Configuration.TrimPropertiesValue = false;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").GetPropertyValue("key1", ""), " val[ue1");
        }

        [TestMethod]
        public void TestParseComments()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.ParseComments = true;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments.Count, 2);

            parser.Configuration.ParseComments = false;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments.Count, 0);
        }

        [TestMethod]
        public void TestTrimComments()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.TrimComments = false;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments[0], " line comment test1  ");

            parser.Configuration.TrimComments = true;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments[0], "line comment test1");
        }

        [TestMethod]
        public void TestRemoveCommentString()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.RemoveCommentString = false;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments[0], "; line comment test1  ");

            parser.Configuration.RemoveCommentString = true;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments[0], " line comment test1  ");
        }

        [TestMethod]
        public void TestParseBlankLineAsComment()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.ParseBlankLineAsComment = false;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments.Count, 2);

            parser.Configuration.ParseBlankLineAsComment = true;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").Comments.Count, 3);
            Assert.AreEqual(iniData.GetSection("qwer").Comments[1], "");
        }     
        
        [TestMethod]
        public void TestParseCommentAfterProperty()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.ParseCommentAfterProperty = false;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key1").CommentAfterValue, "");

            parser.Configuration.ParseCommentAfterProperty = true;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("1").GetProperty("key1").CommentAfterValue, " test comment after value");
        }

        [TestMethod]
        public void TestParseCommentAfterSection()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.ParseCommentAfterSection = false;
            var iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").CommentAfterSectionName, "");

            parser.Configuration.ParseCommentAfterSection = true;
            iniData = parser.Parse(_data);
            Assert.AreEqual(iniData.GetSection("qwer").CommentAfterSectionName, " test comment after section name ");
        }   
        
        [TestMethod]
        public void TestInvalidLineAsComment()
        {
            var data = @";test invalid line as comment
invalid line 
[qwe]
key1 = value1";

            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Scheme.CommentStrings.Add("//");

            parser.Configuration.InvalidLineAsComment = true;
            var iniData = parser.Parse(data);
            Assert.AreEqual(iniData.GetSection("qwe").Comments.Count, 2);
            Assert.AreEqual(iniData.GetSection("qwe").Comments[1], "invalid line ");

            parser.Configuration.InvalidLineAsComment = false;
            iniData = parser.Parse(data);
            Assert.AreEqual(iniData.GetSection("qwe").Comments.Count, 1);
        }
    }
}
