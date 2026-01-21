using System.ComponentModel.DataAnnotations;

namespace Dashboard.Core.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Navigation property for users
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
