namespace Contract.Models;

public class ApiResponse <T>
{
    public Int32 StatusCode { get; set; }
    public String Message { get; set; }
    public Boolean Success { get; set; }
    public T Data { get; set; }
}