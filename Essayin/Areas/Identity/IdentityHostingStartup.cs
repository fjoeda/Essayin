using System;
using Essayin.Areas.Identity.Data;
using Essayin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Essayin.Areas.Identity.IdentityHostingStartup))]
namespace Essayin.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<EssayinUserContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("EssayinUserContextConnection")));

                services.AddDefaultIdentity<EssayinUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<EssayinUserContext>();
            });
        }
    }
}