

using Model;
using Persistence;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model.Custom;

namespace Service
{
    public class UserService
    {
        public IEnumerable<UserGrid> GetAll()
        {
            var result = new List<UserGrid>() ;
            
            using (var ctx = new ApplicationDbContext())
            {
                result = (
                       from au in ctx.ApplicationUsers
                       from aur in ctx.ApplicationUserRoles.Where(x => x.UserId == au.Id).DefaultIfEmpty()
                       from ar in ctx.ApplicationRoles.Where(x => x.Id == aur.RoleId && x.Enabled).DefaultIfEmpty()
                       select new UserGrid
                       {
                           Id = au.Id,
                           nombre = au.nombre,
                           apellido = au.apellido,
                           cedula = au.cedula,
                           Role = ar.Name,
                           Email = au.Email
                       }
                       ).ToList();
            }

            return result;
        }

        public ApplicationUser Get(string id)
        {
            var result = new ApplicationUser(); 

            using (var ctx = new ApplicationDbContext())
            {
                result = ctx.ApplicationUsers.Where(x => x.Id == id).Single();
            }

            return result;
        }

        public void Update(ApplicationUser model)
        {
            var result = new List<ApplicationUser>();

            using (var ctx = new ApplicationDbContext())
            {

                var originalEntity = ctx.ApplicationUsers.Where(x => x.Id == model.Id).Single();

                originalEntity.nombre = model.Email;
                originalEntity.nombre = model.nombre;
                originalEntity.nombre = model.apellido;
                originalEntity.nombre = model.cedula;

                ctx.Entry(originalEntity).State = EntityState.Modified;
                ctx.SaveChanges();
            }
            
        }

    }
}
