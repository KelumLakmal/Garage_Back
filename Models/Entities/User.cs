using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string UserName { get; set; }

    public required string FirstName {get; set;}
    public required string LastName {get; set;}

    public DateTime CreatedDate {get; set;}
    public DateTime? ModifiedDate {get; set;}
    public string? UserImagePath {get; set;}
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

}