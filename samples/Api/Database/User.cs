namespace Api.Database;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public virtual Account Account { get; set; }
}