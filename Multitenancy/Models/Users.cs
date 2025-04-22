namespace Multitenancy.Models
{
    public class Users
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
