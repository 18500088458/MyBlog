using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MyBlog.Model
{
    /// <summary>
    /// 博客文章类别
    /// </summary>
    public class TypeInfo:BaseId
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(12)")]
        public string Name { get; set; }
    }
}
