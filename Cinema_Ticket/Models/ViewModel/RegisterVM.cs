
namespace Cinema_Ticket.Models.ViewModel
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; } = null!;
        [Required,EmailAddress]
        public string Email { get; set; } = null!;
        [Required,DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required,DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;

    }
}
