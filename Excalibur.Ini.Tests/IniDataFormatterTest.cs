using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excalibur.Ini.Tests
{
    [TestClass]
    public class IniDataFormatterTest
    {
       private readonly string _data = @"   ; test global properties
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
        public void TestParse()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentStrings.Add("//");
            parser.Configuration.TrimPropertiesValue = true;

            var iniData = parser.Parse(_data);

            var formatter = new IniDataFormatter();
            var formattingConfiguration = new IniFormattingConfiguration();
            var result = formatter.Format(iniData, formattingConfiguration);
            Assert.AreEqual(result, @"; test global properties
globalKey1 = globalValue1
globalKey2 = globalValue2
; line comment test1    
;line comment test2
[qwer]; test comment after section name 
key1 = val[ue1
key2 = value2

; test section comment
; test section comment
[1]
; test property comment
; test property comment
key1 = value1; test comment after value
key2 = value2; test comment ; after value
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
;test end of file commments3");
        }
    }
}
