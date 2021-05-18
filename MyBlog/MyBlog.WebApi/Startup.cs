using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;//.net core�Դ�������ע����
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
    //����WBӦ������Ҫ�ġ����񡿺͡��м����
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //�÷���������ʱ���á�
        //ʹ�ô˷�����������ӷ���
        //IServiceCollection:�������������񼯺�
        //      ��Ҫ�õ�ʲô���񣬾���ӽ���
        //                      logger
        //                      swagger
        //                      sqlsugar                            
        //                      iocע������
        //                      MemoryCache
        //                      �Զ�����񡾷��������� �����������ڡ�
        //                          ע���Զ�������ʱ�򣬱���Ҫѡ��һ����������
        //                                    �������ڷ���
        //                                      ˲ʱ
        //                                          ÿ�δӷ�������������ʵ��ʱ�����ᴴ��һ���µ�ʵ��
        //                                          services.AddTransient();               
        //                                      ������
        //                                          �̵߳�������ͬһ���߳�(����)�ֻʵ����һ��
        //                                          �߳�������
        //                                          services.AddScoped();
        //                                      ����
        //                                          ȫ�ֵ�����ÿһ�ζ���ʹ����ͬ�ĵ���
        //                                          services.AddSingleton();             
        //                      ...
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //��ӶԿ�������API��ع��ܵ�֧�֣����ǲ�֧����ͼ��ҳ��
            services.AddControllers();

            ////��ӶԿ�������API��ع��ܵ�֧�֣�ͬʱҲ֧����ͼ��ҳ��
            //ASP.NET CORE 3.X Mvcģ��Ĭ��ʹ��
            //services.AddControllersWithViews();

            ////ASP.NET Core 2.X
            //services.AddMvc();

            ////Razor Pages����С��������֧��
            //services.AddRazorPages();

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyBlog.WebApi", Version = "v1" });

                #region Swaggerʹ�ü�Ȩ���

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "ֱ�����¿�������Bearer {token}(ע������֮����һ���ո�)",
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
                IsAutoCloseConnection = true//�Զ��ͷ�
            });

            #endregion

            #region IOC����ע��

            services.AddCustomeIOC();

            #endregion

            #region JWT��Ȩ

            services.AddCustomerJWT();

            #endregion

            #region AutoMapper

            services.AddAutoMapper(typeof(CustomAutoMapperProfile));

            #endregion

            //����Զ������
            //--------------------------------------------
            //��Ҫ������������
            //  ����ӵķ���Ӧ����ʲô�������ڣ�
            //  ����ӵķ���Ҫ��Ҫ���ã�
            //��services.AddControllers();
            //����Ҫ�����������ڣ�����Ҫ�������ã������ڿ�����ʱ�������ˣ����ҷ�װ����
        }

        //�÷���������ʱ���á�
        //ʹ�ô˷�������HTTP����ܵ���
        //�����м�����м����ɹܵ�
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //IApplicationBuilder���������ṩ����Ӧ�ó�������Ļ���
            //IApplicationBuilder.Use���м��ί����ӵ�Ӧ�ó��������ܵ�

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyBlog.WebApi v1"));
            }

            app.UseRouting();

            //��Ȩ
            app.UseAuthorization();
            //��Ȩ
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    /// <summary>
    /// IServiceCollection��չ��
    /// </summary>
    public static class IOCExtend
    {
        private const string SecureKey = "SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF";

        /// <summary>
        /// IOC��չ����
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomeIOC(this IServiceCollection services)
        {
            //ע����� .net core�Դ���ע�᷽ʽ
            //services.AddSingleton
            //services.AddScoped
            //services.AddTransient

            //����ʹ�������ķ�ʽע�����
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
        /// JWT��չ����
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
                        ValidIssuer = "http://localhost:6060",//jwt��ַ
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
