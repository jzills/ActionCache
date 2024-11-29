namespace Api.Database;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string PhoneNumber { get; set; }  

    //public virtual ICollection<User> Users { get; set; }      
}