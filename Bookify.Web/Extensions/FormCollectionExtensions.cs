namespace Bookify.Web.Extensions
{
    public static class FormCollectionExtensions
    {


        public static DataTableFilterationDto GetDataTableFilters(this IFormCollection form)
        {
            // form = Request.Form
            var sortColumnIndex = form["order[0][column]"];


            DataTableFilterationDto booksFilteredDto = new DataTableFilterationDto
            {
                skip = int.Parse(form["start"]!),
                pageSize = int.Parse(form["length"]!),
                searchValue = form["search[value]"],
                sortColumnIndex = sortColumnIndex,
                sortColumn = form[$"columns[{sortColumnIndex}][name]"],
                sortColumnDirection = form["order[0][dir]"]


            };
            return booksFilteredDto;

        }
    }
}
