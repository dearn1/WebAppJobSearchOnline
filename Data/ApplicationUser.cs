using Microsoft.AspNetCore.Identity;

namespace WebAppJobSearchOnline.Data
{
    public class ApplicationUser : IdentityUser
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Address {  get; set; }
        public String Url { get; set; }

    }
}
