namespace Bookify.Application.Comman.Interfaces
{
    public interface IAppEnvironmentService
    {
        //private readonly IWebHostEnvironment webHostEnvironment1l

        string GetRoothPath { get; }
        public bool IsDevelopment { get; }



    }
}
