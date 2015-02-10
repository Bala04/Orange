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
            return await Format<Organization, JOrganization>(Roles.AppAdmin, db.Organizations, "OrganizationsController", x => x.ActiveFlag);
        }

        [HttpGet]
        public async Task<JsonResult> Invites()
        {
            var AccessRole = User.IsInRole(Roles.AppAdmin) ? Roles.SysAdmin : Roles.AppUser;

            if (User.IsInRole(Roles.AppAdmin))
                return await Format<Invite, JInvite>(Roles.Inviter, db.Invites, "InvitesController", x => x.ActiveFlag && x.Role == AccessRole);
            else
            {
                var Organization = User.GetOrganization();
                return await Format<Invite, JInvite>(Roles.Inviter, db.Invites, "InvitesController", x => x.ActiveFlag && x.Role == AccessRole && x.Organization.Key == Organization, x => x.Organization);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Plants()
        {
            var Organization = User.GetOrganization();
            return await Format<Plant, JPlant>(Roles.SysAdmin, db.Plants, "PlantsController", x => x.ActiveFlag && x.Organization.Key == Organization, x => x.Organization);
        }

        [HttpGet]
        public async Task<JsonResult> Divisions()
        {
            var Organization = User.GetOrganization();
            return await Format<Division, JDivision>(Roles.SysAdmin, db.Divisions, "DivisionsController", x => x.ActiveFlag && x.Plant.Organization.Key == Organization, x => x.Plant.Organization);
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
            return await Format<Menus, JsonMenuViewModel, JMenus>(null, db.Menus, null, x => Roles.Contains(x.Access), (value) =>
            {
                return new JsonMenuViewModel()
                {
                    Menus = value.OrderBy(x => x.Order).ToList()
                };
            });
        }

        [HttpGet]
        public async Task<JsonResult> DepartmentMenu(string id)
        {
            return await _DepartmentMenu(id);
        }

        private async Task<JsonResult> _DepartmentMenu(string id)
        {
            return await Format<DepartmentMenu, JsonListViewModel<JDepartmentMenu>, JDepartmentMenu>(Roles.SysAdmin, db.DepartmentMenus, null, x => x.Division.Key == id, (value) =>
            {
                return new JsonListViewModel<JDepartmentMenu>()
                {
                    List = value
                };
            }, x => x.Department.Division.Plant.Organization, x => x.Menu);
        }

        [HttpGet]
        public async Task<JsonResult> DepartmentUser(string id)
        {
            return await _DepartmentUser(id);
        }

        private async Task<JsonResult> _DepartmentUser(string id)
        {
            return await Format<DepartmentUser, JsonListViewModel<JDepartmentUser>, JDepartmentUser>(Roles.SysAdmin, db.DepartmentUsers, null, x => x.Division.Key == id, (value) =>
            {
                return new JsonListViewModel<JDepartmentUser>()
                {
                    List = value
                };
            }, x => x.Department.Division.Plant.Organization, x => x.User);
        }

        [HttpGet]
        public async Task<JsonResult> DivisionAccess(string id)
        {
            return await _DivisionAccess(id);
        }

        private async Task<JsonResult> _DivisionAccess(string id)
        {
            return await Format<AccessLevel, JsonListViewModel<JAccessLevel>, JAccessLevel>(Roles.SysAdmin, db.AccessLevels, null, x => x.User.Id == id, (value) =>
            {
                return new JsonListViewModel<JAccessLevel>()
                {
                    List = value
                };
            }, x => x.Division.Plant.Organization, x => x.User);
        }

        [HttpGet]
        public async Task<JsonResult> MappedUsers()
        {
            return await Format<DepartmentUser, JsonListViewModel<JDepartmentUser>, JDepartmentUser>(Roles.SysAdmin, db.DepartmentUsers, null, x => true, (value) =>
           {
               return new JsonListViewModel<JDepartmentUser>()
               {
                   List = value
               };
           }, x => x.Department.Division.Plant.Organization, x => x.User);
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
                case "department":
                    {
                        return await Format<Department, JsonListViewModel<JDepartment>, JDepartment>(Roles.SysAdmin, db.Departments, null, x => x.Division.Key == id && x.Access == Roles.SysAdmin, (value) =>
                        {
                            return new JsonListViewModel<JDepartment>()
                            {
                                List = value
                            };
                        }, x => x.Division);
                    }
                case "menus":
                    {
                        return await List(Roles.SysAdmin, db.Menus, null, x => x.Access == Roles.SysAdmin, (value) =>
                        {
                            return new JsonListViewModel<Menus>()
                            {
                                List = value.OrderBy(x => x.Name).ToList()
                            };
                        });
                    }
                case "department-menu":
                    {
                        return await List(Roles.SysAdmin, db.DepartmentMenus, null, x => x.Division.Key == id, (value) =>
                        {
                            return new JsonListViewModel<DepartmentMenu>()
                            {
                                List = value
                            };
                        }, x => x.Department, x => x.Menu);

                    }
                case "add-department-menu":
                    {
                        var data = Request.QueryString["ref"] != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<EntityManupulateHelper>(Request.QueryString["ref"]) : null;

                        if (data != null)
                        {
                            var Division = await db.Divisions.Include(x => x.Plant).Include(x => x.Plant.Organization).Where(x => x.Key == id).FirstOrDefaultAsync();

                            if (Division != null)
                            {
                                var Department = await db.Departments.FindAsync(data.Entity);

                                if (Department != null)
                                {
                                    var DepartmentMenu = db.DepartmentMenus;

                                    foreach (var item in data.Add)
                                    {
                                        var Menu = await db.Menus.FindAsync(item);

                                        if (Menu == null)
                                        {
                                            return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                                        }

                                        DepartmentMenu.Add(new DepartmentMenu
                                        {
                                            Division = Division,
                                            Department = Department,
                                            Menu = Menu
                                        });
                                    }

                                    foreach (var item in data.Remove)
                                    {
                                        var Menu = await db.DepartmentMenus.FindAsync(item);

                                        if (Menu == null)
                                        {
                                            return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                                        }

                                        DepartmentMenu.Remove(Menu);
                                    }

                                    Exception Exception = null;

                                    try
                                    {
                                        await db.SaveChangesAsync();
                                        return await _DepartmentMenu(Division.Key);
                                    }
                                    catch (Exception ex)
                                    {
                                        Exception = ex;
                                    }

                                    return await JsonExceptionViewModel.Get(Exception).toJson();
                                }
                            }
                        }

                        return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                    }
                case "add-department-user":
                    {
                        var data = Request.QueryString["ref"] != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<EntityManupulateHelper>(Request.QueryString["ref"]) : null;

                        if (data != null)
                        {
                            var Division = await db.Divisions.Include(x => x.Plant).Include(x => x.Plant.Organization).Where(x => x.Key == id).FirstOrDefaultAsync();

                            if (Division != null)
                            {
                                var Department = await db.Departments.FindAsync(data.Entity);

                                if (Department != null)
                                {
                                    var DepartmentUser = db.DepartmentUsers;

                                    foreach (var item in data.Add)
                                    {
                                        var User = await db.Users.Where(x => x.Id == item).FirstOrDefaultAsync();

                                        if (User == null)
                                        {
                                            return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                                        }

                                        DepartmentUser.Add(new DepartmentUser
                                        {
                                            Division = Division,
                                            Department = Department,
                                            User = User
                                        });
                                    }

                                    foreach (var item in data.Remove)
                                    {
                                        var User = await db.DepartmentUsers.FindAsync(item);

                                        if (User == null)
                                        {
                                            return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                                        }

                                        DepartmentUser.Remove(User);
                                    }

                                    Exception Exception = null;

                                    try
                                    {
                                        await db.SaveChangesAsync();
                                        return await _DepartmentUser(Division.Key);
                                    }
                                    catch (Exception ex)
                                    {
                                        Exception = ex;
                                    }

                                    return await JsonExceptionViewModel.Get(Exception).toJson();
                                }
                            }
                        }

                        return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                    }

                case "access-division":
                    {
                        var data = Request.QueryString["ref"] != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<EntityManupulateHelper>(Request.QueryString["ref"]) : null;

                        if (data != null)
                        {
                            var User = db.Users.Find(data.Entity);

                            if (User != null)
                            {
                                var AccessLevel = db.AccessLevels;

                                foreach (var item in data.Add)
                                {
                                    var Division = await db.Divisions.Where(x => x.Key == item).FirstOrDefaultAsync();

                                    if (Division == null)
                                    {
                                        return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                                    }

                                    AccessLevel.Add(new AccessLevel
                                    {
                                        Division = Division,                                        
                                        User = User
                                    });
                                }

                                foreach (var item in data.Remove)
                                {
                                    var Access = await db.AccessLevels.FindAsync(item);

                                    if (Access == null)
                                    {
                                        return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                                    }

                                    AccessLevel.Remove(Access);
                                }

                                Exception Exception = null;

                                try
                                {
                                    await db.SaveChangesAsync();
                                    return await _DivisionAccess(data.Entity);
                                }
                                catch (Exception ex)
                                {
                                    Exception = ex;
                                }

                                return await JsonExceptionViewModel.Get(Exception).toJson();
                            }
                        }

                        return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
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
            where T1 : class
            where T2 : class, IJsonBase<T1, T2>
        {
            return await ViewHelper.Format<T1, JsonViewModel, T2>(Request, Response, Controller, Role, User, value, exp, Includes, null);
        }

        private async Task<JsonResult> Format<T1, T2, T3>(string Role, DbSet<T1> value, string Controller, Expression<Func<T1, bool>> exp, Func<List<T3>, T2> operation = null, params Expression<Func<T1, object>>[] Includes)
            where T1 : class
            where T2 : JsonViewModel
            where T3 : class, IJsonBase<T1, T3>
        {
            return await ViewHelper.Format<T1, T2, T3>(Request, Response, Controller, Role, User, value, exp, Includes, operation);
        }

        public class EntityManupulateHelper
        {
            public string Entity { get; set; }
            public string[] Add { get; set; }
            public string[] Remove { get; set; }
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