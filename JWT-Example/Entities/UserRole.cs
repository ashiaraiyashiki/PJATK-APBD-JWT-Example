using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Example.Entities;

[Table("UserRoles")]
public class UserRole
{
    [Key]
    public int Id { get; set; }

    [MaxLength(30)] 
    public string Name { get; set; } = null!;
    

    public virtual ICollection<User> Users { get; set; } = null!;
}