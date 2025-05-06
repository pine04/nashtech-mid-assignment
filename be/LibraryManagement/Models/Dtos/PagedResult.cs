namespace LibraryManagement.Models.Dtos
{
    public class PagedResult<TResult>
    {
        public required List<TResult> Results { get; set; }
        public int TotalRecordCount { get; set; }
    }
}