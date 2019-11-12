using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Essayin.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the EssayinUser class
    public class EssayinUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public string Role { get; set; }
    }
}
