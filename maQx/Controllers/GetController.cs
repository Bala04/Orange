using maQx.Models;
using maQx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Security.Claims;
using System.Linq.Expressions;

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
            return await Format<Organization, IOrganization>(Roles.AppAdmin, db.Organizations, "OrganizationsController", x => x.ActiveFlag);
        }

        [HttpGet]
        public async Task<JsonResult> Invites()
        {
            var AccessRole = User.IsInRole(Roles.AppAdmin) ? Roles.SysAdmin : Roles.AppUser;

            if (User.IsInRole(Roles.AppAdmin))
                return await Format<Invite, IInvite>(Roles.Inviter, db.Invites, "InvitesController", x => x.ActiveFlag && x.Role == AccessRole);
            else
            {
                var Organization = User.GetOrganization();
                return await Format<Invite, IInvite>(Roles.Inviter, db.Invites, "InvitesController", x => x.ActiveFlag && x.Role == AccessRole && x.Organization.Key == Organization, x => x.Organization);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Plants()
        {
            var Organization = User.GetOrganization();
            return await Format<Plant, IPlant>(Roles.SysAdmin, db.Plants, "PlantsController", x => x.ActiveFlag && x.Organization.Key == Organization, x => x.Organization);
        }

        [HttpGet]
        public async Task<JsonResult> Divisions()
        {
            var Organization = User.GetOrganization();
            return await Format<Division, IDivision>(Roles.SysAdmin, db.Divisions, "DivisionsController", x => x.ActiveFlag && x.Plant.Organization.Key == Organization, x => x.Plant, x => x.Plant.Organization);
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
                        Users = await GetAdministrator(db.Administrators.Include("User").Include("Organization"), x => x.Role == Roles.AppUser && x.ActiveFlag && x.Organization.Key == Key, Claim.FindFirst("Organization.Name").Value);
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
                return Json(JsonExceptionViewModel.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Menus()
        {
            var Roles = User.GetRoles();
            return await Format<Menus, JsonMenuViewModel, IMenu>(null, db.Menus, null, x => Roles.Contains(x.Access), (value) =>
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

                        if (Organization == null)
                        {
                            return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                        }

                        var Invite = await db.Invites.SingleOrDefaultAsync(x => x.Username == Name);

                        if (Invite == null)
                        {
                            // BUG: return await List(Roles.AppAdmin, (System.Data.Entity.DbSet<ApplicationUser>)db.Users, null, x => x.UserName == Name, (value) =>
                            // FIX: Username for Invite sholud be access by the Role Role.Inviter. 12/12/2014
                            return await List(Roles.Inviter, (DbSet<ApplicationUser>)db.Users, null, x => x.UserName == Name, (value) =>
                            {
                                return new JsonViewModel<bool>()
                                {
                                    Value = value.Any()
                                };
                            });
                        }

                        return await new JsonViewModel<bool>() { Value = true }.toJson();
                    }

                default: return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
            }
        }

        static async Task<List<UserViewModel>> GetAdministrator(IQueryable<Administrator> Value, Expression<Func<Administrator, bool>> Exp, string Organization)
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

        private async Task<JsonResult> List<T>(string Role, DbSet<T> value, string Controller, Expression<Func<T, bool>> exp, params Expression<Func<T, object>>[] Includes)
            where T : class
        {
            return await ViewHelper.List<T, JsonViewModel>(Request, Response, Controller, Role, User, value, exp, Includes, null);
        }

        private async Task<JsonResult> List<T1, T2>(string Role, DbSet<T1> value, string Controller, Expression<Func<T1, bool>> exp, Func<List<T1>, T2> operation = null, params Expression<Func<T1, object>>[] Includes)
            where T1 : class
            where T2 : JsonViewModel
        {
            return await ViewHelper.List(Request, Response, Controller, Role, User, value, exp, Includes, operation);
        }

        private async Task<JsonResult> Format<T1, T2>(string Role, DbSet<T1> value, string Controller, Expression<Func<T1, bool>> exp, params Expression<Func<T1, object>>[] Includes)
            where T1 : class, T2
            where T2 : class, IAppBase
        {
            return await ViewHelper.Format<T1, JsonViewModel, T2>(Request, Response, Controller, Role, User, value, exp, Includes, null);
        }

        private async Task<JsonResult> Format<T1, T2, T3>(string Role, DbSet<T1> value, string Controller, Expression<Func<T1, bool>> exp, Func<List<T3>, T2> operation = null, params Expression<Func<T1, object>>[] Includes)
            where T1 : class, T3
            where T2 : JsonViewModel
            where T3 : class, IAppBase
        {
            return await ViewHelper.Format<T1, T2, T3>(Request, Response, Controller, Role, User, value, exp, Includes, operation);
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