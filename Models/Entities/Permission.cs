using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Permission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<UserPermission> UserPermissions {get; set;} = new List<UserPermission>();
}