using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task4.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public virtual string? FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public virtual string? LastName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public virtual string? FullName { get; set; }

    [PersonalData]
    public List<Message> Messages { get; set; } = new List<Message>();
}

