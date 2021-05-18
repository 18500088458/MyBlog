using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyBlog.WebApi.Utility.ApiResult;
using MyBlog.IService;
using MyBlog.Model;
using MyBlog.WebApi.Utility._MD5;
using AutoMapper;
using MyBlog.Model.Dto;

namespace MyBlog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WriterInfoController : ControllerBase
    {
        private IWriterInfoService _iWriterInfoService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iTypeInfoService"></param>
        public WriterInfoController(IWriterInfoService iWriterInfoService)
        {
            _iWriterInfoService = iWriterInfoService;
        }

        /// <summary>
        /// 作者信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("WriterInfos")]
        public async Task<ActionResult<ApiResult>> GetWriterInfo()
        {
            var data = await _iWriterInfoService.QueryAsync();

            if (data == null || data.Count == 0)
            {
                return ApiResultHelper.Error("没有更多的作者信息");
            }

            return ApiResultHelper.Success(data);
        }

        /// <summary>
        /// 添加作者信息
        /// </summary>
        /// <param name="name"></param>   
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResult>> Create(string name, string userName, string userPwd)
        {
            WriterInfo item = new WriterInfo
            {
                Name = name,
                UserName = userName,
                UserPwd = MD5Helper.MD5Encrypt32(userPwd)
            };

            var dbItem = await _iWriterInfoService.FindAsync(w => w.UserName == userName);
            if (dbItem != null)
            {
                return ApiResultHelper.Error("该账号已存在，请知悉！");
            }

            var isCreated = await _iWriterInfoService.CreateAsync(item);
            if (!isCreated)
            {
                return ApiResultHelper.Error("添加失败！");
            }

            return ApiResultHelper.Success(item);
        }

        /// <summary>
        /// 删除作者信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            var isDeleted = await _iWriterInfoService.DeleteAsync(id);
            if (!isDeleted)
            {
                return ApiResultHelper.Error("删除失败！");
            }

            return ApiResultHelper.Success(isDeleted);
        }

        /// <summary>
        /// 修改作家信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        public async Task<ActionResult<ApiResult>> Edit(string name)
        {
            int id = Convert.ToInt32(this.User.FindFirst("Id").Value);
            var writerItem = await _iWriterInfoService.FindAsync(id);

            if (writerItem != null)
            {
                writerItem.Name = name;

                var result = await _iWriterInfoService.EditAsync(writerItem);
                if (!result)
                {
                    return ApiResultHelper.Error("修改失败");
                }
                else
                {
                    return ApiResultHelper.Success("修改成功");
                }
            }

            return ApiResultHelper.Error("没找到对应作者信息");
        }

        /// <summary>
        /// 获取作家信息
        /// </summary>
        /// <param name="iMapper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FindWriter")]
        public async Task<ApiResult> FindWriter([FromServices] IMapper iMapper, int id)
        {
            var writerInfo = await _iWriterInfoService.FindAsync(id);
            var writerDto = iMapper.Map<BlogNewsDto>(writerInfo);
            return ApiResultHelper.Success(writerDto);
        }
    }
}
