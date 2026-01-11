using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string UserName { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

}