using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.F2FPay.AspnetCore;
using Components;
using Components.Middleware;
using Infrastructure.log;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Repository;
using static Components.Filter.SigFilter;

namespace CoreFrame
{

    public class Startup
    {
        IConfiguration Configuration;
        public Startup(IConfiguration Configuration)
        {

            this.Configuration = Configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //数据库上下文注入 使用DbContext池，提高性能 
            services.AddDbContextPool<SyDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));
            //数据服务注入
            services.AddDataService();
            //调用缓存
            services.AddDistributedMemoryCache();
            //注册log4net 日志
            Log4netHelper.Repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(Log4netHelper.Repository, new FileInfo(Environment.CurrentDirectory + "/Config/log4net.config"));
            //配置支付宝alipay服务
            ConfigureAlipay(services);
            //全局配置Json序列化处理
            services.AddControllersWithViews(config => {
                config.Filters.Add(typeof(SignFilter));
            })
      .AddNewtonsoftJson(options =>
        options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddRazorPages();
        }
        private void ConfigureAlipay(IServiceCollection services)
        {
            var alipayOptions = Configuration.GetSection("Alipay").Get<AlipayOptions>();
           //支付宝支付接口
            services.AddAlipay(options => options.SetOption(alipayOptions))
           //支付宝面对面接口
            .AddAlipayF2F();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //伪静态页 --静态页会映射为相应路由
            app.UseStaticFiles();
            app.UseMiddleware<StaticRewrite>();
            #region 路由节点
            //使用路由
            app.UseRouting();
            //配置终结点
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //pattern: "{controller=play}/{action=play}/{id?}");
                endpoints.MapAreaControllerRoute(
                    name: "areas", "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                //endpoints.MapHub<ChatHub>("/chatHub");
            });
            #endregion
        }
    }
}
