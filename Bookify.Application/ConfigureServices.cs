using Bookify.Application.Services;
using Bookify.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class ApplicationDependencyInjectionConfig
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {


        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IAuthorService, AuthorService>();
        services.AddTransient<IBookService, BookService>();
        services.AddTransient<IBookCopyService, BookCopyService>();
        services.AddTransient<ISubscriberService, SubscriberService>();
        services.AddTransient<IRentalService, RentalService>();
        services.AddTransient<IUserService, UserService>();
        return services;
    }
}