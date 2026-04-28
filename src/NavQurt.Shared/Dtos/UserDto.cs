namespace NavQurt.Shared.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = default!;
    public string? LastName { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];

    public string GetFullName()
    {
        if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
        {
            return UserName;
        }

        return string.Join(" ", FirstName, LastName);
    }
}
