using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            //默认配置
            //环境变量(DOTNET开头)
            //加载命令行参数
            //加载项目的配置文件
                //加载应用配置
                //加载项目环境变量
                //配置默认日志组件
            Host.CreateDefaultBuilder(args)//通用主机
                //调用这里的扩展方法，执行自定义配置
                //启用kestrel
                .ConfigureWebHostDefaults(webBuilder =>//web主机
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
