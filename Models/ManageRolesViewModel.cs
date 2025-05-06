using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DomKultury.Models
{
    public class ManageRolesViewModel
    {
        public List<IdentityUser> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
