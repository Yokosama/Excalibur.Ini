using System.IO;
using System.Text;

namespace Excalibur.Ini
{
    /// <summary>
    /// Ini文件辅助类
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// IniData解析器
        /// </summary>
        public IniDataParser Parser { get; private set; }
        /// <summary>
        /// IniData格式化
        /// </summary>
        public IIniDataFormatter Formatter { get; private set; }
        /// <summary>
        /// 格式化配置
        /// </summary>
        public IniFormattingConfiguration FormattingConfiguration { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>

        public IniFile()
        {
            Parser = new IniDataParser();
            Formatter = new IniDataFormatter();
            FormattingConfiguration = new IniFormattingConfiguration();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parser">IniData解析器</param>
        /// <param name="formatter">IniData格式化类</param>
        /// <param name="formattingConfiguration">格式化配置</param>
        public IniFile(IniDataParser parser, IIniDataFormatter formatter, IniFormattingConfiguration formattingConfiguration)
        {
            Parser = parser;
            Formatter = formatter;
            FormattingConfiguration = formattingConfiguration;
        }

        /// <summary>
        /// 加载ini文件
        /// </summary>
        /// <param name="file">ini文件路径</param>
        /// <param name="encoding">ini文件编码</param>
        /// <returns>解析后的IniData对象</returns>
        public IniData Load(string file, Encoding encoding)
        {
            using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, encoding))
                {
                    return Parser.Parse(sr);
                }
            }
        }

        /// <summary>
        /// 保存IniData到文件中
        /// </summary>
        /// <param name="iniData">IniData</param>
        /// <param name="file">保存后的文件路径</param>
        /// <param name="encoding">保存后的ini文件编码</param>
        /// <param name="overwrite">存在相同文件时是否覆盖</param>
        /// <returns>true：保存成功；false：存在相同文件，保存失败</returns>
        public bool Save(IniData iniData, string file, Encoding encoding, bool overwrite = false)
        {
            if(File.Exists(file) && !overwrite)
            {
                return false;
            }

            var content = Formatter.Format(iniData, FormattingConfiguration);
            File.WriteAllText(file, content, encoding);
            return true;
        }
    }
}
