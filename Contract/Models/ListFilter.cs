namespace Contract.Models;

public class ListFilter
{
    public Int32 Page { get; set; } = 1;
    public Int32 PageSize { get; set; } = 10;
    public String SearchName { get; set; } = String.Empty;
    public OrderTypeEnum OrderType { get; set; }
}