using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyBlog.WebApi.Utility.ApiResult;
using MyBlog.IService;
using MyBlog.Model;
using SqlSugar;
using AutoMapper;
using MyBlog.Model.Dto;

namespace MyBlog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogNewsController : ControllerBase
    {
        private IBlogNewsService _iBlogNewsService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iBlogNewsService"></param>
        public BlogNewsController(IBlogNewsService iBlogNewsService)
        {
            _iBlogNewsService = iBlogNewsService;
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("BlogNews")]
        public async Task<ActionResult<ApiResult>> GetBlogNews()
        {
            int id = Convert.ToInt32(this.User.FindFirst("id").Value);            
            var data = await _iBlogNewsService.QueryAsync(c=> c.WriterId == id);

            if (data == null || data.Count == 0)
            {
                return ApiResultHelper.Error("没有更多的文章");
            }

            return ApiResultHelper.Success(data);
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResult>> Create(string title, string content, int typeId)
        {
            BlogNews item = new BlogNews
            {
                BrowseCount = 0,
                Content = content,
                LikeCount = 0,
                Time = DateTime.Now,
                Title = title,
                TypeId = typeId,
                WriterId = Convert.ToInt32(this.User.FindFirst("id").Value)
            };

            var isCreated = await _iBlogNewsService.CreateAsync(item);
            if (!isCreated)
            {
                return ApiResultHelper.Error("添加失败！");
            }

            return ApiResultHelper.Success(item);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            var isDeleted = await _iBlogNewsService.DeleteAsync(id);
            if (!isDeleted)
            {
                return ApiResultHelper.Error("删除失败！");
            }

            return ApiResultHelper.Success(isDeleted);
        }

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        public async Task<ActionResult<ApiResult>> Edit(int id, string title, string content, int typeId)
        {
            var blogNewsItem = await _iBlogNewsService.FindAsync(id);
            if (blogNewsItem == null)            
            {
                return ApiResultHelper.Error("没找到对应的文章信息！");
            }

            blogNewsItem.Title = title;
            blogNewsItem.Content = content;
            blogNewsItem.TypeId = typeId;

            var result = await _iBlogNewsService.EditAsync(blogNewsItem);
            if (!result)
            {
                return ApiResultHelper.Error("修改失败！");
            }

            return ApiResultHelper.Success(result);
        }

        [HttpGet("BlogNewsPage")]
        public async Task<ApiResult> GetBlogNewsPage([FromServices]IMapper iMapper,int page, int size) 
        {
            RefAsync<int> total = 0;
            var blogNewsList = _iBlogNewsService.QueryAsync(page,size,total);
            try
            {
                var blogNewsDtoList = iMapper.Map<List<BlogNewsDto>>(blogNewsList);
                return ApiResultHelper.Success(blogNewsDtoList, total);
            }
            catch (Exception ex)
            {
                return ApiResultHelper.Error("AutoMapper映射错误");
            }
        }
    }
}
