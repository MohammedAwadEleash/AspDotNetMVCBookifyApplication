using Bookify.Application;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Tasks;
using Bookify.Web;
using Bookify.Web.Seeds;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration, builder);
builder.Services.AddApplicationServices();
builder.Services.AddWebServices();
//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>()






//Add Serilog 
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();
var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "Deny");
    await next();
});

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

}
else
{
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    Secure = CookieSecurePolicy.Always
//});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
var scope = scopeFactory.CreateScope();

// to create instance of RoleManager ,UserManager  without use constractor  oe new()
var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

await DefaultRoles.SeedRolesAsync(roleManger);
await DefaultUsers.SeedAdminUserAsync(userManger);

// Use hangfire
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Bookify Dashboard",
   // IsReadOnlyFunc = (DashboardContext context) => true ,
    Authorization = new IDashboardAuthorizationFilter[]
    {

        new HangfireAuthorizationFilter("AdminsOnly")
    }

});




var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
var appEnvironmentService = scope.ServiceProvider.GetRequiredService<IAppEnvironmentService>();
var emailBodyBuilder = scope.ServiceProvider.GetRequiredService<IEmailBodyBuilder>();
var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();


var hangfireTasks = new HangfireTasks(dbContext, appEnvironmentService, emailBodyBuilder, emailSender, whatsAppService);
RecurringJob.AddOrUpdate(() => hangfireTasks.PrepareExpirationAlert(), "0 18 * * *");
RecurringJob.AddOrUpdate(() => hangfireTasks.RentalExpirationAlert(), "0 14 * * *");

// end hangfire code 


app.Use(async (context, next) =>
{
    LogContext.PushProperty("UserId", context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    LogContext.PushProperty("UserName", context.User.FindFirst(ClaimTypes.Name)?.Value);

    await next();
});

app.UseSerilogRequestLogging();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
