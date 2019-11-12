using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essayin.Areas.Identity.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Essayin.Data
{
    public class UserService
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        UserManager<EssayinUser> UserManager { get; set; }

        public EssayinUser GetAuthenticatedUserInfo()
        {
            var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;
            var user = authState.User;
            var userinfo = UserManager.GetUserAsync(user).Result;
            return userinfo;
        }
    }

}
