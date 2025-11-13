namespace Cinema_Ticket.Models.ViewModel
{
    public class ResetPasswordVM
    {
        [Required , DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required , DataType(DataType.Password) , Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
