using System.ComponentModel.DataAnnotations;

namespace Multitenancy.Models
{
    public class LoginTenant
    {
        [Key]
        public int Lid { get; set; }
        public string LUserName { get; set; }
        public string LPassword { get; set; }
        public string LIdentifier { get; set; }
    }
}
