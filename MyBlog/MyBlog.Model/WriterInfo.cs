using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MyBlog.Model
{
    /// <summary>
    /// 博客文章作者
    /// </summary>
    public class WriterInfo:BaseId
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDataType ="nvarchar(8)")]
        public string Name { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(20)")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(30)")]
        public string UserPwd { get; set; }
    }
}
