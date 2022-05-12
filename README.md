# Excalibur.Ini

C#实现的ini读写库。支持.NET，.NET Core等平台。

## 特点

1、支持自定义的注释解析，如`;`，`//`，`#` 等

2、支持节点名称、属性值后的注释解析

3、支持全局属性解析

4、支持自定义ini格式，包括节点起始、结束符，键值分隔符（默认为 `=` ）

5、支持重复的节点和属性配置

6、查找节点和属性的速度快

7、更自由的解析和格式化写入配置

## 使用方式
添加命名控件：

```csharp
using Excalibur.Ini
```

通过`IniDataParser`解析ini字符串或者文件流

```csharp
var parser = new IniDataParser();

// 解析ini字符串
var iniData = parser.Parse(@"inistring");

// 解析ini文件
using (FileStream fs = File.Open("test.ini", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
{
    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
    {
        var iniData = parser.Parse(sr);
    }
}
```

通过`IniFile`加载、保存文件

```csharp
var iniFile = new IniFile();

// 加载文件
var iniData = iniFile.Load("test.ini", Encoding.Default);

// 保存文件
iniFile.Save(data, saveFile, Encoding.Default)
```

获取ini节点

```csharp
var section = iniData.GetSection("SectionName");
```

获取属性、属性值，属性值默认存储为`string`类型

```csharp
// 获取属性
var property = iniData.GetProperty("Key");

// 通过iniData获取属性值
// 获取默认的字符串值
var value = iniData.GetPropertyRawValue("SectionName", "Key", "DefaultValue")

// 获取自动转换类型后的属性值
var value = iniData.GetPropertyValue("SectionName", "Key", 0)

// 通过Section获取属性值
// 获取默认的字符串值
var value = section.GetPropertyRawValue("Key", "DefaultValue")

// 获取自动转换类型后的属性值
var value = section.GetPropertyValue("Key", 0)
```

修改ini解析格式，可以通过`IniDataParser.Scheme`进行修改：

```csharp
var parser = new IniDataParser();
// 添加注释类型
parser.Scheme.CommentStrings.Add("//");
parser.Scheme.CommentStrings.Add("#");

// 修改节点起始、结束符号
parser.Scheme.SectionStartString = "<"
parser.Scheme.SectionEndString = ">"

// 修改键值分隔符
parser.Scheme.PropertyAssignmentString = "-"
```

修改ini解析配置，可以通过`IniDataParser.Configuration`进行修改：

```csharp
var parser = new IniDataParser();
// 忽略节点名称、属性关键字的大小写
parser.Configuration.CaseInsensitive = true;

// 是否允许全局节点存在
parser.Configuration.AllowKeysWithoutSection = true;

// 解析到重复节点时的行为，允许重复添加
parser.Configuration.DuplicateSectionsBehaviour = DuplicateBehaviour.AllowAndRepeat;

// 解析属性行后的注释
parser.Configuration.ParseCommentAfterProperty = true;
```

更多ini解析配置，可以参见`IniParserConfiguration.cs`

通过`IniDataFormatter`格式化输出文本，通过`IniFormattingConfiguration`进行格式化方式配置：

```csharp
var parser = new IniDataParser();
var iniData = parser.Parse(@"inistring");

var formatter = new IniDataFormatter();
// 格式化配置
var formattingConfiguration = new IniFormattingConfiguration();
// 获取格式化后的ini字符串
var result = formatter.Format(iniData, formattingConfiguration);
```

也可以根据实际需求，自行编写格式化功能

更多使用方式可以参考测试项目