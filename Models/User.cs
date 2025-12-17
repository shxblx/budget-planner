using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }   

        [Required]
        public string PasswordHash { get; set; }

    }
}
