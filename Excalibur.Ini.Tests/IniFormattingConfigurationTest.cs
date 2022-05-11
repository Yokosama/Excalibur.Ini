using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class IniFormattingConfigurationTest
    {
       private readonly string _data = @"[1]
key1 = val[ue1
key2 = value2";

        [TestMethod]
        public void TestSpacesBetweenKeyAndAssignment()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(_data);

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration
            {
                NumberSpacesBetweenKeyAndAssignment = 0
            };
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @"[1]
key1= val[ue1
key2= value2
");
        }

        [TestMethod]
        public void TestSpacesBetweenAssignmentAndValue()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(_data);

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration
            {
                NumberSpacesBetweenAssignmentAndValue = 0
            };
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @"[1]
key1 =val[ue1
key2 =value2
");
        }

        [TestMethod]
        public void TestNewLineBeforeSectionName()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(@";TestNewLineBeforeSectionName
[1]
key1 = val[ue1
key2 = value2");

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration
            {
                NewLineBeforeSectionName = true
            };
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @";TestNewLineBeforeSectionName

[1]
key1 = val[ue1
key2 = value2
");
        }

        [TestMethod]
        public void TestNewLineAfterSectionName()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(_data);

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration
            {
                NewLineAfterSectionName = true
            };
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @"[1]

key1 = val[ue1
key2 = value2
");
        }

        [TestMethod]
        public void TestNewLineAfterProperty()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(_data);

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration
            {
                NewLineAfterProperty = true
            };
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @"[1]
key1 = val[ue1

key2 = value2

");
        }

        [TestMethod]
        public void TestNewLineBeforeProperty()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(_data);

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration
            {
                NewLineBeforeProperty = true
            };
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @"[1]

key1 = val[ue1

key2 = value2
");
        }
    }
}
