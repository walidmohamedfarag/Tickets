namespace Cinema_Ticket.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; } = DateTime.Now.AddMinutes(10);
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }
    }
}
