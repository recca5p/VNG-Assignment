namespace Contract.Models;

public class JwtSettings
{
    public string Key { get; set; }
    public int ExpiryMinutes { get; set; }
}