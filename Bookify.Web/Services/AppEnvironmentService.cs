namespace Bookify.Web.Services
{
    public class AppEnvironmentService : IAppEnvironmentService
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public AppEnvironmentService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetRoothPath => _webHostEnvironment.WebRootPath;



        public bool IsDevelopment => _webHostEnvironment.IsDevelopment();


    }
}
