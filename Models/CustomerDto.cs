using System.Security.Cryptography;

public class CustomerDto
{
    public int Id { get; set; }

    public required string Name {get; set;}

    public string? Mobile {get; set;}
    public string? Nic {get; set;}
    public string? Email {get; set;}
}