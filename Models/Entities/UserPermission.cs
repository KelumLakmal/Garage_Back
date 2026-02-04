using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class UserPermission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; } = null!;
    public int PermissionId { get; set; }

    // [JsonIgnore]
    public Permission Permission { get; set; } = null!;
    public bool IsActive {get; set;}


}