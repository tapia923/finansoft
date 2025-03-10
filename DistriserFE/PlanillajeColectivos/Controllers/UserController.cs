using Microsoft.AspNet.Identity.Owin;
using Model;
using Service;
using Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PlanillajeColectivos.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService _userService = new UserService();

        private ApplicationRoleManager _roleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

        private ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
        }


        public ActionResult Index()
        {

            return View(
                _userService.GetAll()     
            );
        }

        public ActionResult Get(string id)
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(
                    _userService.Get(id)
                );
        }

        public async Task<ActionResult> AddRoleToUser(string Id, string role)
        {

            var roles = await _userManager.GetRolesAsync(Id);

            if(roles.Any())
            {
                throw new Exception("Solo se puede tener un rol por usuario");
            }
            
            await _userManager.AddToRoleAsync(Id, role);

            return RedirectToAction("Index");
        }


        public async Task CreateRoles()
        {
            var roles = new List<ApplicationRole>
            {
                new ApplicationRole { Name = "Admin"},
                new ApplicationRole { Name = "User"}
            };

            foreach(var r in roles)
            {
                if(!await _roleManager.RoleExistsAsync(r.Name))
                {
                   await _roleManager.CreateAsync(r);
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public string Admin()
        {
            return "Admin";
        }

        [Authorize(Roles = "User")]
        public string Useer()
        {
            return "User";
        }

    }
}