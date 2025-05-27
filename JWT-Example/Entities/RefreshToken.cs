using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JWT_Example.Entities;

[Table("RefreshTokens")]
[Index(nameof(Token), IsUnique = true)]
public class RefreshToken
{
    [Key]
    public int UserId { get; set; }
    
    [MaxLength(128)]
    public string Token { get; set; } = null!;
    
    public DateTime ExpiresAt { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}