using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyBlog.WebApi.Utility.ApiResult;
using MyBlog.IService;
using MyBlog.Model;

namespace MyBlog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeInfoController : ControllerBase
    {
        private ITypeInfoService _iTypeInfoService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iTypeInfoService"></param>
        public TypeInfoController(ITypeInfoService iTypeInfoService)
        {
            _iTypeInfoService = iTypeInfoService;
        }

        /// <summary>
        /// 文章类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("TypeInfos")]
        public async Task<ActionResult<ApiResult>> GetTypeInfo()
        {
            var data = await _iTypeInfoService.QueryAsync();

            if (data == null || data.Count == 0)
            {
                return ApiResultHelper.Error("没有更多的文章类型");
            }

            return ApiResultHelper.Success(data);
        }

        /// <summary>
        /// 添加文章类型
        /// </summary>
        /// <param name="name"></param>        
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResult>> Create(string name)
        {
            TypeInfo item = new TypeInfo
            {
                Name = name
            };

            var isCreated = await _iTypeInfoService.CreateAsync(item);
            if (!isCreated)
            {
                return ApiResultHelper.Error("添加失败！");
            }

            return ApiResultHelper.Success(item);
        }

        /// <summary>
        /// 删除文章类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            var isDeleted = await _iTypeInfoService.DeleteAsync(id);
            if (!isDeleted)
            {
                return ApiResultHelper.Error("删除失败！");
            }

            return ApiResultHelper.Success(isDeleted);
        }

        /// <summary>
        /// 修改文章类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        public async Task<ActionResult<ApiResult>> Edit(int id, string name)
        {
            var item = await _iTypeInfoService.FindAsync(id);
            if (item == null)
            {
                return ApiResultHelper.Error("没找到对应的文章类型信息！");
            }

            item.Name = name;            

            var result = await _iTypeInfoService.EditAsync(item);
            if (!result)
            {
                return ApiResultHelper.Error("修改失败！");
            }

            return ApiResultHelper.Success(result);
        }
    }
}
