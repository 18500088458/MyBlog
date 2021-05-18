using System;
using SqlSugar;

namespace MyBlog.Model
{
    /// <summary>
    /// 编号
    /// </summary>
    public class BaseId
    {
        [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public int Id { get; set; }
    }
}
