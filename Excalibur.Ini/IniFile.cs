using System.IO;
using System.Text;

namespace Excalibur.Ini
{
    public class IniFile
    {
        public IniDataParser Parser { get; private set; }
        public IIniDataFormatter Formatter { get; private set; }
        public IniFormattingConfiguration FormattingConfiguration { get; private set; }

        public IniFile()
        {
            Parser = new IniDataParser();
            Formatter = new IniDataFormatter();
            FormattingConfiguration = new IniFormattingConfiguration();
        }

        public IniFile(IniDataParser parser, IniDataFormatter formatter, IniFormattingConfiguration formattingConfiguration)
        {
            Parser = parser;
            Formatter = formatter;
            FormattingConfiguration = formattingConfiguration;
        }

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
