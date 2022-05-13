using System;
using System.Reflection;

namespace Excalibur.Ini
{
    /// <summary>
    /// 解析时异常
    /// </summary>
    public class IniParsingException : Exception
    {
        /// <summary>
        /// 库版本
        /// </summary>
        public Version LibVersion { get; }
        /// <summary>
        /// 异常行号
        /// </summary>
        public uint LineNumber { get; }
        /// <summary>
        /// 异常行的内容
        /// </summary>
        public string LineContents { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg">异常信息</param>
        /// <param name="lineNumber">异常行号</param>
        public IniParsingException(string msg, uint lineNumber)
            : this(msg, lineNumber, string.Empty, null)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg">异常信息</param>
        /// <param name="innerException">内部异常对象</param>
        public IniParsingException(string msg, Exception innerException)
            : this(msg, 0, string.Empty, innerException)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="lineNumber"></param>
        /// <param name="lineContents"></param>
        public IniParsingException(string msg, uint lineNumber, string lineContents)
            : this(msg, lineNumber, lineContents, null)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg">异常信息</param>
        /// <param name="lineNumber">异常所在行号</param>
        /// <param name="lineContents">异常行的内容</param>
        /// <param name="innerException"></param>
        public IniParsingException(string msg, uint lineNumber, string lineContents, Exception innerException)
            : base(
                $"Line: {lineNumber} Source: \'{lineContents}\' Parsing failed, {msg}",
                innerException)
        {
            LibVersion = GetAssemblyVersion();
            LineNumber = lineNumber;
            LineContents = lineContents;
        }

        private Version GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
