using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Goldnote.Data;
using Microsoft.EntityFrameworkCore;
using Goldnote.Models;
namespace Goldnote
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var host= CreateHostBuilder(args).Build();


            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                /*try
                {*/


                services.GetRequiredService<ILogger<Program>>().Log(LogLevel.Debug, "image----------------------------------------------------------------------------");

                var imageModelDbContext = services.GetRequiredService<ImageModelDbContext>();
                imageModelDbContext.Database.Migrate();
                /*}
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }*/
            }
            //データベースマイグレーション
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var mvcGoldnoteContext = services.GetRequiredService<MvcGoldnoteContext>();
                    mvcGoldnoteContext.Database.Migrate();

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {

                    var userDbContext = services.GetRequiredService<UserDbContext>();
                    userDbContext.Database.Migrate();


                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }


            //ロールの追加
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var role = "Administrator";

                try
                {
                    Task<IdentityResult> roleResult;

                    Task<bool> hasRole = roleManager.RoleExistsAsync(role);
                    hasRole.Wait();

                    if (!hasRole.Result)
                    {
                        roleResult = roleManager.CreateAsync(new IdentityRole(role));
                        roleResult.Wait();
                    }
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating Role.");
                }
            }

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var role = "Editor";

                try
                {
                    Task<IdentityResult> roleResult;

                    Task<bool> hasRole = roleManager.RoleExistsAsync(role);
                    hasRole.Wait();

                    if (!hasRole.Result)
                    {
                        roleResult = roleManager.CreateAsync(new IdentityRole(role));
                        roleResult.Wait();
                    }
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating Role.");
                }
            }


            


            host.Run();




        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
