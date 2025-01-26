using Bookify.Web.Core.mapping;
using FluentValidation.AspNetCore;
using HashidsNet;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;


namespace Bookify.Web
{
    public static class DependencyInjectionConfig
    {


        public static IServiceCollection AddWebServices(this IServiceCollection services)

        {



            services.AddTransient<IAppEnvironmentService, AppEnvironmentService>();


            services.AddDataProtection().SetApplicationName(nameof(Bookify));
            services.AddSingleton<IHashids>(_ => new Hashids("f1nd1ngn3m0", minHashLength: 12));

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();



            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

            services.AddExpressiveAnnotations();


            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddControllersWithViews();

            services.AddMvc(options =>
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            );
            return services;
        }
    }
}