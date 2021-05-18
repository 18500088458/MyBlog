using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;//.net core自带的依赖注入框架
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar.IOC;
using MyBlog.IRepository;
using MyBlog.Repository;
using MyBlog.IService;
using MyBlog.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyBlog.WebApi.Utility._AutoMapper;

namespace MyBlog.WebApi
{
    //配置WB应用所需要的【服务】和【中间件】
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //该方法由运行时调用。
        //使用此方法向容器添加服务。
        //IServiceCollection:服务容器、服务集合
        //      想要用到什么服务，就添加进来
        //                      logger
        //                      swagger
        //                      sqlsugar                            
        //                      ioc注入容器
        //                      MemoryCache
        //                      自定义服务【服务生命期 类型生命周期】
        //                          注册自定义服务的时候，必须要选择一个生命周期
        //                                    生命周期分类
        //                                      瞬时
        //                                          每次从服务容器里请求实例时，都会创建一个新的实例
        //                                          services.AddTransient();               
        //                                      作用域
        //                                          线程单例，在同一个线程(请求)里，只实例化一次
        //                                          线程作用域
        //                                          services.AddScoped();
        //                                      单例
        //                                          全局单例，每一次都是使用相同的单例
        //                                          services.AddSingleton();             
        //                      ...
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加对控制器和API相关功能的支持，但是不支持视图和页面
            services.AddControllers();

            ////添加对控制器和API相关功能的支持，同时也支持视图和页面
            //ASP.NET CORE 3.X Mvc模板默认使用
            //services.AddControllersWithViews();

            ////ASP.NET Core 2.X
            //services.AddMvc();

            ////Razor Pages和最小控制器的支持
            //services.AddRazorPages();

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyBlog.WebApi", Version = "v1" });

                #region Swagger使用鉴权组件

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "直接在下框中输入Bearer {token}(注意两者之间是一个空格)",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    {
                        new OpenApiSecurityScheme
                        { 
                            Reference = new OpenApiReference
                            { 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });

                #endregion
            });

            #endregion

            #region SqlSugarIoc

            services.AddSqlSugar(new IocConfig()
            {
                ConnectionString = this.Configuration["SqlConn"],
                DbType = IocDbType.SqlServer,
                IsAutoCloseConnection = true//自动释放
            });

            #endregion

            #region IOC依赖注入

            services.AddCustomeIOC();

            #endregion

            #region JWT鉴权

            services.AddCustomerJWT();

            #endregion

            #region AutoMapper

            services.AddAutoMapper(typeof(CustomAutoMapperProfile));

            #endregion

            //添加自定义服务
            //--------------------------------------------
            //需要考虑以下问题
            //  你添加的服务，应该是什么生命周期？
            //  你添加的服务，要不要配置？
            //像services.AddControllers();
            //不需要考虑生命周期；不需要考虑配置，作者在开发的时候就想好了，并且封装好了
        }

        //该方法由运行时调用。
        //使用此方法配置HTTP请求管道。
        //配置中间件，中间件组成管道
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //IApplicationBuilder——该类提供配置应用程序请求的机制
            //IApplicationBuilder.Use将中间件委托添加到应用程序的请求管道

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyBlog.WebApi v1"));
            }

            app.UseRouting();

            //授权
            app.UseAuthorization();
            //鉴权
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    /// <summary>
    /// IServiceCollection扩展类
    /// </summary>
    public static class IOCExtend
    {
        private const string SecureKey = "SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF";

        /// <summary>
        /// IOC扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomeIOC(this IServiceCollection services)
        {
            //注册服务 .net core自带的注册方式
            //services.AddSingleton
            //services.AddScoped
            //services.AddTransient

            //可以使用其他的方式注册服务
            //  Unity
            //  AutoFac
            //  Ninject

            services.AddScoped<IBlogNewsRepository, BlogNewsRepository>();
            services.AddScoped<IBlogNewsService, BlogNewsService>();

            services.AddScoped<IWriterInfoRepository, WriterInfoRepository>();
            services.AddScoped<IWriterInfoService, WriterInfoService>();

            services.AddScoped<ITypeInfoRepository, TypeInfoRepository>();
            services.AddScoped<ITypeInfoService, TypeInfoService>();

            return services;
        }

        /// <summary>
        /// JWT扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomerJWT(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)                                                         
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecureKey)),
                        ValidateIssuer = true,
                        ValidIssuer = "http://localhost:6060",//jwt地址
                        ValidateAudience = true,
                        ValidAudience = "http://localhost:5000",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(60)
                    };
                });

            return services;
        }
    }
}
