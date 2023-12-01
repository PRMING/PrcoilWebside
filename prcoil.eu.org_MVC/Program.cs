/*
                   _ooOoo_
                  o8888888o
                  88" . "88
                  (| -_- |)
                  O\  =  /O
               ____/`---'\____
             .'  \\|     |//  `.
            /  \\|||  :  |||//  \
           /  _||||| -:- |||||-  \
           |   | \\\  -  /// |   |
           | \_|  ''\---/''  |   |
           \  .-\__  `-`  ___/-. /
         ___`. .'  /--.--\  `. . __
      ."" '<  `.___\_<|>_/___.'  >'"".
     | | :  `- \`.;`\ _ /`;.`/ - ` : | |
     \  \ `-.   \_ __\ /__ _/   .-` /  /
======`-.____`-.___\_____/___.-`____.-'======
                   `=---='
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            佛祖保佑       永无BUG
*/

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace prcoil.eu.org_MVC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //---------------------------------------------------------------------------------------------------------------------
        //跨域权限设置
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Policy1",
                policy =>
                {
                    policy.WithOrigins("http://example.com",
                            "http://www.contoso.com")
                        .WithMethods("PUT", "DELETE", "GET");
                });

            options.AddPolicy("AnotherPolicy",
                policy =>
                {
                    policy.WithOrigins("https://www.prcoil.eu.org",
                            "http://127.0.0.1:5501",
                            "http://localhost:63342",
                            "https://testipv6.prcoil.eu.org",
                            "https://localhost:7190",
                            "https://aspnet.prcoil.eu.org")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // 在此处配置JWT身份验证
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "prcoil", //发行人Issuer

                    ValidateAudience = true,
                    ValidAudience = "webprcoil", //订阅人Audience

                    ValidateLifetime = true, //是否验证失效时间

                    ValidateIssuerSigningKey = true, //是否验证SecurityKey
                    //SecurityKey
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        "amA9gMV2GQNhg4NAeJe5sBiclyvv7HQD9eGPr3y1CTPUmXJewqo1UMyW/ouxZLJdqDO8351LBKtD8+S7pvrZsw==")),
                    ClockSkew = TimeSpan.FromSeconds(30), //过期时间容错值，解决服务器端时间不同步问题（秒）
                    RequireExpirationTime = true
                };
            });

        // 启用Swagger
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //---------------------------------------------------------------------------------------------------------------------

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // 启用Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // 在这里配置其他服务-----------------------------------------------------------------------------------------------------

        //跨域
        app.UseCors();
        //调用中间件：UseAuthentication（认证），必须在所有需要身份认证的中间件前调用，比如 UseAuthorization（授权）。
        app.UseAuthentication();

        //---------------------------------------------------------------------------------------------------------------------

        app.UseAuthorization();

        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}