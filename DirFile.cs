using System;
using System.Collections.Generic;
using System.Text;

namespace H1Z1_JS语言文件生成器
{
    public class DirFileRecord
    {
        /// <summary>
        /// 索引
        /// </summary>
        public long Index { get; set; }
        /// <summary>
        /// 开始位置
        /// </summary>
        public long Start { get; set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
    }
}
