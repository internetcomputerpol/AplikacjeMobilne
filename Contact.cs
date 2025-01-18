public class Contact
{
    public string Id { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public Contact()
    {
        Id = Guid.NewGuid().ToString();
    }
}