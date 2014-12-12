using maQx.Models;
using maQx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace maQx.Controllers
{
    [AjaxOnly]
    [AuthorizeGetView]
    public class GetController : Controller
    {
        private AppContext db = new AppContext();

        [HttpGet]
        public async Task<JsonResult> Organizations()
        {
            return await List(Roles.AppAdmin, db.Organizations, "OrganizationsController", x => x.ActiveFlag);
        }

        [HttpGet]
        public async Task<JsonResult> Invites()
        {
            var AccessRole = User.IsInRole(Roles.AppAdmin) ? Roles.SysAdmin : Roles.AppUser;
            var Claim = ((ClaimsIdentity)User.Identity);

            if (User.IsInRole(Roles.AppAdmin))
                return await List(Roles.Inviter, db.Invites, "InvitesController", x => x.ActiveFlag && x.Role == AccessRole);
            else
            {
                var Key = Claim.FindFirst("Organization.Key").Value;
                return await List(Roles.Inviter, db.Invites, "InvitesController", x => x.ActiveFlag && x.Role == AccessRole && x.Organization.Key == Key);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Administrators()
        {
            try
            {
                List<UserViewModel> Users = null;
                var Claim = ((ClaimsIdentity)User.Identity);

                if (User.IsInRole(Roles.Inviter))
                {
                    if (User.IsInRole(Roles.AppAdmin))
                        Users = await GetAdministrator(db.Administrators.Include("User").Include("Organization"), x => x.Role == Roles.SysAdmin && x.ActiveFlag, null);
                    else
                    {
                        var Key = Claim.FindFirst("Organization.Key").Value;
                        Users = await GetAdministrator(db.Administrators.Include("User"), x => x.Role == Roles.AppUser && x.ActiveFlag && x.Organization.Key == Key, Claim.FindFirst("Organization.Name").Value);
                    }
                }

                if (Users == null)
                {
                    return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                }

                return await new JsonListViewModel<UserViewModel>(Users, TableTools.GetTools(Type.GetType("maQx.Controllers.Administrators"))).toJson();
            }
            catch (Exception ex)
            {
                return JsonExceptionViewModel.Get(ex).toJsonUnAsync();
            }
        }

        [HttpGet]
        public async Task<JsonResult> Menus()
        {
            var Roles = User.GetRoles();
            return await List(null, db.Menus, null, x => Roles.Contains(x.Access), (value) =>
            {
                return new JsonMenuViewModel()
                {
                    Menus = value.OrderBy(x => x.Order).ToList()
                };
            });
        }

        [HttpGet]
        public async Task<JsonResult> Exists(string id, string type)
        {
            switch (type.ToLower())
            {
                case "username":
                    {
                        var Organization = Request.QueryString["ref"] != null ? await db.Organizations.FindAsync(Request.QueryString["ref"]) : null;
                        var Name = id + "@" + Organization.Domain;

                        if (Organization != null)
                        {
                            var Invite = await db.Invites.SingleOrDefaultAsync(x => x.Username == Name);

                            if (Invite == null)
                            {
                                // BUG: return await List(Roles.AppAdmin, (System.Data.Entity.DbSet<ApplicationUser>)db.Users, null, x => x.UserName == Name, (value) =>
                                // FIX: Username for Invite sholud be access by the Role Role.Inviter. 12/12/2014
                                return await List(Roles.Inviter, (System.Data.Entity.DbSet<ApplicationUser>)db.Users, null, x => x.UserName == Name, (value) =>
                                {
                                    return new JsonViewModel<bool>()
                                    {
                                        Value = value.Any()
                                    };
                                });
                            }

                            return await new JsonViewModel<bool>() { Value = true }.toJson();
                        }

                        return await JsonErrorViewModel.GetResourceNotFoundError().toJson();
                    }

                default: return await JsonErrorViewModel.GetResourceNotFoundError().toJson();
            }
        }

        static async Task<List<UserViewModel>> GetAdministrator(IQueryable<Administrator> Value, System.Linq.Expressions.Expression<Func<Administrator, bool>> Exp, string Organization)
        {
            try
            {
                var b = String.IsNullOrWhiteSpace(Organization);
                return await Value.Where(Exp).Select(x => new UserViewModel
                 {
                     Code = x.User.Code,
                     Name = x.User.Firstname,
                     Email = x.User.Email,
                     Organization = b ? x.Organization.Name : Organization

                 }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<JsonResult> List<T>(string Role, System.Data.Entity.DbSet<T> value, string Controller, System.Linq.Expressions.Expression<Func<T, bool>> exp)
            where T : class
        {
            return await ViewHelper.List<T, JsonViewModel>(Request, Response, Controller, Role, User, value, exp, null);
        }

        private async Task<JsonResult> List<T1, T2>(string Role, System.Data.Entity.DbSet<T1> value, string Controller, System.Linq.Expressions.Expression<Func<T1, bool>> exp, Func<List<T1>, T2> operation = null)
            where T1 : class
            where T2 : JsonViewModel
        {
            return await ViewHelper.List(Request, Response, Controller, Role, User, value, exp, operation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}