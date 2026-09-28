using IPL_Franchises.Application.Interfaces;
using IPL_Franchises.Infrastructure.Data;
using IPL_Franchises.Infrastructure.Identity;
using IPL_Franchises.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IPL_Franchises.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        /*
         * DATABASE
         */

        services.AddDbContext<IPLDbContext>(
            options =>
            {
                options.UseSqlServer(
                    configuration
                        .GetConnectionString(
                            "DefaultConnection"));
            });


        /*
         * ASP.NET CORE IDENTITY
         */

        services
            .AddIdentityCore<ApplicationUser>(
                options =>
                {
                    /*
                     * USER
                     */

                    options.User
                        .RequireUniqueEmail = true;


                    /*
                     * PASSWORD RULES
                     */

                    options.Password
                        .RequiredLength = 8;

                    options.Password
                        .RequireDigit = true;

                    options.Password
                        .RequireLowercase = true;

                    options.Password
                        .RequireUppercase = true;

                    options.Password
                        .RequireNonAlphanumeric = false;
                })
            .AddEntityFrameworkStores<IPLDbContext>();


        /*
         * APPLICATION SERVICES
         */

        services.AddScoped<
            IProductService,
            ProductService>();

        services.AddScoped<
            ICartService,
            CartService>();

        services.AddScoped<
            IOrderService,
            OrderService>();


        return services;
    }
}
