namespace Contract.Models;

public class FilteredResponse<T>
{
    public Int32 Page { get; set; }
    public Int32 PageSize { get; set; }
    public Int32 TotalCount { get; set; }
    public IEnumerable<T> Data { get; set; }
}