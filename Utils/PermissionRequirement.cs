using Microsoft.AspNetCore.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission {get; set;}

    public PermissionRequirement(string permission)
    {
       Permission = permission; 
    }
    
}