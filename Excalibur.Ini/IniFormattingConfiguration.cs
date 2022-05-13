﻿using System;

namespace Excalibur.Ini
{
    /// <summary>
    /// Ini内容格式化配置
    /// </summary>
    public class IniFormattingConfiguration : ICloneable<IniFormattingConfiguration>
    {
        /// <summary>
        /// 新行的字符串 Environment.NewLine
        /// </summary>
        public string NewLineString => Environment.NewLine;

        /// <summary>
        /// 属性关键字和赋值符之间的空格数，默认值：1
        /// </summary>
        public uint NumberSpacesBetweenKeyAndAssignment
        {
            set
            {
                SpacesBetweenKeyAndAssignment = new string(' ', (int)value);
            }
        }
        /// <summary>
        /// 属性关键字和赋值符之间的空格，默认值：' '
        /// </summary>
        public string SpacesBetweenKeyAndAssignment { get; private set; }

        /// <summary>
        /// 赋值符和属性值之间的空格数，默认值：1
        /// </summary>
        public uint NumberSpacesBetweenAssignmentAndValue
        {
            set
            {
                SpacesBetweenAssignmentAndValue = new string(' ', (int)value);
            }
        }

        /// <summary>
        /// 赋值符和属性值之间的空格，默认值：' '
        /// </summary>
        public string SpacesBetweenAssignmentAndValue { get; private set; }

        /// <summary>
        /// 需要前面字符不能为空， 在SectionName前空一行，默认值：false
        /// </summary>
        public bool NewLineBeforeSectionName { get; set; } = false;

        /// <summary>
        /// 在SectionName后空一行，默认值：false
        /// </summary>
        public bool NewLineAfterSectionName { get; set; } = false;

        /// <summary>
        /// 在属性前空一行，默认值：false
        /// </summary>
        public bool NewLineAfterProperty { get; set; } = false;

        /// <summary>
        /// 在属性后空一行，默认值：false
        /// </summary>
        public bool NewLineBeforeProperty { get; set; } = false;

        /// <summary>
        /// 在节点定义后空一行，默认值：true
        /// </summary>
        public bool NewLineAfterSection { get; set; } = true;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IniFormattingConfiguration()
        {
            NumberSpacesBetweenKeyAndAssignment = 1;
            NumberSpacesBetweenAssignmentAndValue = 1;
        }      
        
        /// <summary>
        /// 复制其他配置的构造函数
        /// </summary>
        /// <param name="other">其他格式化配置</param>
        public IniFormattingConfiguration(IniFormattingConfiguration other)
        {
            SpacesBetweenKeyAndAssignment = other.SpacesBetweenKeyAndAssignment;
            SpacesBetweenAssignmentAndValue = other.SpacesBetweenAssignmentAndValue;
            NewLineBeforeSectionName = other.NewLineBeforeSectionName;
            NewLineAfterSectionName = other.NewLineAfterSectionName;
            NewLineAfterProperty = other.NewLineAfterProperty;
            NewLineBeforeProperty = other.NewLineAfterSection;
        }

        /// <summary>
        /// 复制格式化配置对象
        /// </summary>
        /// <returns></returns>
        public IniFormattingConfiguration Clone()
        {
            return new IniFormattingConfiguration(this);
        }
    }
}
