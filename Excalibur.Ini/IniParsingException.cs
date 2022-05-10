using System;
using System.Reflection;

namespace Excalibur.Ini
{
    /// <summary>
    /// 解析时异常
    /// </summary>
    public class IniParsingException : Exception
    {
        public Version LibVersion { get; }
        public uint LineNumber { get; }
        public string LineContents { get; }

        public IniParsingException(string msg, uint lineNumber)
            : this(msg, lineNumber, string.Empty, null)
        { }

        public IniParsingException(string msg, Exception innerException)
            : this(msg, 0, string.Empty, innerException)
        { }

        public IniParsingException(string msg, uint lineNumber, string lineContents)
            : this(msg, lineNumber, lineContents, null)
        { }

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
