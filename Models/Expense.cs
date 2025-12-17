using System.ComponentModel.DataAnnotations;
namespace BudgetPlanner.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date {  get; set; }

        [Required]
        public string Category { get; set; }

        public string Note { get; set; }

        [Required]
        public string UserEmail { get; set; }
    }
}
