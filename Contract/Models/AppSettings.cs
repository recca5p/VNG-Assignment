namespace Contract.Models;

public class AppSettings
{
    public JwtSettings JwtSettings { get; set; }
    public IList<string> ExcludedPaths { get; set; }
    public AWS AWS { get; set; }
    public string ApiHost { get; set; }
    public int ResetPasswordTimeInSeconds { get; set; }
}