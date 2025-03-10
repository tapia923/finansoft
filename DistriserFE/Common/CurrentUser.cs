using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class CurrentUser
    {
        public string UserName { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string cedula { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        //public string dependencia { get; set; }


        public static CurrentUser Get
        {
            get
            {
                var user = HttpContext.Current.User;

                if (user == null)
                {
                    return null;
                }
                else if (string.IsNullOrEmpty(user.Identity.GetUserId()))
                {
                    return null;
                }

                var jUser = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.UserData).Value;

                return JsonConvert.DeserializeObject<CurrentUser>(jUser);
            }
        }

    }

    
}
