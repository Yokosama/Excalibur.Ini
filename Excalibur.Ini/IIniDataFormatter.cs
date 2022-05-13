namespace Excalibur.Ini
{
    /// <summary>
    /// 格式化IniData到字符串中
    /// </summary>
    public interface IIniDataFormatter
    {
        /// <summary>
        /// 格式化iniData为字符串
        /// </summary>
        /// <param name="iniData">Ini数据</param>
        /// <param name="format">格式化配置</param>
        /// <returns>格式化后的字符串</returns>
        string Format(IniData iniData, IniFormattingConfiguration format);
    }
}
