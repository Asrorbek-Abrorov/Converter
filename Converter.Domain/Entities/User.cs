namespace Converter.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Phone { get; set; }
    public long TelegramId { get; set; }
    public string? Email { get; set; }
}
