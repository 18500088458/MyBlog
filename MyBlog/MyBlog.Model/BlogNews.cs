using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MyBlog.Model
{
    /// <summary>
    /// 博客文章
    /// </summary>
    public class BlogNews:BaseId
    {
        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(ColumnDataType ="nvarchar(50)")]
        public string Title { get; set; }

        /// <summary>
        /// 文章内容g
        /// </summary>
        [SugarColumn(ColumnDataType ="text")]
        public string Content { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; }
 
        /// <summary>
        /// 浏览量
        /// </summary>
        public int BrowseCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 文章类别编号
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 文章类别信息
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public TypeInfo TypeInfo { get; set; }

        /// <summary>
        /// 作者编号
        /// </summary>
        public int WriterId { get; set; }

        /// <summary>
        /// 作者信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public WriterInfo WriterInfo { get; set; }
    }
}
