
namespace Bookify.Domain.Dtos
{
    public record DataTableFilterationDto
    {
        public int skip { get; set; }
        public int pageSize { get; set; }
        public string? searchValue { get; set; }
        public string? sortColumnIndex { get; set; }
        public string? sortColumn { get; set; }
        public string? sortColumnDirection { get; set; }



    }
}
