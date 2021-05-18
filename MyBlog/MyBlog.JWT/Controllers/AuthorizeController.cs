using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyBlog.IService;
using MyBlog.JWT.Utility._MD5;
using MyBlog.JWT.Utility.ApiResult;
using MyBlog.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private const string SecureKey = "SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF";
        private IWriterInfoService _iWriterInfoService;

        public AuthorizeController(IWriterInfoService iWriterInfoService)
        {
            _iWriterInfoService = iWriterInfoService;
        }

        /// <summary>
        /// 授权【提供token值】
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Login(string username, string pwd)
        {
            string encrypthPwd = MD5Helper.MD5Encrypt32(pwd);
            var writerItem = await _iWriterInfoService.FindAsync(c => c.UserName == username && c.UserPwd == encrypthPwd);
            if (writerItem != null)
            {           
                var claims = new Claim[] { 
                    new Claim(ClaimTypes.Name, writerItem.Name),
                    new Claim("Id", writerItem.Id.ToString()),
                    new Claim("UserName", writerItem.UserName)
                    //不能放敏感信息
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecureKey));

                var token = new JwtSecurityToken(
                    issuer: "http://localhost:6060",
                    audience: "http://localhost:5000",
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return ApiResultHelper.Success(jwtToken);
            }
            else
            {
                return ApiResultHelper.Error("用户名密码错误！");
            }
        }
    }
}
