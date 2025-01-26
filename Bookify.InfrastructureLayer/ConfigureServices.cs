using Bookify.Infrastructure.Persistence;
using Bookify.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure;
public static class InfrastructureDependencyInjectionConfig
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration, WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString!));

        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));





        services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<ApplicationUser>>();
        services.Configure<SecurityStampValidatorOptions>(options =>
               options.ValidationInterval = TimeSpan.Zero);

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;
        });


        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();
        services.AddTransient<IWhatsAppService, WhatsAppService>();

        services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
        services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

        builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection(nameof(TwilioSettings)));

        services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
        services.AddHangfireServer();




        services.Configure<AuthorizationOptions>(options =>
                 options.AddPolicy("AdminsOnly", policy =>
                 {
                     policy.RequireAuthenticatedUser();
                     policy.RequireRole(AppRoles.Admin);
                 }));


        return services;
    }
}